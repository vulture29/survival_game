using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellephantShooting : MonoBehaviour {
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    AudioSource gunAudio;

    float timer;
    float effectsDisplayTime = 0.05f;

    // Use this for initialization
    void Start () {
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();

        ParticleSystem.MainModule main = gunParticles.main;
        main.startColor = Color.red;
        gunLine.material.color = Color.red;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (timer >= effectsDisplayTime)
        {
            // ... disable the effects.
            gunLine.enabled = false;
        }
    }

    public void AttackPlayer(Vector3 attackPosition)
    {
        // enemy attack player
        timer = 0f;

        gunAudio.Play();

        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, attackPosition);

    }
}
