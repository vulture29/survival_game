using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System;

public class NetworkManager
{
    public static string username;
    public string data = "init";
    public bool IsConnected = false;

    Socket serverSocket;
    IPAddress ipAddr; 
    IPEndPoint ipEnd;
    byte[] recvDataBuf = new byte[1024];
    byte[] sendDataBuf = new byte[] { };
    Thread sockThread;
    GameController gameController;

    public NetworkManager()
    {
        // Set connection endpoint
        ipAddr = IPAddress.Parse("127.0.0.1");
        ipEnd = new IPEndPoint(ipAddr, 9998);
    }

    // Start socket thread
    public void StartUp(GameController gameC)
    {
        gameController = gameC;
        sockThread = new Thread(new ThreadStart(StartSocket));
        sockThread.Start();
    }

    void StartSocket()
    {
        // connect socket and start listening
        SocketConnet();
        SocketListen();
    }

    void SocketConnet()
    {
        // Connect
        if (serverSocket != null)
            serverSocket.Close();
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.NoDelay = true;
        try
        {
            serverSocket.Connect(ipEnd);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            IsConnected = false;
        }
        IsConnected = true;
    }

    // Send data to buffer
    public void SocketSend(string sendStr)
    {
        int size = sendStr.Length + Configuration.NET_HEAD_LENGTH_SIZE;

        byte[] wsize = BitConverter.GetBytes(size);
        byte[] sendDataByte = Encoding.ASCII.GetBytes(sendStr);
        byte[] rawData = wsize.Concat(sendDataByte).ToArray();

        SendRaw(rawData);
    }

    // Write raw data to buffer
    void SendRaw(byte[] data)
    {
        if (!BitConverter.IsLittleEndian)
            Array.Reverse(data);

        sendDataBuf = sendDataBuf.Concat(data).ToArray();
        Process();
    }

    // Process buffer
    void Process()
    {
        if (IsConnected && serverSocket.Connected)
        {
            TrySend();
        }
    }

    // Send data from buffer to socket
    // Return the length of sent bytes
    int TrySend()
    {
        int wsize = 0;

        if (sendDataBuf.Length == 0)
            return 0;
        try
        {
            wsize = serverSocket.Send(sendDataBuf);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            return -1;
        }

        sendDataBuf = sendDataBuf.Skip(wsize).ToArray();

        return wsize;
    }

    // Read data from socket every 100 miliseconds
    void SocketListen()
    {
        while (true)
        {
            if (serverSocket.Poll(-1, SelectMode.SelectRead))
            {
                // receive the length of the message
                int recvLen = 0;
                byte[] recvData_length_str = new byte[4];
                while (recvLen < 4)
                {
                    recvLen += serverSocket.Receive(recvData_length_str, recvLen, 4, SocketFlags.None);
                }
                int recvData_length = BitConverter.ToInt32(recvData_length_str, 0);

                // receive the original data
                recvLen = 0;
                string recvStr = "";
                while (recvLen < recvData_length)
                {
                    if (recvData_length - recvLen > 1024)
                    {
                        recvLen += serverSocket.Receive(recvDataBuf, 0, 1024, SocketFlags.None);
                        recvStr += Encoding.ASCII.GetString(recvDataBuf, 0, 1024);
                    }
                    else
                    {
                        int recvLen_tmp = recvData_length - recvLen;
                        recvLen += serverSocket.Receive(recvDataBuf, 0, recvData_length - recvLen, SocketFlags.None);
                        recvStr += Encoding.ASCII.GetString(recvDataBuf, 0, recvLen_tmp);
                    }
                }
                gameController.recvMsg(recvStr);
            }
        }
    }

    public void SocketQuit()
    {
        // disconnect
        IsConnected = false;
        if (sockThread != null)
        {
            sockThread.Interrupt();
            sockThread.Abort();
        }
        if (serverSocket != null)
            serverSocket.Close();
    }
}
