using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //CenterScreenMessages
    //[SerializeField] private TMP_Text countdown = null;
    [SerializeField] private Countdown countdown;
    [SerializeField] private TMP_Text gameoverText = null;
    [SerializeField] private TMP_Text playerScoredText = null;


    //ScoreboardUI
    [SerializeField] private TMP_Text scoreTextHuman = null;
    [SerializeField] private TMP_Text scoreTextMonster = null;
    [SerializeField] private TMP_Text scoreTextHumanBG = null;
    [SerializeField] private TMP_Text scoreTextMonsterBG = null;
    [SerializeField] private TMP_Text scoreTextTimer = null;
    [SerializeField] private int gameSeconds;
    private int warriorScore = 0;
    private int monsterScore = 0;
    private int timeRemainingSeconds;

    //ChargeMeter
    [SerializeField] private GameObject chargeBar = null;
    [SerializeField] private Image chargeBarFill = null;
    [SerializeField] private TMP_Text chargeBarText = null;

    //PassMeter
    //[SerializeField] private TMP_Text passMeterText = null;
    [SerializeField] private GameObject passMeter = null;
    [SerializeField] private Image passMeterFill = null;

    //Monster and Human UI
    [SerializeField] private GameObject monsterUI = null;
    [SerializeField] private Image monsterAbility1Bar = null;
    [SerializeField] private Image monsterAbility2Bar = null;
    [SerializeField] private Image monsterAbility3Bar = null;

    [SerializeField] private GameObject player1UI = null;
    [SerializeField] private GameObject player2UI = null;
    [SerializeField] private GameObject player3UI = null;
    [SerializeField] private Image player1respawnfill = null;
    [SerializeField] private Image player2respawnfill = null;
    [SerializeField] private Image player3respawnfill = null;

    Coroutine timerCoroutine;
    GameplayManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        timeRemainingSeconds = gameSeconds;
        ShowChargeBar(false);
        ShowMonsterUI(false);
        ShowPlayerUI(false, 1);
        ShowPlayerUI(false, 2);
        ShowPlayerUI(false, 3);
        ShowPassMeter(false);
        UpdateChargeBarText("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Countdown()
    {
        countdown.gameObject.SetActive(true);
        countdown.Reset();
        countdown.Play();
        yield return new WaitForSeconds(2.3f);
        countdown.gameObject.SetActive(false);
    }

    public void ShowGameOverText(bool state)
    {
        gameoverText.gameObject.SetActive(state);
    }

    public void ShowPlayerScoredText(bool state)
    {
        playerScoredText.gameObject.SetActive(state);
    }

    public void UpdatePlayerScoredText(string player)
    {
        playerScoredText.text = "" + player + " SCORED!";
    }

    public IEnumerator ScoreTimer()
    {
        //int counter = 0;
        int minutes;
        int seconds;
        //Debug.Log("Timer: " + counter);
        //scoreTextTimer.text = "" + timeRemainingSeconds / 60 + ":" + timeRemainingSeconds % 60;
        while (timeRemainingSeconds > 0)
        {
            minutes = timeRemainingSeconds / 60;
            seconds = timeRemainingSeconds % 60;

            if (seconds < 10)
            {
                scoreTextTimer.text = "" + minutes + ":0" + seconds;
            }
            else
            {
                scoreTextTimer.text = "" + minutes + ":" + seconds;
            }
            
            //Debug.Log("Timer: " + counter);
            yield return new WaitForSeconds(1f);
            timeRemainingSeconds--;
            //counter++;
        }
        scoreTextTimer.text = "0:00";
        ShowGameOverText(true);
        Debug.Log("End Coroutine");

        // Pause Game
        Time.timeScale = 0;

        //Could potentially stop player movement with isPlaying setter here (set isPlaying to false) after connecting GM to this file
        //Its probably better to keep that kind of function in the 'GameplayManager' though
        GM.StopPlaying();
    }

    public void StartTimer()
    {
        Debug.Log("Should be started");
        timerCoroutine = StartCoroutine(ScoreTimer());
    }
    public void StopTimer()
    {
        Debug.Log("Should be stopped");
        StopCoroutine(timerCoroutine);
    }

    public void WarriorPoint()
    {
        warriorScore++;
        UpdateScoreHuman(); 
    }

    public void MonsterPoint()
    {
        monsterScore++;
        UpdateScoreMonster();
    }

    public void ResetScoreAndTime()
    {
        monsterScore = 0;
        UpdateScoreMonster();
        warriorScore = 0;
        UpdateScoreHuman();
        timeRemainingSeconds = gameSeconds;
    }

    private void UpdateScoreHuman()
    {
        scoreTextHuman.text = "" + warriorScore;
        scoreTextHumanBG.text = "" + warriorScore;
    }

    private void UpdateScoreMonster()
    {
        scoreTextMonster.text = "" + monsterScore;
        scoreTextMonsterBG.text = "" + monsterScore;
    }

    public void UpdateChargeBar(float charge)
    {
        chargeBarFill.fillAmount = charge;
    }

    public void ShowMonsterUI(bool state)
    {
        monsterUI.gameObject.SetActive(state);
    }

    public void UpdateMonsterAbility1Bar(float charge)
    {
        monsterAbility1Bar.fillAmount = charge;
    }

    public void UpdateMonsterAbility2Bar(float charge)
    {
        monsterAbility2Bar.fillAmount = charge;
    }

    public void UpdateMonsterAbility3Bar(float charge)
    {
        monsterAbility3Bar.fillAmount = charge;
    }

    public void ShowChargeBar(bool state)
    {
        chargeBar.gameObject.SetActive(state);
    }

    public void UpdateChargeBarText(string text)
    {
        chargeBarText.text = text;
    }

    public void ShowPassMeter(bool state)
    {
        passMeter.gameObject.SetActive(state);
    }

    public void UpdatePassMeter(float passMeter)
    {
        passMeterFill.fillAmount = passMeter;
    }

    public void ShowPlayerUI(bool state, int player)
    {
        if (player == 1)
        {
            player1UI.gameObject.SetActive(state);
        }
        else if (player == 2)
        {
            player2UI.gameObject.SetActive(state);
        }
        else if (player == 3)
        {
            player3UI.gameObject.SetActive(state);
        }
    }

    public void UpdatePlayerRespawnBar(float charge, int player)
    {
        if (player == 1)
        {
            player1respawnfill.fillAmount = charge;
        }
        else if (player == 2)
        {
            player2respawnfill.fillAmount = charge;
        }
        else if (player == 3)
        {
            player3respawnfill.fillAmount = charge;
        }
    }
}
