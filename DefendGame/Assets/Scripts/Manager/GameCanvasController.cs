using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasController : MonoBehaviour {
    public GameController gameController;
    public GameObject winTxt;
    public GameObject failTxt;
    public GameObject timerTxt;
    public Slider skillCDSlider;
    public float skillCD;
    public float timerValue;
    public Text arrivedTxt;
    public Text healthTxt;
    public Text moneyTxt;
    public Text waveTxt;
    public Text levelTxt;
    public Text hurtLevelTxt;
    public Text slowLevelTxt;

    public float timer;

    bool gameEndFlag;
    float skillTimer;

	// Use this for initialization
	void Start ()
    {
        winTxt.SetActive(false);
        failTxt.SetActive(false);
        gameEndFlag = false;
        skillTimer = 0;
        timer = timerValue;
    }
	
	// Update is called once per frame
	void Update () {
        // display timer if needed
        if (Mathf.Floor(timer) >= 0)
        {
            timerTxt.GetComponent<Text>().text = Mathf.Floor(timer).ToString();
            timer -= Time.deltaTime;
        }
        else
        {
            timerTxt.SetActive(false);
        }

        if (gameEndFlag && Input.GetKeyDown(KeyCode.Return))
        {
            // restart game when game end
            gameController.SocketSend("LoginService", "restartGame", "", "");
            gameEndFlag = false;
            winTxt.SetActive(false);
            failTxt.SetActive(false);
        }

        // update magic skill timer
        if (skillTimer >= 0)
        {
            skillTimer -= Time.deltaTime;
        }
        skillCDSlider.value = skillTimer / skillCD;
	}

    public void UpdateCanvas(string arrivedEnemy, string money, string wave, string healthStr, string level, string hurtTrapLevel, string slowTrapLevel)
    {
        // set canvas value
        arrivedTxt.text = "Arrived Enemy: " + arrivedEnemy;
        moneyTxt.text = "Money: " + money;
        waveTxt.text = "Wave: " + wave;
        healthTxt.text = "Health: " + healthStr;
        levelTxt.text = "Lv: " + level;
        hurtLevelTxt.text = "Lv: " + hurtTrapLevel;
        slowLevelTxt.text = "Lv: " + slowTrapLevel;
    }

    public void ResetCanvas()
    {
        Start();
    }

    public void StartSkillCD()
    {
        skillTimer = skillCD;
    }

    public void PlayerWin()
    {
        winTxt.SetActive(true);
        gameEndFlag = true;
    }

    public void PlayerFail()
    {
        failTxt.SetActive(true);
        gameEndFlag = true;
    }

    public void PlayTimer()
    {
        timerTxt.SetActive(true);
        timer = timerValue;
    }
}
