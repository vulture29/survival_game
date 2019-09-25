using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message {
    // message class to send
    // contain service id, command id, value, username, entity id
    public string sid;
    public string cid;
    public string value;
    public string username;
    public string eid;

    public Message(string s, string c, string v, string u, string e)
    {
        this.sid = s;
        this.cid = c;
        this.value = v;
        this.username = u;
        this.eid = e;
    }

    public static Message StrToMsg(string jsonString)
    {
        return JsonUtility.FromJson<Message>(jsonString);
    }

    public static string MsgToStr(Message msg)
    {
        return JsonUtility.ToJson(msg);
    }
}

[System.Serializable]
public class RecvMessage
{
    // message class to receive
    // contain command string and args
    public string command;
    public string args;

    public RecvMessage(string c, string a)
    {
        this.command = c;
        this.args = a;
    }

    public static RecvMessage StrToMsg(string jsonString)
    {
        return JsonUtility.FromJson<RecvMessage>(jsonString);
    }

    public static string MsgToStr(RecvMessage msg)
    {
        return JsonUtility.ToJson(msg);
    }
}
