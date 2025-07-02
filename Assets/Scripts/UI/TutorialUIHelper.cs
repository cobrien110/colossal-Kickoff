using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIHelper : MonoBehaviour
{
    [SerializeField] private TMP_Text[] objText;

    //Fade to black setup
    [SerializeField] private Image fadeToBlack;
    private float startAlpha = 0f;
    private float endAlpha = .7f;
    private Color fadeCol = new Color(0, 0, 0);
    private bool fadeStart = false;
    private bool fadeEnd = false;

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
        if (fadeStart) FadeToBlack();
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

    public void FadeToBlack()
    {
        startAlpha = Mathf.MoveTowards(startAlpha, endAlpha, Time.deltaTime);
        fadeCol.a = startAlpha;
        fadeToBlack.color = fadeCol;

        if (startAlpha >= endAlpha)
        {
            startAlpha = endAlpha;
            Time.timeScale = 0f;
            fadeStart = false;
            fadeEnd = true;
        }
    }

    public void FadeStart(bool input)
    {
        fadeStart = input;
    }

    public void ResumeGame()
    {
        fadeCol.a = 0f;
        fadeToBlack.color = fadeCol;
        fadeEnd = false;
    }

    public bool GetFadeEnd()
    {
        return fadeEnd;
    }


}
