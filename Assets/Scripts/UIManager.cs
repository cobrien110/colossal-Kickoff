using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //CenterScreenMessages
    [SerializeField] private TMP_Text countdown = null;
    [SerializeField] private TMP_Text gameoverText = null;
    
    //ScoreboardUI
    [SerializeField] private TMP_Text scoreTextHuman = null;
    [SerializeField] private TMP_Text scoreTextMonster = null;
    [SerializeField] private TMP_Text scoreTextTimer = null;
    private int warriorScore = 0;
    private int monsterScore = 0;
    private int timeRemainingSeconds = 10;

    Coroutine timerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Countdown()
    {
        countdown.text = "3";
        countdown.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown.text = "2";
        yield return new WaitForSeconds(1f);
        countdown.text = "1";
        yield return new WaitForSeconds(1f);
        countdown.text = "GO";
        yield return new WaitForSeconds(1f);
        countdown.gameObject.SetActive(false);
    }

    private void ShowGameOverText()
    {
        gameoverText.gameObject.SetActive(true);
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
        ShowGameOverText();
        Debug.Log("End Coroutine");

        //Could potentially stop player movement with isPlaying setter here (set isPlaying to false) after connecting GM to this file
        //Its probably better to keep that kind of function in the 'GameplayManager' though
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

    public void warriorPoint()
    {
        warriorScore++;
        updateScoreHuman(); 
    }

    public void monsterPoint()
    {
        monsterScore++;
        updateScoreMonster();
    }

    private void updateScoreHuman()
    {
        scoreTextHuman.text = "" + warriorScore;
    }

    private void updateScoreMonster()
    {
        scoreTextMonster.text = "" + monsterScore;
    }
}
