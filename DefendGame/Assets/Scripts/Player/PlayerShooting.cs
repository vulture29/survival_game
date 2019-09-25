using UnityEngine;


public class PlayerShooting : MonoBehaviour
{
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public GameObject skillObject;
    public Transform skillParent;

    float timer;                                    // A timer to determine when to fire.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;                           // Reference to the audio source.
    Light gunLight;                                 // Reference to the light component.
    public Light faceLight;								// Duh
    float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

    void Awake()
    {
        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();
        faceLight = GetComponentInChildren<Light>();
    }

    void Update()
    {
        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }


    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLine.enabled = false;
        faceLight.enabled = false;
        gunLight.enabled = false;
    }

    public void ShootMagic(Vector3 shootPosition)
    {
        Shoot(shootPosition, true);
    }


    public void Shoot(Vector3 shootPosition, bool skill = false)
    {
        // Set color
        if (skill)
        {
            ParticleSystem.MainModule main = gunParticles.main;
            main.startColor = Color.cyan;
            faceLight.color = Color.gray;
            gunLight.color = Color.blue;
            gunLine.material.color = Color.cyan;
        }
        else
        {
            ParticleSystem.MainModule main = gunParticles.main;
            main.startColor = Color.yellow;
            faceLight.color = Color.yellow;
            gunLight.color = Color.yellow;
            gunLine.material.color = Color.white;
        }

        // Reset the timer.
        timer = 0f;

        // Play the gun shot audioclip.
        gunAudio.Play();

        // Enable the lights.
        gunLight.enabled = true;
        faceLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, shootPosition);

        if (skill)
        {
            Destroy(Instantiate(skillObject, shootPosition, Quaternion.identity, skillParent), 3f);
        }
    }
}