using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHolder : MonoBehaviour
{
    [SerializeField] private int playerID;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        playerID = GameObject.FindGameObjectsWithTag("PlayerHolder").Length - 1;

        if (SceneManager.GetActiveScene().name.Equals("MainMenus"))
        {
            GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor(playerID);
        }
        else
        {
            if (playerID == 0)
            {
                GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>().spawnMonster(playerID);
            } else
            {
                GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID);

            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("MainMenus"))
        {
            GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor(playerID);
        }
        else
        {
            if (playerID == 0)
            {
                GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>().spawnMonster(playerID);
            }
            else
            {
                GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID);

            }
        }
    }
}