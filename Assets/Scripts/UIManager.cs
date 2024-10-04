using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{


    //CenterScreenMessages
    //[SerializeField] private TMP_Text countdown = null;
    [SerializeField] private Countdown countdown;
    [SerializeField] private TMP_Text gameoverText = null;
    [SerializeField] private TMP_Text playerScoredText = null;

    public bool menuReturn = false;


    //UpperScoreboardUI
    [SerializeField] private GameObject topScoreboard = null;
    [SerializeField] private TMP_Text scoreTextHuman = null;
    [SerializeField] private TMP_Text scoreTextMonster = null;
    [SerializeField] private TMP_Text scoreTextHumanBG = null;
    [SerializeField] private TMP_Text scoreTextMonsterBG = null;
    [SerializeField] private TMP_Text scoreTextTimer = null;
    [SerializeField] private int gameSeconds;
    private int warriorScore = 0;
    private int monsterScore = 0;
    private int timeRemainingSeconds;

    //(Stats) ScoreboardUI
    [SerializeField] private GameObject statsScoreboard = null;

    [SerializeField] private TMP_Text Warrior1GoalsText = null;
    [SerializeField] private TMP_Text Warrior2GoalsText = null;
    [SerializeField] private TMP_Text Warrior3GoalsText = null;

    [SerializeField] private TMP_Text Warrior1DeathsText = null;
    [SerializeField] private TMP_Text Warrior2DeathsText = null;
    [SerializeField] private TMP_Text Warrior3DeathsText = null;

    [SerializeField] private TMP_Text Warrior1StealsText = null;
    [SerializeField] private TMP_Text Warrior2StealsText = null;
    [SerializeField] private TMP_Text Warrior3StealsText = null;

    [SerializeField] private TMP_Text MonsterGoalsText = null;
    [SerializeField] private TMP_Text MonsterKillsText = null;  
    [SerializeField] private TMP_Text MonsterAbUsedText = null;


    // Dev Stats
    [SerializeField] private GameObject devStats = null;
    [SerializeField] private TMP_Text gameWinnerText = null;

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

    //Dev Stats and/or stats to add to Scoreboard
    /* Kills with specific abilities
     * Timestamps and player ID of goals
     * Goal count as well by player
     * Steal count
     * Pass count
     * Monster and Warrior dribble time
     * 
     */

    Coroutine timerCoroutine;
    GameplayManager GM;
    StatTracker ST;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        timeRemainingSeconds = gameSeconds;
        ShowChargeBar(false);
        ShowMonsterUI(false);
        //ShowPlayerUI(false, 1);
        //ShowPlayerUI(false, 2);
        //ShowPlayerUI(false, 3);
        //ShowPassMeter(false);
        UpdateChargeBarText("");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (statsScoreboard.activeInHierarchy)
            {
                ShowStatsScoreboard(false);
            }
            else if (!devStats.activeInHierarchy)
            {
                ShowStatsScoreboard(true);
            }
            else if (devStats.activeInHierarchy)
            {
                ShowDevStats(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (statsScoreboard.activeInHierarchy)
            {
                ShowStatsScoreboard(false);
                ShowDevStats(true);
            }
            else if (devStats.activeInHierarchy)
            {
                ShowDevStats(false);
                ShowStatsScoreboard(true);
            }
        }

        // Copy game stats to Clipboard
        if (Input.GetKeyDown(KeyCode.C))
        {
            string textToCopy = "";

            if (timeRemainingSeconds <= 0)
            {
                textToCopy = "Stats from Game:\n";
                    
            }
            else
            {
                textToCopy = "Stats from " + timeRemainingSeconds + " seconds:\n";
            }

            textToCopy += "\nWinner: " + gameWinnerText.text;

            textToCopy += "\nScore: " + warriorScore + " - " + monsterScore;

            textToCopy += "\nKills: " + MonsterKillsText.text;
            
            CopyToClipboard(textToCopy);
        }
    }

    public IEnumerator Countdown()
    {
        countdown.Reset();
        countdown.gameObject.SetActive(true);
        countdown.Play();
        yield return new WaitForSeconds(3.0f);
        countdown.gameObject.SetActive(false);
    }

    public void ShowGameOverText(bool state, int winner)
    {
        if (state) {
            if (winner == 0)
            {
                gameoverText.text = "HUMANS WIN!";
                gameWinnerText.text = "Humans";
            }
            else if (winner == 1)
            {
                gameoverText.text = "MONSTERS WIN!";
                gameWinnerText.text = "Monster";
            }
            else if (winner == 2)
            {
                gameoverText.text = "TIE GAME!";
                gameWinnerText.text = "Tie";
            }
        }
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
        timeRemainingSeconds = -1;

        ShowGameOverText(true, CheckWinner());
        ST.UpdateGameWinner(CheckWinner());
        ShowTopScoreboard(false);
        ShowStatsScoreboard(true);
        //Debug.Log("End Coroutine");

        // Pause Game


        //Could potentially stop player movement with isPlaying setter here (set isPlaying to false) after connecting GM to this file
        //Its probably better to keep that kind of function in the 'GameplayManager' though
        //GM.StopPlaying();

        if (menuReturn)
        {
            Debug.Log("Back to Menu");
            SceneManager.LoadScene("MainMenus");
        }
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

    public int GetTimeRemaining()
    {
        return timeRemainingSeconds;
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

    public int CheckWinner()
    {

        if (warriorScore > monsterScore) return 0;
        else if (monsterScore > warriorScore) return 1;
        else return 2;
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
        passMeterFill.fillAmount = passMeter / 100f;
    }

    public void ShowPlayerUI(bool state, int player)
    {
        Debug.Log("Showing UI sucessfully for player: " + player);
        if (player == 1)
        {
            Debug.Log("Calling UI player 1");
            player1UI.gameObject.SetActive(state);
        }
        else if (player == 2)
        {
            Debug.Log("Calling UI player 2");
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

    public void ShowTopScoreboard(bool state)
    {
        topScoreboard.gameObject.SetActive(state);
    }

    public void ShowStatsScoreboard(bool state)
    {
        statsScoreboard.gameObject.SetActive(state);
    }

    public void ShowDevStats(bool state)
    {
        devStats.gameObject.SetActive(state);
    }

    public void UpdateWarriorGoalsSB(int player)
    {
        if (player == 1) Warrior1GoalsText.text = "" + ST.GetWGoals(1);
        if (player == 2) Warrior2GoalsText.text = "" + ST.GetWGoals(2);
        if (player == 3) Warrior3GoalsText.text = "" + ST.GetWGoals(3);
    }

    public void UpdateWarriorDeathsSB(int player)
    {
        if (player == 1) Warrior1DeathsText.text = "" + ST.GetWDeaths(1);
        if (player == 2) Warrior2DeathsText.text = "" + ST.GetWDeaths(2);
        if (player == 3) Warrior3DeathsText.text = "" + ST.GetWDeaths(3);
    }

    public void UpdateWarriorStealsSB(int player)
    {
        if (player == 1) Warrior1StealsText.text = "" + ST.GetWSteals(1);
        if (player == 2) Warrior2StealsText.text = "" + ST.GetWSteals(2);
        if (player == 3) Warrior3StealsText.text = "" + ST.GetWSteals(3);
    }
    public void UpdateMonsterKillsSB()
    {
        MonsterKillsText.text = "" + ST.GetMKills();
    }

    public void UpdateMonsterAbilitiesSB()
    {
        MonsterAbUsedText.text = "" + ST.GetMAbUsed();
    }

    public void UpdateMonsterGoalsSB()
    {
        MonsterGoalsText.text = "" + ST.GetMGoals();
    }

    public void UpdateGameWinner(string winner)
    {
        gameWinnerText.text = winner;
    }

    void CopyToClipboard(string s)
    {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }
}
