using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayerController : MonoBehaviour
{
    public GameObject floatingTxtPrefab;
    public string playerId;
    public MainPlayerShooting playerShooting;
    public bool isDead;
    public AudioClip deathClip;
    public GameCanvasController gameCanvasController;
    public Slider healthSlider;

    int health;
    string healthStr;
    string wave;
    string arrivedEnemy;
    string money;
    string level;
    string hurtTrapLevel;
    string slowTrapLevel;

    AudioSource playerAudio;
    Animator anim;
    Rigidbody rb;
    GameController gameController;
    

    // Use this for initialization
    void Start () {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        playerAudio = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        wave = "1";
        arrivedEnemy = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            // no action for dead player
            return;
        }

        // update health slider value
        healthSlider.value = health;

        // update canvas value
        gameCanvasController.UpdateCanvas(arrivedEnemy, money, wave, healthStr, level, hurtTrapLevel, slowTrapLevel);

        // capture player level up
        if (Input.GetKeyDown(KeyCode.C))
        {
            gameController.SocketSend("LevelService", "playerLevelUp", "", playerId);
        }
        // capture hurt trap level up
        if (Input.GetKeyDown(KeyCode.V))
        {
            gameController.SocketSend("LevelService", "hurtTrapLevelUp", "", playerId);
        }
        // capture slow trap level up
        if (Input.GetKeyDown(KeyCode.B))
        {
            gameController.SocketSend("LevelService", "slowTrapLevelUp", "", playerId);
        }
    }

    void TakeDamage(int damage)
    {
        // Player damage animation
        if (floatingTxtPrefab)
        {
            GameObject floatingText = Instantiate(floatingTxtPrefab, transform);
            floatingText.GetComponent<TextMesh>().color = Color.red;
            floatingText.GetComponent<TextMesh>().text = damage.ToString();
            Destroy(floatingText, 1f);
        }
    }

    public void InitPlayerPosition(string message)
    {
        transform.position = GameUtility.Vector2StrToVector3(message);
    }

    public void Dead()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;

        // Turn off any remaining shooting effects.
        playerShooting.DisableEffects();

        rb.isKinematic = true;

        // Tell the animator that the player is dead.
        anim.SetTrigger("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        playerAudio.clip = deathClip;
        playerAudio.Play();
    }

    public void PlayerMagicShooting(Vector3 shootPoint)
    {
        // play magic shooting and set skill CD
        playerShooting.PlayerMagicShooting(shootPoint);
        gameCanvasController.StartSkillCD();
    }

    public void UpdateMainPlayerInfo(string updateHealth, string updateMoney, string updateLevel, string updateHurtTrapLevel, string updateSlowTrapLevel)
    {
        // update player infomation
        if (int.Parse(updateHealth) < health)
        {
            TakeDamage(health - int.Parse(updateHealth));
        }
        health = int.Parse(updateHealth);
        healthStr = updateHealth;
        money = updateMoney;
        level = updateLevel;
        hurtTrapLevel = updateHurtTrapLevel;
        slowTrapLevel = updateSlowTrapLevel;
    }

    public void UpdateWave(string updateWave)
    {
        // update current wave
        wave = updateWave;
        gameCanvasController.PlayTimer();
    }

    public void updateArrivedEnemy(string updateArrivedEnemy)
    {
        // update arrived enemy number
        arrivedEnemy = updateArrivedEnemy;
    }

    public void DeadOption()
    {
        // play when player dead
        gameCanvasController.PlayerFail();
    }

    public void ResetPlayer()
    {
        // reset player
        isDead = false;
        anim.SetTrigger("Reset");
    }
}
