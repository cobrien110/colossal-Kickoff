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


        if (SceneManager.GetActiveScene().name.Equals("MainMenus"))
        {
            GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor();
        }
        else
        {
            GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>().spawnMonster();
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
        Debug.Log("Entered Scene Loaded");
        if (scene.name.Equals("MainMenus"))
        {
            GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor();
        }
        else
        {
            GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>().spawnMonster();
        }
    }
}
