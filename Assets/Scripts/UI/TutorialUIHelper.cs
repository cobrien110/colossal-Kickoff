using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIHelper : MonoBehaviour
{
    [SerializeField] private TMP_Text[] objText;
    [SerializeField] private GameObject[] subsets;
    [SerializeField] private GameObject allSubsets;
    [SerializeField] private TMP_Text mainMessage;
    private string[] mainMessages = new string[]
    {
        "Welcome to Colossal Kickoff!\n\nHere, you will be learning how to play as a warrior. We will begin with basic movement and kicking.",
        "Great job!\n\nNext, kick the ball towards the goals to score. When playing, the team with the most points wins!  Goals will also have barriers, which need to be hit by the ball a few times to break down. Give it a try!",
        "The warriors have a few ways to fight back against the monsters. Each warrior can slide to steal the ball and dodge incoming attacks.\n\nWarriors can also pass to each other to charge their SUPER KICK METER, which allows you kick with much more power. This powerful kick stuns monsters and breaks down goal barriers with ease!\n\nYou can also press (A) while your teammate is dribbling to call for a pass.",
        "Congratulations!\n\nYou are now ready to take on the monsters. See you on the pitch!"
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
        startAlpha = Mathf.MoveTowards(startAlpha, endAlpha, (1.2f* Time.deltaTime));
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
