using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerMovment : MonoBehaviour {
    public float moveSpeed;
    public float rotateSensitivity;
    public Transform cam;
    public Animator anim;
    public float cameraRotationUpLimit;
    public float cameraRotationDownLimit;

    GameController gameController;
    MainPlayerController playerController;
    Rigidbody rb;
    Vector3 velocity = Vector3.zero;
    float yRotation = 0f;
    float xRotation = 0f;
    float currentRotationX = 0f;
    bool walking;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        playerController = GetComponent<MainPlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // freeze if player is dead
        if (!playerController.isDead)
        {
            // set velocity
            float horizontalMov = Input.GetAxisRaw("Horizontal");
            float verticalMov = Input.GetAxisRaw("Vertical");

            Vector3 horizontalMovVec = transform.right * horizontalMov;
            Vector3 verticalMovVec = transform.forward * verticalMov;

            velocity = horizontalMovVec + verticalMovVec;
            velocity.y = 0f;
            velocity = velocity.normalized * moveSpeed;

            // set rotation
            float yRot = Input.GetAxisRaw("Mouse X");
            float xRot = Input.GetAxisRaw("Mouse Y");

            yRotation = yRot * rotateSensitivity;
            xRotation = xRot * rotateSensitivity;
        }
    }
    
    void FixedUpdate()
    {
        if (!playerController.isDead)
        {
            // perform movement and rotation
            PerformMovement();
            PerformRotation();

            anim.SetBool("IsWalking", walking);
        }
    }

    void PerformMovement()
    {
        // set walking flag if velocity is non-zero
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            walking = true;
        }
        else
        {
            walking = false;
        }
    }
    
    void PerformRotation()
    {
        // rotate player around y axis
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0, yRotation, 0)));

        // rotate camera around x axis
        if (cam != null)
        {
            // set max rotation
            currentRotationX -= xRotation;
            currentRotationX = Mathf.Clamp(currentRotationX, -cameraRotationDownLimit, cameraRotationUpLimit);
            
            cam.transform.localEulerAngles = new Vector3(currentRotationX, 0f, 0f);
        }
    }

    IEnumerator UpdatePlayerCoroutine()
    {
        Start();
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (playerController.playerId.Length > 0)
            {
                gameController.SocketSend("MoveService", "updatePlayerPosition", new Vector2(rb.position.x, rb.position.z).ToString(), playerController.playerId);
                gameController.SocketSend("MoveService", "updatePlayerRotation", transform.eulerAngles.y.ToString(), playerController.playerId);
            }
        }
    }

    public void StartUpdatePlayer()
    {
        // update current position and rotation to game server
        StartCoroutine(UpdatePlayerCoroutine());
    }
}
