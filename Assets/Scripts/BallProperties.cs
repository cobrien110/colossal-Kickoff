using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProperties : MonoBehaviour
{
    [HideInInspector] public GameObject ballOwner = null;
    private UIManager UM = null;
    private GameplayManager GM = null;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Warrior"))
        {
            ballOwner = other.gameObject;
        }

        if (other.tag.Equals("WarriorGoal"))
        {
            UM.monsterPoint();
            GM.Reset();
            Destroy(this.gameObject);
        }

        if (other.tag.Equals("MonsterGoal"))
        {
            UM.warriorPoint();
            GM.Reset();
            Destroy(this.gameObject);
        }
    }
}
