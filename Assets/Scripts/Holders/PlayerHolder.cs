using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHolder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (SceneManager.GetActiveScene().name.Equals("MainMenus"))
        {
            GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor();
        } else
        {
            GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>().spawnMonster();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
