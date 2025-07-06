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

    public List<Color> warriorColors;
    public List<Color> skinColors;
    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        for (int i = 1; i < warriorCount; i++)
        {
            //UM.ShowPlayerUI(true, i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawnWarrior(int playerID, int warriorPosition, Gamepad gamepad, Color color, Color skinColor)
    {
        //Debug.Log(warriorCount);
        warriorCount++;

        //warriorPrefab.GetComponentInChildren<SpriteRenderer>().sharedMaterial.color = color;

        //MaterialPropertyBlock MPB = new MaterialPropertyBlock();
        //MPB.SetColor("_Color", color);
        //warriorPrefab.GetComponentInChildren<SpriteRenderer>().SetPropertyBlock(MPB);

        warriorColors.Add(color);
        skinColors.Add(skinColor);

        GM.AddPlayer(warriorPrefab, playerID, warriorPosition, gamepad);
    }
}
