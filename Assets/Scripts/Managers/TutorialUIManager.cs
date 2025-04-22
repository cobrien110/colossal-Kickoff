using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TutorialUIManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Text playerScoredText = null;
    
    [Header("ScoreboardUI")]
    [SerializeField] private GameObject statsScoreboard = null;
    private bool isSuperFlashing = false;
    
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
        
        ShowChargeBar(false);
        ShowMonsterUI(false);
        //ShowPlayerUI(false, 1);
        //ShowPlayerUI(false, 2);
        //ShowPlayerUI(false, 3);
        //ShowPassMeter(false);
        UpdateChargeBarText("");

        UpdatePlayerLabels();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPlayerScoredText(bool state)
    {
        playerScoredText.gameObject.SetActive(state);
    }

    public void UpdatePlayerScoredText(int player)
    {
        if (player == 0) playerScoredText.text = "CPU SCORED!";
        else if (player > 0) playerScoredText.text = "PLAYER " + (player) + " SCORED!";
        else playerScoredText.text = "PLAYER " + (player * -1) + " OWN GOALED!";
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

        if (warriorContestFill.fillAmount == 1)
        {
            ShowMiddleContestBar(true);

            if (!isSuperFlashing)
            {
                Debug.Log("STARTING SUPER KICK FLASH");
                isSuperFlashing = true;
                InvokeRepeating("SuperKickFlash", 1f, 0.5f);
            }
        }

        else if (middleContestFill.gameObject.activeInHierarchy && warriorContestFill.fillAmount < 1.0f)
        {
            ShowMiddleContestBar(false);
            Debug.Log("STOPPING SUPER KICK FLASH");
            CancelInvoke("SuperKickFlash");
            isSuperFlashing = false;
            RBgo.SetActive(true);
        }
    }

    private void SuperKickFlash()
    {
        RBgo.SetActive(!RBgo.activeInHierarchy);
    }

    public void UpdateMonsterContestBar(float charge, Color c)
    {
        monsterContestFill.fillAmount = charge;

        if (warriorContestFill.fillAmount == 1)
        {
            ShowMiddleContestBar(true);
        }

        else if (middleContestFill.gameObject.activeInHierarchy && warriorContestFill.fillAmount < 1.0f)
        {
            ShowMiddleContestBar(false);
        }

        if (!monsterContestFill.color.Equals(c))
        {
            monsterContestFill.color = c;
        }
    }

    public void ShowMiddleContestBar(bool state)
    {
        middleContestFill.gameObject.SetActive(state);
    }

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
        }
        else
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

        foreach (GameObject warrior in warriors)
        {
            if (warrior.GetComponent<WarriorController>() != null)
            {
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

    public void SetPlayerPortrait(bool isAI, int warriorPosition)
    {
        if (isAI)
        {
            switch (warriorPosition)
            {
                case 1:
                    player1Icon.sprite = robotSpriteAlive;
                    player1Dead.sprite = robotSpriteDead;
                    break;

                case 2:
                    player2Icon.sprite = robotSpriteAlive;
                    player2Dead.sprite = robotSpriteDead;
                    break;

                case 3:
                    player3Icon.sprite = robotSpriteAlive;
                    player3Dead.sprite = robotSpriteDead;
                    break;

                default:
                    break;
            }
        }
        else
        {
            switch (warriorPosition)
            {
                case 1:
                    player1Icon.sprite = humanSpriteAlive;
                    player1Dead.sprite = humanSpriteDead;
                    break;

                case 2:
                    player2Icon.sprite = humanSpriteAlive;
                    player2Dead.sprite = humanSpriteDead;
                    break;

                case 3:
                    player3Icon.sprite = humanSpriteAlive;
                    player3Dead.sprite = humanSpriteDead;
                    break;

                default:
                    break;
            }
        }
    }
}
