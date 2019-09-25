using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public GameObject gameParent;
    public GameObject loginParent; 
    public MainPlayerController mainPlayerController;
    public EnemyManager enemyManager;
    public PlayerManager playerManager;
    public TrapManager trapManager;
    public GameCanvasController gameCanvasController;

    NetworkManager networkManager;

    delegate void MessageHandler(string msg);
    Dictionary<string, MessageHandler> commandMap;

    Queue<RecvMessage> recvMsgQueue;

    void Awake()
    {
        networkManager = new NetworkManager();
        commandMap = new Dictionary<string, MessageHandler>();
        recvMsgQueue = new Queue<RecvMessage>();
    }

    // Use this for initialization
    void Start () {

        networkManager.StartUp(this);
        gameParent.SetActive(false);
        loginParent.SetActive(true);

        // Register command
        commandMap.Add("initPlayerPosition", new MessageHandler(InitPlayerPosition));
        commandMap.Add("updatePlayerEid", new MessageHandler(UpdatePlayerEid));
        commandMap.Add("syncPlayer", new MessageHandler(SyncPlayer));
        commandMap.Add("syncEnemy", new MessageHandler(SyncEnemy));
        commandMap.Add("syncTrap", new MessageHandler(SyncTrap));
        commandMap.Add("playerShoot", new MessageHandler(PlayerShoot));
        commandMap.Add("playerMagicShoot", new MessageHandler(PlayerMagicShoot));
        commandMap.Add("playerDead", new MessageHandler(PlayerDead)); 
        commandMap.Add("enemyDead", new MessageHandler(EnemyDead));
        commandMap.Add("enemyArrive", new MessageHandler(EnemyArrive));
        commandMap.Add("zombearAttackPlayer", new MessageHandler(EnemyAttackPlayer));
        commandMap.Add("hellephantAttackPlayer", new MessageHandler(EnemyAttackPlayer)); 
        commandMap.Add("updateWave", new MessageHandler(UpdateWave));
        commandMap.Add("playerWin", new MessageHandler(PlayerWin)); 
        commandMap.Add("playerFail", new MessageHandler(PlayerFail));
        commandMap.Add("updateArrivedEnemy", new MessageHandler(UpdateArrivedEnemy));
        commandMap.Add("restartGame", new MessageHandler(RestartGame));
    }
	
	// Update is called once per frame
	void Update () {
        while (recvMsgQueue != null && recvMsgQueue.Count > 0)
        {
            RecvMessage message = recvMsgQueue.Dequeue();
            HandleMsg(message);
        }
    }

    public void SocketSend(string service, string command, string data, string eid = "")
    {
        // send message to server
        Message msg = new Message(service, command, data, mainPlayerController.playerId, eid);
        networkManager.SocketSend(Message.MsgToStr(msg));
    }

    public void recvMsg(string data)
    {
        // receive message from server
        if (data[0] != '{' || data[data.Length-1] != '}')
        {
            Debug.Log("invalid message");
            return;
        }
        RecvMessage msg = RecvMessage.StrToMsg(data);

        recvMsgQueue.Enqueue(msg);
    }

    void HandleMsg(RecvMessage msg)
    {
        // handle message and refect to corresponding function
        if (commandMap.ContainsKey(msg.command))
        {
            MessageHandler messageHandler = commandMap[msg.command];
            messageHandler(msg.args);
        }
    }

    public void Login(string username, string passwd)
    {
        // Send username info and login
        if (username == null || username.Length <= 0 || passwd == null || passwd.Length <= 0)
        {
            return;
        }
        SocketSend("LoginService", "login", passwd, username);
    }

    public void Register(string username, string passwd)
    {

        // Send username info and register
        if (username == null || username.Length <= 0 || passwd == null || passwd.Length <= 0)
        {
            return;
        }
        SocketSend("LoginService", "register", passwd, username);
    }

    void PlayerWin(string message)
    {
        // player win
        gameCanvasController.PlayerWin();
    }

    void PlayerFail(string message)
    {
        // player fail
        gameCanvasController.PlayerFail();
    }

    void InitPlayerPosition(string message)
    {
        mainPlayerController.InitPlayerPosition(message);
    }

    void UpdatePlayerEid(string message)
    {
        // set game state from login to in-game
        gameParent.SetActive(true);
        loginParent.SetActive(false);

        // lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // update player entity id
        mainPlayerController.playerId = message;
        mainPlayerController.GetComponent<MainPlayerMovment>().StartUpdatePlayer();
    }

    public void SyncEnemy(string message)
    {
        // sync all enemy
        string[] enemyArgs = message.Split(';');

        for (int i = 0; i < enemyArgs.Length; i++)
        {
            // one enemy info
            string[] args = enemyArgs[i].Split(':');
            if (args.Length != 5)
            {
                return;
            }
            enemyManager.UpdateEnemy(args[0], args[1], args[2], args[3], args[4]);
        }
    }

    public void EnemyAttackPlayer(string message)
    {
        // enemy attack player
        string[] args = message.Split(':');
        if (args.Length != 3)
        {
            return;
        }
        string pid = args[2];
        Vector3 playerPosition = new Vector3();

        // Find player posiiton by pid
        if (pid.Equals(mainPlayerController.playerId))
        {
            playerPosition = mainPlayerController.transform.position;
        }
        else
        {
            playerPosition = playerManager.playerDict[pid].GetComponent<PlayerController>().transform.position;
        }

        enemyManager.EnemyAttack(args[0], args[1], playerPosition);
    }

    public void SyncPlayer(string message)
    {
        // sync player info
        string[] playerArgs = message.Split(';');
        for (int i = 0; i < playerArgs.Length; i++)
        {
            // one player
            string[] args = playerArgs[i].Split(':');
            if (args.Length != 8)
            {
                return;
            }
            if (args[0].Equals(mainPlayerController.playerId))
            {
                // Update main player info
                mainPlayerController.UpdateMainPlayerInfo(args[3], args[4], args[5], args[6], args[7]);
            }
            else {
                // update other player info using playermanager
                playerManager.UpdatePlayer(args[0], args[1], args[2], args[3]);
            }
        }
    }

    public void SyncTrap(string message)
    {
        // sync all trap
        string[] trapArgs = message.Split(';');

        for (int i = 0; i < trapArgs.Length; i++)
        {
            // one trap
            string[] args = trapArgs[i].Split(':');
            if (args.Length != 3)
            {
                return;
            }
            // update trap using trapManager
            trapManager.UpdateTrap(args[0], args[1], args[2]);
        }
    }

    public void EnemyDead(string message)
    {
        // enemy dead
        EnemyController enemyController = enemyManager.enemyDict[message].GetComponent<EnemyController>();
        enemyController.Death();
    }

    public void EnemyArrive(string message)
    {
        // enemy arrived target
        EnemyController enemyController = enemyManager.enemyDict[message].GetComponent<EnemyController>();
        enemyController.Arrive();
    }

    public void PlayerDead(string message)
    {
        // player dead
        if (message.Equals(mainPlayerController.playerId))
        {
            // main player dead
            mainPlayerController.Dead();
        }
        else
        {
            // other player dead
            playerManager.Dead(message);
        }
    }

    public void PlayerShoot(string message)
    {
        // other player shoot
        string[] args = message.Split(':');
        if (args.Length != 2)
        {
            return;
        }

        Vector3 shootPoint = GameUtility.Vector3StrToVector3(args[1]);

        playerManager.PlayerAttack(args[0], shootPoint);

    }

    public void PlayerMagicShoot(string message)
    {
        // player magic shoot
        string[] args = message.Split(':');
        if (args.Length != 2)
        {
            return;
        }

        Vector3 shootPoint = GameUtility.Vector3StrToVector3(args[1]);

        string playerId = args[0];
        if (playerId.Equals(mainPlayerController.playerId))
        {
            // main player magic shoot
            mainPlayerController.PlayerMagicShooting(shootPoint);
        }
        else {
            // other player magic shoot
            playerManager.PlayerMagicAttack(args[0], shootPoint);
        }

    }

    public void UpdateWave(string message)
    {
        // udpate wave info
        mainPlayerController.UpdateWave(message);
    }

    public void UpdateArrivedEnemy(string message)
    {
        // update arrived enemy number
        mainPlayerController.updateArrivedEnemy(message);
    }

    public void RestartGame(string message)
    {
        // restart game and reset objects
        Debug.Log("Restart game");
        ResetEnemy();
        ResetPlayer();
        ResetOtherPlayer();
        ResetTrap();
        ResetCanvas();
    }

    void ResetCanvas()
    {
        gameCanvasController.ResetCanvas();
    }

    void ResetEnemy()
    {
        enemyManager.ResetEnemy();
    }

    void ResetPlayer()
    {
        mainPlayerController.ResetPlayer();
    }

    void ResetOtherPlayer()
    {
        playerManager.ResetPlayers();
    }

    void ResetTrap()
    {
        trapManager.ResetTrap();
    }
}
