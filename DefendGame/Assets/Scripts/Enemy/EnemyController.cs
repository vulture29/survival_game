using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {
    public GameObject floatingTxtPrefab;
    public HellephantShooting hellephantShooting;
    public string id;
    public Vector3 startPosition;
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;
    public bool isDead;
    public int health;
    public Slider healthSlider;
    public float maxHealth;
    public float destroyTime = 3f;

    public bool isSinking;

    ParticleSystem hitParticles;
    Vector3 target;
    float time;
    float timeToReachTarget;
    CapsuleCollider capsuleCollider;
    Animator anim;
    AudioSource enemyAudio;
    Rigidbody rb;
    float yRotation;

    // Use this for initialization
    void Start () {
		startPosition = transform.position;
        target = transform.position;
        capsuleCollider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        enemyAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // update health slider value
        healthSlider.value = health / maxHealth * 100f;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (isSinking)
        {
            // sinking enemy when dead
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
        }
        else
        {
            // move enemy position with lerp
            time += Time.deltaTime / timeToReachTarget;
            rb.MovePosition(Vector3.Lerp(startPosition, target, time));
            rb.MoveRotation(Quaternion.Euler(0, yRotation, 0));
        }
    }

    void SetDestination(Vector3 destination, float reachTime)
    {
        // set enemy's position to go 
        time = 0;
        startPosition = transform.position;
        timeToReachTarget = reachTime;
        target = destination;
    }

    void TakeDamage(int damage)
    {
        if(floatingTxtPrefab)
        {
            // player hurt animation
            GameObject floatingText = Instantiate(floatingTxtPrefab, transform);
            floatingText.GetComponent<TextMesh>().color = Color.white;
            floatingText.GetComponent<TextMesh>().text = damage.ToString();
            Destroy(floatingText, 1f);
        }
    }

    public void UpdatePosHealth(string position, string rotation, string heal)
    {
        // update enemy position, rotation and health
        yRotation = float.Parse(rotation);
        Vector3 dest = GameUtility.Vector2StrToVector3(position);
        
        SetDestination(dest, 0.1f);

        if (int.Parse(heal) < health)
        {
            TakeDamage(health - int.Parse(heal));
        }
        health = int.Parse(heal);
    }

    public void Death()
    {
        // enemy dead
        isDead = true;

        capsuleCollider.isTrigger = true;

        anim.SetTrigger("Dead");

        enemyAudio.clip = deathClip;
        enemyAudio.Play();

        StartSinking();
    }

    public void Arrive()
    {
        // enemy arrive target
        StartSinking();
    }

    void StartSinking()
    {
        // start sinking enemy and destroy it
        GetComponent<Rigidbody>().isKinematic = true;
        isSinking = true;
        Destroy(gameObject, 2f);
    }
}
