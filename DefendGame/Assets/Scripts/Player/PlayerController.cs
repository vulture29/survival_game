using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public string id;
    public int health;
    public PlayerShooting playerShooting;
    Animator anim;
    public float timeToReachTarget;
    public bool isDead;
    public Slider healthSlider;

    float time;
    Vector3 targetPosition;
    Vector3 startPosition;
    float targetRotation;
    Rigidbody rb;
    bool walking = false;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // set health slider value
        healthSlider.value = health;
    }

    void FixedUpdate()
    {
        // perform movement and rotation
        PerformMovementRotation();
        anim.SetBool("IsWalking", walking);
    }

    void PerformMovementRotation()
    {
        // perform movement with lerp
        time += Time.fixedDeltaTime / timeToReachTarget;
        rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, time));

        // directly rotate player
        rb.MoveRotation(Quaternion.Euler(new Vector3(0, targetRotation, 0)));
    }

    public void UpdatePosHealth(string position, string rotation, string heal)
    {
        // update player's position, rotation and health
        time = 0;
        startPosition = transform.position;
        targetPosition = GameUtility.Vector2StrToVector3(position);

        // calcultate player's velocity
        Vector3 velocity = targetPosition - transform.position;
        if (velocity.x > 0.1 || velocity.y > 0.1 || velocity.z > 0.1)
        {
            walking = true;
        }
        else
        {
            walking = false;
        }
        targetRotation = float.Parse(rotation);
        health = int.Parse(heal);
    }

    public void Dead()
    {
        isDead = true;

        // Turn off any remaining shooting effects.
        playerShooting.DisableEffects();

        rb.isKinematic = true;

        // Tell the animator that the player is dead.
        anim.SetTrigger("Die");
    }
}
