using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;

public class UIManager : MonoBehaviour
{
    //InGameUI
    [SerializeField] private GameObject allInGameUI = null;

    //CenterScreenMessages
    [Header("CenterScreenMessages")]
    //[SerializeField] private TMP_Text countdown = null;
    [SerializeField] private Countdown countdown;
    [SerializeField] private TMP_Text gameoverText = null;
    [SerializeField] private TMP_Text playerScoredText = null;
    [SerializeField] private GameObject pauseScreen = null;
    [SerializeField] private GameObject hideWhenPaused = null;
    [SerializeField] private GameObject overtimeGraphic = null;

    //UpperScoreboardUI
    [Header("UpperScoreboardUI")]
    [SerializeField] private GameObject topScoreboard = null;
    [SerializeField] private TMP_Text scoreTextHuman = null;
    [SerializeField] private TMP_Text scoreTextMonster = null;
    [SerializeField] private TMP_Text scoreTextHumanBG = null;
    [SerializeField] private TMP_Text scoreTextMonsterBG = null;
    [SerializeField] private TMP_Text scoreTextTimer = null; 
    [SerializeField] private int warriorScore = 0;
    [SerializeField] private int monsterScore = 0;
    private int timeRemainingSeconds;

    //(Stats) ScoreboardUI
    [Header("ScoreboardUI")]
    [SerializeField] private GameObject statsScoreboard = null;

    [SerializeField] private TMP_Text[] WarriorLabels = null;
    [SerializeField] private TMP_Text MonsterLabel = null;

    [SerializeField] private TMP_Text Warrior1GoalsText = null;
    [SerializeField] private TMP_Text Warrior2GoalsText = null;
    [SerializeField] private TMP_Text Warrior3GoalsText = null;

    [SerializeField] private TMP_Text Warrior1AssistsText = null;
    [SerializeField] private TMP_Text Warrior2AssistsText = null;
    [SerializeField] private TMP_Text Warrior3AssistsText = null;

    [SerializeField] private TMP_Text Warrior1DeathsText = null;
    [SerializeField] private TMP_Text Warrior2DeathsText = null;
    [SerializeField] private TMP_Text Warrior3DeathsText = null;

    [SerializeField] private TMP_Text Warrior1StealsText = null;
    [SerializeField] private TMP_Text Warrior2StealsText = null;
    [SerializeField] private TMP_Text Warrior3StealsText = null;

    [SerializeField] private TMP_Text MonsterGoalsText = null;
    [SerializeField] private TMP_Text MonsterKillsText = null;  
    [SerializeField] private TMP_Text MonsterAbUsedText = null;

    [SerializeField] private GameObject P1MVP = null;
    [SerializeField] private GameObject P2MVP = null;
    [SerializeField] private GameObject P3MVP = null;

    [SerializeField] private TMP_Text winnerTextSB = null;

    // Dev Stats
    [SerializeField] private GameObject devStats = null;
    [SerializeField] private TMP_Text gameWinnerText = null;

    //ChargeMeter
    [Header("ChargeMeter")]
    [SerializeField] private GameObject chargeBar = null;
    [SerializeField] private Image chargeBarFill = null;
    [SerializeField] private TMP_Text chargeBarText = null;

    

    //PassMeter
    //[SerializeField] private TMP_Text passMeterText = null;
    /*[SerializeField] private GameObject passMeter = null;
    [SerializeField] private Image passMeterFill = null;*/

    //ContestBar
    [Header("ContestBar")]
    [SerializeField] private Image warriorContestFill = null;
    private Color warriorBarStartColor = Color.white;
    [SerializeField] private Image monsterContestFill = null;
    //[SerializeField] private Image middleContestFill = null;

    //Monster and Human UI
    [Header("Monster and Human UI")]
    [SerializeField] private GameObject monsterUI = null;
    [SerializeField] private Image monsterAbility1Bar = null;
    [SerializeField] private Image monsterAbility2Bar = null;
    [SerializeField] private Image monsterAbility3Bar = null;

    [SerializeField] private Image player1Icon = null;
    [SerializeField] private Image player2Icon = null;
    [SerializeField] private Image player3Icon = null;
    [SerializeField] private Image player1Alive = null;
    [SerializeField] private Image player2Alive = null;
    [SerializeField] private Image player3Alive = null;
    [SerializeField] private Image player1Dead = null;
    [SerializeField] private Image player2Dead = null;
    [SerializeField] private Image player3Dead = null;

    [SerializeField] private Sprite humanSpriteAlive = null;
    [SerializeField] private Sprite humanSpriteDead = null;
    [SerializeField] private Sprite robotSpriteAlive = null;
    [SerializeField] private Sprite robotSpriteDead = null;
    //I might change how this looks to just change the image component and color instead of having different objects (I will at some point)

    [SerializeField] private Image monsterAbility1Icon = null;
    [SerializeField] private Image monsterAbility2Icon = null;
    [SerializeField] private Image monsterAbility3Icon = null;
    [SerializeField] private TMP_Text monsterAbility1Text = null;
    [SerializeField] private TMP_Text monsterAbility2Text = null;
    [SerializeField] private TMP_Text monsterAbility3Text = null;

    [SerializeField] private GameObject controlsHolder = null;
    [SerializeField] private GameObject RBgo = null;
    private bool isSuperFlashing = false;
    //private int[] playerIds = new int[4];

    public bool isSuperKickWindow = false;
    [SerializeField] private float superKickWindowTime = 10.0f;

    [SerializeField] private AudioPlayer AP;

    [SerializeField] private TMP_InputField console = null;

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
    /*Coroutine ability1TimerCoroutine;
    Coroutine ability2TimerCoroutine;
    Coroutine ability3TimerCoroutine;*/
    GameplayManager GM;
    StatTracker ST;
    //TMP_InputField console;

    public bool overtime = false;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        //console = GameObject.Find("Canvas").GetComponentInChildren<TMP_InputField>();
        timeRemainingSeconds = GM.gameSeconds;
        warriorBarStartColor = warriorContestFill.color;
        ShowChargeBar(false);
        ShowMonsterUI(false);
        //ShowPlayerUI(false, 1);
        //ShowPlayerUI(false, 2);
        //ShowPlayerUI(false, 3);
        //ShowPassMeter(false);
        UpdateChargeBarText("");

        UpdatePlayerLabels();

        //PlayerPrefs Show Control
        if (PlayerPrefs.GetInt("showControls") == 0)
        {
            controlsHolder.SetActive(false);
        } else
        {
            RBgo = controlsHolder.GetComponentInChildren<Image>().gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (console.gameObject.activeInHierarchy && !console.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                console.gameObject.SetActive(false);
            } 
        }
        
        else if (!console.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && GM.debugMode)
            {
                console.gameObject.SetActive(true);
            }

            //if (Input.GetKeyDown(KeyCode.Tab))
            //{
            //    if (statsScoreboard.activeInHierarchy)
            //    {
            //        ShowStatsScoreboard(false);
            //    }
            //    else if (!devStats.activeInHierarchy)
            //    {
            //        ShowStatsScoreboard(true);
            //    }
            //    else if (devStats.activeInHierarchy)
            //    {
            //        ShowDevStats(false);
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    //if (statsScoreboard.activeInHierarchy)
            //    //{
            //    //    ShowStatsScoreboard(false);
            //    //    ShowDevStats(true);
            //    //}
            //    //else if (devStats.activeInHierarchy)
            //    //{
            //    //    ShowDevStats(false);
            //    //    ShowStatsScoreboard(true);
            //    //}

            //    if (!statsScoreboard.activeInHierarchy)
            //    {
            //        ShowStatsScoreboard(true);
            //    } 
            //    else
            //    {
            //        ShowStatsScoreboard(false);
            //    }
            //}
        }

        if (isSuperKickWindow)
        {
            GM.passMeter -= Time.deltaTime / superKickWindowTime;
            UpdateWarriorContestBar(GM.passMeter);
        }
    }

    public IEnumerator Countdown()
    {
        countdown.Reset();
        countdown.gameObject.SetActive(true);
        countdown.Play();
        yield return new WaitForSeconds(3.5f);
        countdown.gameObject.SetActive(false);
    }

    public void ShowGameOverText(bool state, int winner)
    {
        if (state) {
            if (winner == 0)
            {
                gameoverText.text = "WARRIORS WIN!";
                gameWinnerText.text = "WARRIORS";
            }
            else if (winner == 1)
            {
                gameoverText.text = "MONSTER WINS!";
                gameWinnerText.text = "MONSTER";
            }
            else if (winner == 2)
            {
                gameoverText.text = "TIE GAME!";
                gameWinnerText.text = "TIE";
            }
        }
        gameoverText.gameObject.SetActive(state);
    }

    public void ShowOvertime(bool state)
    {
        Debug.Log("OVERTIME" + state);
        overtimeGraphic.SetActive(state);
    }

    public void ShowPlayerScoredText(bool state)
    {
        playerScoredText.gameObject.SetActive(state);
    }

    public void UpdatePlayerScoredText(int player, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGBA(color);
        Debug.Log("HEX: " + hex);

        if (player == 0) playerScoredText.text = "CPU SCORED!";
        else if (player == 11) playerScoredText.text = "HUMANS SCORED!";
        else if (player == 12) playerScoredText.text = "MONSTER SCORED!";
        else if (player > 0) playerScoredText.text = "<color=#" + hex + ">PLAYER " + (player) + "</color> SCORED!";
        else playerScoredText.text = "PLAYER " + (player * -1) + " OWN GOALED!";


        //<color=#ff0000ff>
        //</color>
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

        //Check winner
        if (warriorScore != monsterScore)
        {
            overtime = false;
            ShowGameOverText(true, CheckWinner());
            ST.UpdateGameWinner(CheckWinner());
            
            //Hides everything under 'in game ui holder' on canvas and pops up scoreboard
            //ShowInGameUI(false);
            //ShowStatsScoreboard(true);
        }

        //OT
        else if (warriorScore == monsterScore)
        {
            Overtime(PlayerPrefs.GetInt("overtime"));
        }

        //Debug.Log("End Coroutine");

        // Pause Game

        //Could potentially stop player movement with isPlaying setter here (set isPlaying to false) after connecting GM to this file
        //Its probably better to keep that kind of function in the 'GameplayManager' though
        //GM.StopPlaying();
    }


    //New Monster Ability UI
    /*private IEnumerator AbilityTimer(int ability, int cooldown)
    {
        if (ability == 1)
        {
            monsterAbility1Text.text = "" + cooldown;
            yield return new WaitForSeconds(1f);
            cooldown--;
        }
        else if (ability == 2)
        {
            monsterAbility1Text.text = "" + cooldown;
            yield return new WaitForSeconds(1f);
            cooldown--;
        }
        else if (ability == 3)
        {
            monsterAbility1Text.text = "" + cooldown;
            yield return new WaitForSeconds(1f);
            cooldown--;
        }
    }*/

    public void UpdateAbilityTimerText(int ability, float cooldown) 
    {
        if (ability == 1)
        {
            monsterAbility1Text.text = "" + Mathf.Ceil(cooldown);
        }
        else if (ability == 2)
        {
            monsterAbility2Text.text = "" + Mathf.Ceil(cooldown);
        }
        else if (ability == 3)
        {
            monsterAbility3Text.text = "" + Mathf.Ceil(cooldown);
        }
    }

    /*public void StartAbilityTimer(int ability, int cooldown)
    {
        if (ability == 1) ability1TimerCoroutine = StartCoroutine(AbilityTimer(ability, cooldown));

        else if (ability == 2) ability2TimerCoroutine = StartCoroutine(AbilityTimer(ability, cooldown));

        else if (ability == 3) ability3TimerCoroutine = StartCoroutine(AbilityTimer(ability, cooldown));
    }

    public void StopAbilityTimer(int ability)
    {
        if (ability == 1) StopCoroutine(ability1TimerCoroutine);

        else if (ability == 2) StopCoroutine(ability2TimerCoroutine);

        else if (ability == 3) StopCoroutine(ability3TimerCoroutine);
    }*/


    public void ShowAbilityText(bool state, int ability)
    {
        if (ability == 1) monsterAbility1Text.gameObject.SetActive(state);

        else if (ability == 2) monsterAbility2Text.gameObject.SetActive(state);

        else if (ability == 3) monsterAbility3Text.gameObject.SetActive(state);
    }

    public void MonsterIconGreyout(bool state, int ability)
    {
        if (ability == 1)
        {
            if (state) monsterAbility1Icon.color = new Color(0.3f, 0.3f, 0.3f);
            else monsterAbility1Icon.color = new Color(1.0f, 1.0f, 1.0f);
        }

        else if (ability == 2)
        {
                if (state) monsterAbility2Icon.color = new Color(0.3f, 0.3f, 0.3f);
                else monsterAbility2Icon.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else if (ability == 3)
        {
            if (state) monsterAbility3Icon.color = new Color(0.3f, 0.3f, 0.3f);
            else monsterAbility3Icon.color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void UpdateAbilityIcons()
    {
        monsterAbility1Icon.sprite = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>().abilities[0].icon;
        monsterAbility2Icon.sprite = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>().abilities[1].icon;
        monsterAbility3Icon.sprite = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>().abilities[2].icon;
    }

    //End New Monster Ability UI

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

    public void Overtime(int pref)
    {
        //if (overtime == true) return;
        overtime = true;
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("pauseWhistle"));
        ShowOvertime(true);
        Debug.Log("UI ENTERING OT STYLE " + pref);
        //Standard
        if (pref == 0)
        {
            //Debug.Log("UI ENTERING OT STYLE " + pref);
            scoreTextTimer.text = "OT";
            timeRemainingSeconds = 60;
            BallProperties BP = GM.GetBall().GetComponent<BallProperties>();
            BP.ResetBall();
            //ShowOvertime(false);
            GM.OvertimeMusic();
        }

        //Sudden Death
        if (pref == 1)
        {
            scoreTextTimer.text = "OT";
            timeRemainingSeconds = 0;
            BallProperties BP = GM.GetBall().GetComponent<BallProperties>();
            BP.ResetBall();
            //ShowOvertime(false);
            GM.OvertimeMusic();
        }
    }

    public IEnumerator SuddenDeath()
    {
        while (warriorScore == monsterScore)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3.05f);
        overtime = false;
        ShowGameOverText(true, CheckWinner());
        ST.UpdateGameWinner(CheckWinner());

        //Hides everything under 'in game ui holder' on canvas and pops up scoreboard
        ShowInGameUI(false);
        yield break;
    }

    public void SuddenDeathStart()
    {
        StartCoroutine(SuddenDeath());
    }

    public int GetTimeRemaining()
    {
        return timeRemainingSeconds;
    }

    public void SetTimeRemaining(int set)
    {
        timeRemainingSeconds = set;
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
        timeRemainingSeconds = GM.gameSeconds;
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

    /*public void ShowPassMeter(bool state)
    {
        passMeter.gameObject.SetActive(state);
    }

    public void UpdatePassMeter(float passMeter)
    {
        passMeterFill.fillAmount = passMeter;
    }*/

    public void ShowPlayerUI(bool state, int player)
    {
        Debug.Log("Showing UI sucessfully for player: " + player);
        if (player == 1)
        {
            Debug.Log("Calling UI player 1");
            player1Icon.gameObject.SetActive(state);
            
        }
        else if (player == 2)
        {
            Debug.Log("Calling UI player 2");
            player2Icon.gameObject.SetActive(state);
        }
        else if (player == 3)
        {
            player3Icon.gameObject.SetActive(state);
        }
    }

    public void PlayerIconGreyout(bool state, int player)
    {
        if (player == 1)
        {
            if (state)
            {
                player1Alive.gameObject.SetActive(false);
                player1Dead.gameObject.SetActive(true);
            }
            else
            {
                player1Alive.gameObject.SetActive(true);
                player1Dead.gameObject.SetActive(false);
            }
        }

        else if (player == 2)
        {
            if (state)
            {
                player2Alive.gameObject.SetActive(false);
                player2Dead.gameObject.SetActive(true);
            }
            else
            {
                player2Alive.gameObject.SetActive(true);
                player2Dead.gameObject.SetActive(false);
            }
        }
        else if (player == 3)
        {
            if (state)
            {
                player3Alive.gameObject.SetActive(false);
                player3Dead.gameObject.SetActive(true);
            }
            else
            {
                player3Alive.gameObject.SetActive(true);
                player3Dead.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateWarriorContestBar(float charge)
    {
        warriorContestFill.fillAmount = charge;
        
        //if (warriorContestFill.fillAmount == 1 && !isSuperFlashing)
        if (isSuperKickWindow && !isSuperFlashing)
        {
            Debug.Log("STARTING SUPER KICK FLASH");
            isSuperFlashing = true;
            //InvokeRepeating("SuperKickFlash", .4f, 0.5f);
            InvokeRepeating("SuperMeterFlash", .2f, 0.25f);
        }
        
        //if (warriorContestFill.fillAmount == 1 && monsterContestFill.fillAmount == 1)
        //{
        //    ShowMiddleContestBar(true);
        //}

        //if (warriorContestFill.fillAmount < 1.0f)
        if (!isSuperKickWindow && isSuperFlashing)
        {
            //ShowMiddleContestBar(false);
            Debug.Log("STOPPING SUPER KICK FLASH");
            //CancelInvoke("SuperKickFlash");
            CancelInvoke("SuperMeterFlash");
            isSuperFlashing = false;
            RBgo.SetActive(true);
            warriorContestFill.color = warriorBarStartColor;
        }
    }

    private void SuperKickFlash()
    {
        RBgo.SetActive(!RBgo.activeInHierarchy);
        
    }

    private void SuperMeterFlash()
    {
        warriorContestFill.color = warriorContestFill.color.Equals(warriorBarStartColor) ? Color.white : warriorBarStartColor;
    }

    public void UpdateMonsterContestBar(float charge, Color c)
    {
        monsterContestFill.fillAmount = charge;
        
        //if (warriorContestFill.fillAmount == 1 && monsterContestFill.fillAmount == 1)
        //{
        //    ShowMiddleContestBar(true);
        //}

        //else if (middleContestFill.gameObject.activeInHierarchy && warriorContestFill.fillAmount < 1.0f)
        //{
        //    ShowMiddleContestBar(false);
        //}

        if (!monsterContestFill.color.Equals(c))
        {
            monsterContestFill.color = c;
        }
    }

    //public void ShowMiddleContestBar(bool state)
    //{
    //    middleContestFill.gameObject.SetActive(state);
    //}

    public void ShowTopScoreboard(bool state)
    {
        topScoreboard.gameObject.SetActive(state);
    }

    public void ShowStatsScoreboard(bool state)
    {
        statsScoreboard.gameObject.SetActive(state);
        EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonRematch"));
    }

    public void ShowInGameUI(bool state)
    {
        allInGameUI.gameObject.SetActive(state);
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

    public void UpdateWarriorAssistsSB(int player)
    {
        if (player == 1) Warrior1AssistsText.text = "" + ST.GetWAssists(1);
        if (player == 2) Warrior2AssistsText.text = "" + ST.GetWAssists(2);
        if (player == 3) Warrior3AssistsText.text = "" + ST.GetWAssists(3);
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

    public void UpdateWinnerTextSB(string winner)
    {
        winnerTextSB.text = winner;
    }

    public void PauseScreen(bool isPaused, int playerID)
    {
        statsScoreboard.SetActive(isPaused);
        pauseScreen.SetActive(isPaused);
        HideWhenPaused(!isPaused);
        if (isPaused)
        {
            if (AP != null) AP.PlaySoundRandomPitch(AP.Find("pauseWhistle"));
            GameObject.Find("WhoPaused").GetComponent<TMP_Text>().text = "Player " + (playerID + 1) + " Paused";
        } else
        {
            if (AP != null) AP.PlaySoundRandomPitch(AP.Find("pauseWhistle2"));
        }
    }

    private void HideWhenPaused(bool state)
    {
        hideWhenPaused.SetActive(state);
    }

    public void ShowMVP(bool state, int player)
    {
        if (player == 1) P1MVP.SetActive(state);
        if (player == 2) P2MVP.SetActive(state);
        if (player == 3) P3MVP.SetActive(state);
    }

    private void GoldGoals(List<int> player)
    {
        if (player.Contains(1)) Warrior1GoalsText.color = new Color(50, 255, 0);
        if (player.Contains(2)) Warrior2GoalsText.color = new Color(50, 255, 0);
        if (player.Contains(3)) Warrior3GoalsText.color = new Color(50, 255, 0);
    }

    private void GoldAssists(List<int> player)
    {
        if (player.Contains(1)) Warrior1AssistsText.color = new Color(50, 255, 0);
        if (player.Contains(2)) Warrior2AssistsText.color = new Color(50, 255, 0);
        if (player.Contains(3)) Warrior3AssistsText.color = new Color(50, 255, 0);
    }

    private void GoldDeaths(List<int> player)
    {
        if (player.Contains(1)) Warrior1DeathsText.color = new Color(50, 255, 0);
        if (player.Contains(2)) Warrior2DeathsText.color = new Color(50, 255, 0);
        if (player.Contains(3)) Warrior3DeathsText.color = new Color(50, 255, 0);
    }

    private void GoldSteals(List<int> player)
    {
        if (player.Contains(1)) Warrior1StealsText.color = new Color(50, 255, 0);
        if (player.Contains(2)) Warrior2StealsText.color = new Color(50, 255, 0);
        if (player.Contains(3)) Warrior3StealsText.color = new Color(50, 255, 0);
    }

    public void GoldenWarriorStats()
    {
        List<int> goalsL = new List<int>();
        List<int> goalsIndexL = new List<int>();

        List<int> assistsL = new List<int>();
        List<int> assistsIndexL = new List<int>();

        List<int> deathsL = new List<int>();
        List<int> deathsIndexL = new List<int>();

        List<int> stealsL = new List<int>();
        List<int> stealsIndexL = new List<int>();

        int player = -1;

        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");

        //Final Check
        foreach (GameObject warrior in warriors)
        {
            if (warrior.GetComponent<WarriorController>() != null) {
                player = warrior.GetComponent<WarriorController>().playerNum;
            }

            int currentGoals = ST.GetWGoals(player);
            int currentAssists = ST.GetWAssists(player);
            int currentDeaths = ST.GetWDeaths(player);
            int currentSteals = ST.GetWSteals(player);

            if ((currentGoals >= 0 && goalsL.Count == 0) || currentGoals >= goalsL[0])
            {
                if (goalsL.Count == 0 || goalsL.Contains(currentGoals))
                {
                    goalsL.Add(currentGoals);
                    goalsIndexL.Add(player);
                }
                else
                {
                    goalsL.Clear();
                    goalsIndexL.Clear();
                    goalsL.Add(currentGoals);
                    goalsIndexL.Add(player);
                }
            }

            if ((currentAssists >= 0 && assistsL.Count == 0) || currentAssists >= assistsL[0])
            {
                if (assistsL.Count == 0 || assistsL.Contains(currentAssists))
                {
                    assistsL.Add(currentAssists);
                    assistsIndexL.Add(player);
                }
                else
                {
                    assistsL.Clear();
                    assistsIndexL.Clear();
                    assistsL.Add(currentAssists);
                    assistsIndexL.Add(player);
                }
            }

            if ((deathsL.Count == 0) || currentDeaths <= deathsL[0])
            {
                if (deathsL.Count == 0 || deathsL.Contains(currentDeaths))
                {
                    deathsL.Add(currentDeaths);
                    deathsIndexL.Add(player);
                }
                else
                {
                    deathsL.Clear();
                    deathsIndexL.Clear();
                    deathsL.Add(currentDeaths);
                    deathsIndexL.Add(player);
                }
            }

            if ((currentSteals >= 0 && stealsL.Count == 0) || currentSteals >= stealsL[0])
            {
                if (stealsL.Count == 0 || stealsL.Contains(currentSteals))
                {
                    stealsL.Add(currentSteals);
                    stealsIndexL.Add(player);
                }
                else
                {
                    stealsL.Clear();
                    stealsIndexL.Clear();
                    stealsL.Add(currentSteals);
                    stealsIndexL.Add(player);
                }
            }

            //if (ST.GetWAssists(player) > assists)
            //{
            //    assists = ST.GetWAssists(player);
            //    assistsIndex = player;
            //}

            //if (ST.GetWDeaths(player) < deaths)
            //{
            //    deaths = ST.GetWDeaths(player);
            //    deathsIndex = player;
            //}

            //if (ST.GetWSteals(player) > steals)
            //{
            //    steals = ST.GetWSteals(player);
            //    stealsIndex = player;
            //}
        }
        GoldGoals(goalsIndexL);
        GoldAssists(assistsIndexL);
        GoldDeaths(deathsIndexL);
        GoldSteals(stealsIndexL);
    }

    private void UpdatePlayerLabels()
    {
        int count = 0;
        PlayerHolder PH = null;
        GameObject[] playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");

        foreach (GameObject holder in playerHolders)
        { 
            PH = holder.GetComponent<PlayerHolder>();
            if (PH != null)
            {
                if (PH.teamName.Equals("Monster"))
                {
                    MonsterLabel.text = "Player " + (PH.playerID + 1);
                    //playerIds[0] = PH.playerID;
                }
                else
                {
                    WarriorLabels[count].text = "Player " + (PH.playerID + 1);
                    //playerIds[count + 1] = PH.playerID;
                    count++;
                }
            } 
        }
    }

    /// <param name="isAI">If true, sets the portrait for an AI-controlled player; otherwise, for a human player.</param>
    /// <param name="WC">The <see cref="WarriorController"/> representing the warrior whose portrait should be set.</param>
    public void SetPlayerPortrait(bool isAI, WarriorController WC)
    {
        Sprite aliveSprite;
        Sprite deadSprite;

        if (isAI)
        {
            aliveSprite = robotSpriteAlive;
            deadSprite = robotSpriteDead;
        }
        else
        {
            aliveSprite = humanSpriteAlive;
            deadSprite = humanSpriteDead;
        }

        Color playerColor = WC.GetColor();

        Debug.Log("Set Player" + WC.playerNum + " icon to: " + playerColor);

        Image icon;
        Image dead;

        switch (WC.playerNum)
        {
            case 1:
                icon = player1Icon;
                dead = player1Dead;
                break;
            case 2:
                icon = player2Icon;
                dead = player2Dead;
                break;
            case 3:
                icon = player3Icon;
                dead = player3Dead;
                break;
            default:
                return;
        }

        icon.sprite = aliveSprite;
        dead.sprite = deadSprite;

        Material iconMat = new Material(icon.material);
        iconMat.color = playerColor;
        icon.material = iconMat;

        Material deadMat = new Material(dead.material);
        deadMat.color = playerColor;
        dead.material = deadMat;
    }

    public int GetWarriorScore()
    {
        return warriorScore;
    }

    public int GetMonsterScore()
    {
        return monsterScore;
    }
}
