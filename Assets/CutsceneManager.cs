using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public Sprite SPR;
    public TextMeshProUGUI subtitlesText;
    public TextMeshProUGUI lineNumText;
    private int lineNum;
    public bool showLineNum;
    public GameObject subtitles;
    public String menuSceneName;

    [Serializable]
    public struct VoiceLine
    {
        public string text;
        public float time;
    }

    public VoiceLine[] voiceLines;

    // Start is called before the first frame update
    void Start()
    {
        //subtitlesText.text = "TEST TEST";
        StartSubtitles();
    }

    void StartSubtitles()
    {
        StartCoroutine(SubtitlesCoroutine());
    }

    IEnumerator SubtitlesCoroutine()
    {
        subtitles.SetActive(true);

        foreach (var line in voiceLines)
        {
            DoEvent(lineNum);
            subtitlesText.text = line.text;

            if (showLineNum)
            {
                lineNumText.text = lineNum.ToString();
            } else
            {
                lineNumText.text = "";
            }
            lineNum++;

            yield return new WaitForSecondsRealtime(line.time);
        }

        subtitles.SetActive(false);
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(menuSceneName);
    }

    void DoEvent(int eventNum)
    {
        if (eventNum == 3)
        {
            subtitlesText.color = Color.red;
        } else
        {
            subtitlesText.color = Color.green;
        }
    }
}
