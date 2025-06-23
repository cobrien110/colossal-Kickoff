using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorTutorialManager : MonoBehaviour
{
    bool initialized = false;

    [SerializeField] private GameplayManager GM;
    [SerializeField] private UIManager UM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            UM.SetTimeRemaining(181);
        }
    }

    public void Initiate()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();

        Debug.Log("WARRIOR TUTORIAL INITIALIZED");
        initialized = true;
    }
}
