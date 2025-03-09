using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public Animator ANIM;
    public TextMeshProUGUI subtitlesText;
    public TextMeshProUGUI lineNumText;
    private int lineNum;
    public bool showLineNum;
    public GameObject subtitles;
    public String menuSceneName;
    public AudioPlayer sound;
    public Image fadeIn;
    public float fadeSpeed = 1f;
    bool isFadingIn = false;

    // sound management
    private float[] samples = new float[1024]; // Buffer for sample data
    private float silenceThreshold = 0.01f; // Adjust this threshold as needed

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
        subtitlesText.text = "";
        lineNumText.text = "";
        subtitlesText.color = Color.green;

        ANIM.SetBool("isTalking", false);

        //subtitlesText.text = "TEST TEST";
        StartCoroutine(StartSubtitles());
    }

    void Update()
    {
        // fade in and out
        if (isFadingIn)
        {
            float a = fadeIn.color.a + fadeSpeed * Time.deltaTime;
            a = Mathf.Clamp(a, 0, 1);
            fadeIn.color = new Color(0, 0, 0, a);
        } else
        {
            float a = fadeIn.color.a - fadeSpeed * Time.deltaTime;
            a = Mathf.Clamp(a, 0, 1);
            fadeIn.color = new Color(0, 0, 0, a);
        }

        // close mouth when not talking
        if (sound.isPlaying())
        {
            sound.source.clip.GetData(samples, sound.source.timeSamples);
            float maxAmplitude = GetMaxAmplitude(samples);

            bool willTalk = maxAmplitude > silenceThreshold ? true : false;
            ANIM.SetBool("isTalking", willTalk);
        }

            var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                SceneManager.LoadScene(menuSceneName);
            }
        }
    }

    IEnumerator StartSubtitles()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(SubtitlesCoroutine());
        sound.PlaySound(sound.Find("John Orcman finalV1"));
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
        if (eventNum == 0)
        {
            ANIM.SetBool("isTalking", true);
        }
        if (eventNum == 13)
        {
            //ANIM.Play("OrcmanSmash");
        }
        if (eventNum == 37)
        {
            isFadingIn = true;
        } 
    }

    private float GetMaxAmplitude(float[] data)
    {
        float max = 0f;
        foreach (float sample in data)
        {
            if (Mathf.Abs(sample) > max)
                max = Mathf.Abs(sample);
        }
        return max;
    }
}
