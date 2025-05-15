using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Steamworks;

public class MonsterHolder : MonoBehaviour
{
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private GameplayManager GM;
    public int monsterIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnMonster(int playerID, Gamepad gamepad)
    {
        GM.AddPlayer(monsterPrefabs[monsterIndex], playerID, -1, gamepad);
        CheckMonsterAchievement(monsterIndex);
    }

    private void CheckMonsterAchievement(int monsterIndex)
    {
        switch (monsterIndex)
        {
            case 0:
                PlayerPrefs.SetInt("PlayedAsMonster0", 1);
                break;
            case 1:
                PlayerPrefs.SetInt("PlayedAsMonster1", 1);
                break;
            case 2:
                PlayerPrefs.SetInt("PlayedAsMonster2", 1);
                break;
            case 3:
                PlayerPrefs.SetInt("PlayedAsMonster3", 1);
                break;
            case 4:
                PlayerPrefs.SetInt("PlayedAsMonster4", 1);
                break;
        }

        if (PlayerPrefs.GetInt("PlayedAsMonster0") + PlayerPrefs.GetInt("PlayedAsMonster1")
                + PlayerPrefs.GetInt("PlayedAsMonster2") + PlayerPrefs.GetInt("PlayedAsMonster3")
                + PlayerPrefs.GetInt("PlayedAsMonster4") == 5)
        {
            // Steam Achievement Stuff
            if (SteamManager.Initialized)
            {
                Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("ALL_MONSTERS_PLAYED"));
                Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
            }
        }
    }
}
