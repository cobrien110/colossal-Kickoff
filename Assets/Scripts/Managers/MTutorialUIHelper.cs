using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MTutorialUIHelper : MonoBehaviour
{
    [SerializeField] private TMP_Text[] objText;
    [SerializeField] private GameObject[] subsets;
    [SerializeField] private GameObject allSubsets;
    [SerializeField] private TMP_Text mainMessage;
    private string[] mainMessages = new string[]
    {
        "Welcome to Colossal Kickoff!\n\nHere, you will be learning how to play as a monster. We will begin with basic movement and kicking.",
        "Great job!\n\nNext, kick the ball towards the goals to score. When playing, the team with the most points wins!  Goals will also have barriers, which need to be hit by the ball a few times to break down. Give it a try!",
        "Each monster has a few abilities they can use to crush the warriors. Monsters also have a basic attack and a passive.\n\nThe minotaur’s main abilities are Labyrinth Walls and Bull Rush. Walls can be used to block incoming shots, and Bull Rush is a great way to close down space.\n\nSome abilities, such as your basic attack, cannot be used while dribbling. Give it a try!",
        "Congratulations!\n\nYou are now ready to take on the warriors. See you on the pitch!"
    };
    private int mainMessageIndex = 0;

    //Fade to black setup
    [SerializeField] private Image fadeToBlack;
    private float startAlpha = 0f;
    private float endAlpha = .7f;
    private Color fadeCol = new Color(0, 0, 0);
    private bool fadeStart = false;
    private bool fadeEnd = false;
    //private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeStart = true;
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
        startAlpha = Mathf.MoveTowards(startAlpha, endAlpha, (1.2f * Time.deltaTime));
        fadeCol.a = startAlpha;
        fadeToBlack.color = fadeCol;

        if (startAlpha >= endAlpha)
        {
            startAlpha = endAlpha;
            mainMessage.text = mainMessages[mainMessageIndex];
            mainMessage.gameObject.SetActive(true);
            Time.timeScale = 0f;
            fadeStart = false;
            fadeEnd = true;
        }
    }

    public void FadeStart()
    {
        UpdateSupsetHolder();
        fadeStart = true;
    }

    public void ResumeGame()
    {
        startAlpha = 0f;
        fadeCol.a = 0f;
        fadeToBlack.color = fadeCol;
        fadeEnd = false;

        mainMessage.gameObject.SetActive(false);
        mainMessageIndex++;
        UpdateSupsetHolder();
    }

    public bool GetFadeEnd()
    {
        return fadeEnd;
    }

    public void ShowSubset(int index)
    {
        for (int i = 0; i < subsets.Length; i++)
        {
            if (i == index) subsets[i].SetActive(true);
            else subsets[i].SetActive(false);
        }
    }

    public void UpdateSupsetHolder()
    {
        allSubsets.SetActive(!allSubsets.activeInHierarchy);
    }

    public void DelayedFade()
    {
        Invoke("FadeStart", 3.05f);
    }
}
