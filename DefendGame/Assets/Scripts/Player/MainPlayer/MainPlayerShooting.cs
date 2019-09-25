using UnityEngine;


public class MainPlayerShooting : MonoBehaviour
{
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float skillCd = 5f;
    public float range = 100f;                      // The distance the gun can fire.
    public GameObject skillObject;
    public Transform skillParent;
    public GameObject hitParticlesPrefab;

    float timer;                                    // A timer to determine when to fire.
    float skillTimer;
    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;                           // Reference to the audio source.
    Light gunLight;                                 // Reference to the light component.
    public Light faceLight;								// Duh
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

    GameController gameController;
    MainPlayerController pc;

    void Awake()
    {
        // Create a layer mask for the Shootable layer.
        shootableMask = LayerMask.GetMask("Shootable");

        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
        faceLight = GetComponentInChildren<Light>();

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        pc = transform.parent.gameObject.GetComponent<MainPlayerController>();

        skillTimer = skillCd;
    }


    void Update()
    {
        if (pc.isDead)
        {
            return;
        }
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;
        skillTimer += Time.deltaTime;

        // If the Fire1 button is being press and it's time to fire...
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            // ... shoot the gun.
            ParticleSystem.MainModule main = gunParticles.main;
            main.startColor = Color.yellow;
            faceLight.color = Color.yellow;
            gunLight.color = Color.yellow;
            gunLine.material.color = Color.white;
            Shoot();
        }
        if (Input.GetButton("Fire2") && skillTimer >= skillCd && Time.timeScale != 0)
        {
            // shoot magic
            ShootMagic();
        }
        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }

    void ShootMagic()
    {
        // set skill cd timer to 0
        skillTimer = 0f;
        Shoot(true);
    }


    void Shoot(bool skill = false)
    {
        timer = 0f;
        
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            EnemyController enemyController = shootHit.collider.GetComponent<EnemyController>();

            // hit enemy, send attack request
            if (enemyController != null)
            {
                if (skill)
                {
                    gameController.SocketSend("AttackService", "shootMagicAttack", enemyController.id, pc.playerId);
                }
                else
                {
                    gameController.SocketSend("AttackService", "shootAttack", enemyController.id, pc.playerId);
                }
            }

            if (skill)
            {
                gameController.SocketSend("AttackService", "shootMagic", shootHit.point.ToString(), pc.playerId);
            }
            else
            {
                PlayShoot(shootHit.point);
                gameController.SocketSend("AttackService", "shoot", shootHit.point.ToString(), pc.playerId);
            }
        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer to the fullest extent of the gun's range.
            Vector3 shootPoint = shootRay.origin + shootRay.direction * range;
            if (skill)
            {
                //Destroy(Instantiate(skillObject, shootHit.point, Quaternion.identity, skillParent), 3f);
                gameController.SocketSend("AttackService", "shootMagic", shootPoint.ToString(), pc.playerId);
            }
            else
            {
                PlayShoot(shootPoint);
                gameController.SocketSend("AttackService", "shoot", shootPoint.ToString(), pc.playerId);
            }
        }
    }

    void PlayShoot(Vector3 shootPoint)
    {
        // playe shoot effect
        gunAudio.Play();

        gunLight.enabled = true;
        faceLight.enabled = true;

        gunParticles.Stop();
        gunParticles.Play();

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, shootPoint);

        gunParticles.Stop();
        gunParticles.Play();

        GameObject hitParticlesObj = Instantiate(hitParticlesPrefab, shootPoint,
            Quaternion.LookRotation(shootPoint - transform.position));
        ParticleSystem hitParticles = hitParticlesObj.GetComponent<ParticleSystem>();
        hitParticles.Play();
        Destroy(hitParticlesObj, 2f);
    }

    public void PlayerMagicShooting(Vector3 shootPoint)
    {
        // play magic shoot effect once get server response
        ParticleSystem.MainModule main = gunParticles.main;
        main.startColor = Color.cyan;
        faceLight.color = Color.gray;
        gunLight.color = Color.blue;
        gunLine.material.color = Color.cyan;

        PlayShoot(shootPoint);

        Destroy(Instantiate(skillObject, shootHit.point, Quaternion.identity, skillParent), 3f);
    }

    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLine.enabled = false;
        faceLight.enabled = false;
        gunLight.enabled = false;
    }
}