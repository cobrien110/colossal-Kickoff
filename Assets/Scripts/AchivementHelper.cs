using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;

public class AchivementHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CheckStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckStage()
    {
        // checks for globetrotter achievement
        string currentStageName = SceneManager.GetActiveScene().name;
        switch(currentStageName)
        {
            case "GreeceArena":
                PlayerPrefs.SetInt("playedStage1", 1);
                break;

            case "CanadaArena":
                PlayerPrefs.SetInt("playedStage2", 1);
                break;

            case "JapanArena":
                PlayerPrefs.SetInt("playedStage3", 1);
                break;

            case "MexicoStage":
                PlayerPrefs.SetInt("playedStage4", 1);
                break;

            case "EgyptArena":
                PlayerPrefs.SetInt("playedStage5", 1);
                break;

            default:
                // do nothing
                break;
        }

        if (PlayerPrefs.GetInt("playedStage1") + PlayerPrefs.GetInt("playedStage2")
                + PlayerPrefs.GetInt("playedStage3") + PlayerPrefs.GetInt("playedStage4")
                + PlayerPrefs.GetInt("playedStage5") == 5)
        {
            // Steam Achievement Stuff
            if (SteamManager.Initialized)
            {
                Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("ALL_STAGES_PLAYED"));
                Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
            }
        }
    }
}
