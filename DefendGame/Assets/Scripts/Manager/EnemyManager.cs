using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject zomBearPrefab;
    public GameObject hellephantPrefab;
    public Vector3 playerOffset;

    public Dictionary<string, GameObject> enemyDict;

    // Use this for initialization
    void Start () {
        enemyDict = new Dictionary<string, GameObject>();
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void UpdateEnemy(string eid, string type, string position, string rotation, string health)
    {
        if (enemyDict.ContainsKey(eid))
        {
            // update zombear or hellephant
            EnemyController enemyController = enemyDict[eid].GetComponent<EnemyController>();
            enemyController.UpdatePosHealth(position, rotation, health);
        }
        else if(type.Equals("1"))
        {
            // create new zombear
            GameObject zombearObj = Instantiate(zomBearPrefab, GameUtility.Vector2StrToVector3(position),
                Quaternion.Euler(0, float.Parse(rotation), 0), GetComponent<Transform>());
            enemyDict.Add(eid, zombearObj);
            EnemyController enemyController = zombearObj.GetComponent<EnemyController>();
            enemyController.id = eid;
        }
        else if (type.Equals("2"))
        {
            // create new hellephant
            GameObject hellephantObj = Instantiate(hellephantPrefab, GameUtility.Vector2StrToVector3(position),
                Quaternion.Euler(0, float.Parse(rotation), 0), GetComponent<Transform>());
            enemyDict.Add(eid, hellephantObj);
            EnemyController enemyController = hellephantObj.GetComponent<EnemyController>();
            enemyController.id = eid;
        }
    }

    public void EnemyAttack(string eid, string type, Vector3 attackPosition)
    {
        if (enemyDict.ContainsKey(eid))
        {
            EnemyController enemyController = enemyDict[eid].GetComponent<EnemyController>();
            if (type.Equals("1"))
            {
                // no attack animation, do nothing
            }
            else if (type.Equals("2"))
            {
                // play hellephant attack player animation
                HellephantShooting hellephantShooting = enemyController.hellephantShooting;
                hellephantShooting.AttackPlayer(attackPosition + playerOffset);
            }
        }
    }

    public void ResetEnemy()
    {
        // remove all enemy
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        enemyDict = new Dictionary<string, GameObject>();
    }
}
