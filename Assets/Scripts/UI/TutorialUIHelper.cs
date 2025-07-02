using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialUIHelper : MonoBehaviour
{
    [SerializeField] private TMP_Text[] objText;

    // Start is called before the first frame update
    void Start()
    {
        objText[0].color = Color.yellow;
        objText[1].alpha = 0.5f;
        objText[2].alpha = 0.5f;
        objText[3].alpha = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveObjective(int obj)
    {
        int objIndex = obj - 1;

        for (int i = 0; i <= objIndex; i++)
        {
            if (i < objIndex)
            {
                objText[i].color = Color.green;
                objText[i].alpha = 0.5f;
            }
            else if (i == objIndex)
            {
                if (objIndex >= objText.Length) return;
                objText[i].color = Color.yellow;
                objText[i].alpha = 1f;
            }
        }
    }
}
