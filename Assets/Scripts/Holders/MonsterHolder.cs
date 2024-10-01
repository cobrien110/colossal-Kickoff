using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterHolder : MonoBehaviour
{
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private GameplayManager GM;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnMonster()
    {
        PlayerInput newInput = new PlayerInput();
        GM.AddPlayer(newInput, monsterPrefab);
    }
}
