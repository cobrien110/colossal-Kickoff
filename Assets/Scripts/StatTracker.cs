using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class StatTracker : MonoBehaviour
{

    // Warriors:
    //Active:
    private int w1Goals = 0;
    private int w2Goals = 0;
    private int w3Goals = 0;

    private int w1Deaths = 0;
    private int w2Deaths = 0;
    private int w3Deaths = 0;
    //Inactive:
    private int wAssists = 0;
    private int wSteals = 0;
    private int wSaves = 0;

    // Monster:
    //Active:
    private int mGoals = 0;
    private int mAbUsed = 0;
    private int mKills = 0;
    //Inactive:
    private int mSaves = 0;

    // Dev:
    private float ballTimeMonster = 0;
    private float ballTimeWarrior = 0;
    private string gameWinner = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void UpdateWAssists()
    {
        wAssists++;
    }

    public void UpdateWDeaths(int player)
    {
        Debug.Log("PLAYER THAT DIED: " + player);
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

    public void UpdateWSteals()
    {
        wSteals++;
    }

    public void UpdateWSaves()
    {
        wSaves++;
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
        mSaves++;
    }
   
}
