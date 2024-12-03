using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorHolder : MonoBehaviour
{
    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameplayManager GM;
    [SerializeField] private UIManager UM;
    private int warriorCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        UM.ShowPassMeter(true);
        for (int i = 1; i < warriorCount; i++)
        {
            UM.ShowPlayerUI(true, i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawnWarrior(int playerID, Gamepad gamepad, Color color)
    {
        //Debug.Log(warriorCount);
        warriorCount++;
        warriorPrefab.GetComponentInChildren<SpriteRenderer>().sharedMaterial.color = color;
        GM.AddPlayer(warriorPrefab, playerID, gamepad);
    }
}
