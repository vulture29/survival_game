using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public GameObject playerPrefab;
    public ParticleSystem hitParticles;

    public Dictionary<string, GameObject> playerDict;

    // Use this for initialization
    void Start () {
        playerDict = new Dictionary<string, GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdatePlayer(string pid, string position, string rotation, string health)
    {
        if (playerDict.ContainsKey(pid))
        {
            // update player
            PlayerController playerController = playerDict[pid].GetComponent<PlayerController>();
            playerController.UpdatePosHealth(position, rotation, health);
        }
        else
        {
            // create new player
            GameObject playerObj = Instantiate(playerPrefab, GameUtility.Vector2StrToVector3(position), new Quaternion(), GetComponent<Transform>());
            playerDict.Add(pid, playerObj);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.id = pid;
        }
    }

    public void PlayerAttack(string eid, Vector3 attackPosition)
    {
        // perform player attack
        if (playerDict.ContainsKey(eid))
        {
            PlayerShooting playerShooting = playerDict[eid].GetComponent<PlayerController>().playerShooting;
            playerShooting.Shoot(attackPosition);
        }
    }
    
    public void PlayerMagicAttack(string eid, Vector3 attackPosition)
    {
        // perform player magic attack
        if (playerDict.ContainsKey(eid))
        {
            PlayerShooting playerShooting = playerDict[eid].GetComponent<PlayerController>().playerShooting;
            playerShooting.ShootMagic(attackPosition);
        }
    }


    public void Dead(string pid)
    {
        PlayerController playerController = playerDict[pid].GetComponent<PlayerController>();
        playerController.Dead();
    }

    public void ResetPlayers()
    {
        // remove all players
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Start();
    }
}
