using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorHolder : MonoBehaviour
{
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameplayManager GM;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawnWarrior(int playerID)
    {
        GM.AddPlayer(warriorPrefab, playerID);
    }
}
