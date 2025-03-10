using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scoreboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonRematch"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
