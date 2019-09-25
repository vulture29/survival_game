using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerTrapController : MonoBehaviour
{
    public GameObject tmpTrapPrefab;
    public Material tmpTrapMaterial;
    public Material tmpInvalidTrapMaterial;
    public TrapManager trapManager;
    public Transform cam;

    MainPlayerController mainPlayerController;
    int trapType;
    bool validTmpTrapFlag;
    bool trapping;
    GameObject tmpTrap;
    Ray trapRay = new Ray();
    RaycastHit trapHit;
    int floorMask;

    // Use this for initialization
    void Start () {
        mainPlayerController = GetComponent<MainPlayerController>();
        floorMask = LayerMask.GetMask("Floor");
        trapping = false;
        trapType = 0;
        tmpTrap = null;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // start putting hurt trap
            trapping = true;
            trapType = 1;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            // start putting slow trap
            trapping = true;
            trapType = 2;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // cancel tmp trap
            trapping = false;
            if (tmpTrap != null)
            {
                Destroy(tmpTrap);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // put trap down
            if (trapping && validTmpTrapFlag)
            {
                trapping = false;
                // Generate trap
                if (trapType == 1)
                {
                    trapManager.GenerateHurtTrap(tmpTrap.transform.position, mainPlayerController.playerId);
                }
                else if (trapType == 2)
                {
                    trapManager.GenerateSlowTrap(tmpTrap.transform.position, mainPlayerController.playerId);
                }
                Destroy(tmpTrap);
            }
        }

        if (trapping)
        {
            // update trap position due to ray 
            UpdateTmpTrap();
        }
    }

    void UpdateTmpTrap()
    {
        if (tmpTrap == null)
        {
            // create tmp trap if not trap exist
            tmpTrap = Instantiate(tmpTrapPrefab);
        }
        trapRay.origin = cam.transform.position;
        trapRay.direction = cam.transform.forward;
        // raycast to floor mask
        if (Physics.Raycast(trapRay, out trapHit, float.MaxValue, floorMask))
        {
            Vector3 hitPoint = trapHit.point;
            tmpTrap.transform.position = GameUtility.GetRoundVector3(hitPoint);

            // check if trap position is valid 
            if (trapManager.CheckValidTrapPoint(hitPoint))
            {
                validTmpTrapFlag = true;
                tmpTrap.GetComponent<MeshRenderer>().material = tmpTrapMaterial;
            }
            else
            {
                validTmpTrapFlag = false;
                tmpTrap.GetComponent<MeshRenderer>().material = tmpInvalidTrapMaterial;
            }

        }
    }
}
