using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class Stats
{
    //Monster Stats
    public int kills;
    public int mGoals;
    public int abilities;
    public int mWins;

    //Warrior Stats
    public int wGoals;
    public int assists;
    public int deaths;
    public int steals;
    public int wWins;
}

public class StatTracker : MonoBehaviour
{
    public Stats saveData = new Stats();

    // Warriors:
    private int w1Goals = 0;
    private int w2Goals = 0;
    private int w3Goals = 0;

    private int w1Deaths = 0;
    private int w2Deaths = 0;
    private int w3Deaths = 0;

    private int w1Steals = 0;
    private int w2Steals = 0;
    private int w3Steals = 0;

    private int w1Assists = 0;
    private int w2Assists = 0;
    private int w3Assists = 0;

    // Monster:
    private int mGoals = 0;
    private int mKills = 0;
    private int mAbUsed = 0;

    // Dev:
    private float ballTimeMonster = 0;
    private float ballTimeWarrior = 0;
    private string gameWinner = "";

    UnityEvent dataLoad;
    public bool dataLoaded = false;

    //MVP Stats
    [Header("MVP Values")]
    [SerializeField] private int goalValue;
    [SerializeField] private int assistValue;
    [SerializeField] private int stealValue;
    [SerializeField] private int deathValue;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainMenus"))
        {
            MenuController menuController = null;
            menuController = GameObject.Find("MenuController").GetComponent<MenuController>();
            dataLoad = new UnityEvent();
            dataLoad.AddListener(menuController.UpdateStatsText);
        }

        LoadFromJson();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //MVP Score Totaling
    public int GetMVP()
    {
        int p1Score;
        int p2Score;
        int p3Score;

        p1Score = (w1Goals * goalValue) + (w1Assists * assistValue) + (w1Steals * stealValue) - (w1Deaths * deathValue);
        p2Score = (w2Goals * goalValue) + (w2Assists * assistValue) + (w2Steals * stealValue) - (w2Deaths * deathValue);
        p3Score = (w3Goals * goalValue) + (w3Assists * assistValue) + (w3Steals * stealValue) - (w3Deaths * deathValue);

        Debug.Log("" + p1Score + " " + p2Score + " " + p3Score);

        if (p1Score > p2Score && p1Score > p3Score) return 1;
        if (p2Score > p1Score && p2Score > p3Score) return 2;
        if (p3Score > p1Score && p3Score > p2Score) return 3;

        if (w1Goals > w2Goals && w1Goals > w3Goals) return 1;
        if (w2Goals > w1Goals && w2Goals > w3Goals) return 2;
        if (w3Goals > w1Goals && w3Goals > w2Goals) return 3;

        else return -1;
    }

    public void UpdateGameWinner(int winner)
    {
        if (winner == 0) gameWinner = "WARRIORS WIN!";
        else if (winner == 1) gameWinner = "MONSTER WINS!";
        else gameWinner = "TIE GAME!";
    }

    public string GetGameWinner() {  return gameWinner; }

    public void UpdateWGoals(int player)
    {
        if (player == 1) w1Goals++;
        if (player == 2) w2Goals++;
        if (player == 3) w3Goals++;
    }

    public int GetWGoals(int player)
    {
        if (player == 1) return w1Goals;
        if (player == 2) return w2Goals;
        if (player == 3) return w3Goals;
        return -1;
    }

    public void UpdateWAssists(int player)
    {
        if (player == 1) w1Assists++;
        if (player == 2) w2Assists++;
        if (player == 3) w3Assists++;
    }

    public int GetWAssists(int player)
    {
        if (player == 1) return w1Assists;
        if (player == 2) return w2Assists;
        if (player == 3) return w3Assists;
        return -1;
    }

    public void UpdateWDeaths(int player)
    {
        //Debug.Log("PLAYER THAT DIED: " + player);
        if (player == 1) w1Deaths++;
        if (player == 2) w2Deaths++;
        if (player == 3) w3Deaths++;
    }

    public int GetWDeaths(int player)
    {
        if (player == 1) return w1Deaths;
        if (player == 2) return w2Deaths;
        if (player == 3) return w3Deaths;
        return -1;
    }

    public void UpdateWSteals(int player)
    {
        if (player == 1) w1Steals++;
        if (player == 2) w2Steals++;
        if (player == 3) w3Steals++;
    }

    public int GetWSteals(int player)
    {
        if (player == 1) return w1Steals;
        if (player == 2) return w2Steals;
        if (player == 3) return w3Steals;
        return -1;
    }

    public void UpdateWSaves()
    {
        //wSaves++;
    }

    public void UpdateMGoals()
    {
        mGoals++;
    }

    public int GetMGoals()
    {
        return mGoals;
    }

    public void UpdateMAbUsed()
    {
        mAbUsed++;
    }

    public int GetMAbUsed()
    {
        return mAbUsed;
    }

    public void UpdateMKills()
    {
        mKills++;
    }

    public int GetMKills()
    {
        return mKills;
    }

    public void UpdateMSaves()
    {
        //mSaves++;
    }

    public void UpdateMWins()
    {
        
    }
    
    public void SaveToJson()
    {
        string data = JsonUtility.ToJson(saveData);
        string filePath = Application.persistentDataPath + "/SaveData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, data);
        Debug.Log("Data Saved");
    }

    public void LoadFromJson()
    {
        try
        {
            string filePath = Application.persistentDataPath + "/SaveData.json";
            string data = System.IO.File.ReadAllText(filePath);

            saveData = JsonUtility.FromJson<Stats>(data);
            dataLoad.Invoke();
            dataLoaded = true;
            Debug.Log("Data Loaded");
        }
        catch
        {
            Debug.Log("FILE COULD NOT BE FOUND");
        }
    }

    public void UpdateSaveData()
    {
        //Monster Stats
        saveData.kills += mKills;
        saveData.mGoals += mGoals;
        saveData.abilities += mAbUsed;
        if (GetGameWinner().Equals("WARRIORS WIN!")) saveData.mWins += 1;

        //Warrior Stats
        saveData.wGoals += (w1Goals + w2Goals + w3Goals);
        saveData.assists += (w1Assists + w2Assists + w3Assists);
        saveData.deaths += (w1Deaths + w2Deaths + w3Deaths);
        saveData.steals += (w1Steals + w2Steals + w3Steals);
        if (!GetGameWinner().Equals("WARRIORS WIN!")) saveData.wWins += 1;
    }

    public void ResetSaveData()
    {
        saveData.kills = 0;
        saveData.mGoals = 0;
        saveData.abilities = 0;
        saveData.mWins = 0;

        saveData.wGoals = 0;
        saveData.assists = 0;
        saveData.deaths = 0;
        saveData.steals = 0;
        saveData.wWins = 0;

        SaveToJson();
    }

}
