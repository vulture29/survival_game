using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour {
    public GameController gameController;
    public GameObject hurtTrapPrefab;
    public GameObject slowTrapPrefab;

    public Dictionary<string, GameObject> trapDict;

    HashSet<string> trapPosStrSet;

    // Use this for initialization
    void Start()
    {
        trapDict = new Dictionary<string, GameObject>();
        trapPosStrSet = new HashSet<string>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void GenerateHurtTrap(Vector3 position, string playerId)
    {
        // player generate hurt trap
        string vec2PositionStr = (new Vector2(position.x, position.z)).ToString();
        trapPosStrSet.Add(vec2PositionStr);
        gameController.SocketSend("TrapService", "generateHurtTrap", vec2PositionStr, playerId);
    }

    public void GenerateSlowTrap(Vector3 position, string playerId)
    {
        // player generate slow trap
        string vec2PositionStr = (new Vector2(position.x, position.z)).ToString();
        trapPosStrSet.Add(vec2PositionStr);
        gameController.SocketSend("TrapService", "generateSlowTrap", vec2PositionStr, playerId);
    }

    public bool CheckValidTrapPoint(Vector3 hitPoint)
    {
        // check if the trap position is valid
        return !trapPosStrSet.Contains(hitPoint.ToString());
    }

    public void UpdateTrap(string eid, string position, string type)
    {
        // update trap info
        if (trapDict == null || trapDict.ContainsKey(eid))
        {
            return;
        }
        else if (type.Equals("1"))
        {
            // hurt trap
            GameObject trapObj = Instantiate(hurtTrapPrefab, GameUtility.Vector2StrToVector3(position), Quaternion.identity, GetComponent<Transform>());
            trapDict.Add(eid, trapObj);
        }
        else if (type.Equals("2"))
        {
            // slow trap
            GameObject trapObj = Instantiate(slowTrapPrefab, GameUtility.Vector2StrToVector3(position), Quaternion.identity, GetComponent<Transform>());
            trapDict.Add(eid, trapObj);
        }
    }

    public void ResetTrap()
    {
        // remove all trap
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Start();
    }
}
