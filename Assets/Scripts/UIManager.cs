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
    [SerializeField] private Image monsterAbility1Bar = null;
    [SerializeField] private TMP_Text chargeBarText = null;

    //PassMeter
    [SerializeField] private TMP_Text passMeterText = null;

    Coroutine timerCoroutine;
    GameplayManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        timeRemainingSeconds = gameSeconds;
        ShowChargeBar(false);
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

    private void ShowGameOverText(bool state)
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

    public void UpdateMonsterAbility1Bar(float charge)
    {
        monsterAbility1Bar.fillAmount = charge;
    }

    public void ShowChargeBar(bool state)
    {
        chargeBar.gameObject.SetActive(state);
    }

    public void UpdateChargeBarText(string text)
    {
        chargeBarText.text = text;
    }

    public void UpdatePassMeterText(int passMeter)
    {
        passMeterText.text = "Pass Meter: " + passMeter;
    }
}
