using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Random = UnityEngine.Random;

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
    public MusicPlayer musicPlayer;
    public Image fadeIn;
    public float fadeSpeed = 1f;
    bool isFadingIn = false;
    public AsyncLoadManager ALM;
    public Animator newsImages;
    public Animator newsImagesBox;
    public float delayForTechDif = 5f;
    public Image standbySprite;
    public Image staticSprite;
    public AudioPlayer staticSounds;
    private float staticBaseVolume;
    private bool isRandomizingStatic = true;

    // sound management
    private float[] samples = new float[1024]; // Buffer for sample data
    private float silenceThreshold = 0.005f; // Adjust this threshold as needed

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
        ALM.BeginLoad(menuSceneName);
        newsImages.speed = 0f;
        newsImagesBox.speed = 0f;
        //STATIC SOUND PLAYER
        staticBaseVolume = staticSounds.volume;
        
        //ORCMAN FIGHT SOUND PLAYER

        //subtitlesText.text = "TEST TEST";
        StartCoroutine(StartSubtitles());
        StartCoroutine(RandomizeStatic());
    }

    private void Awake()
    {
        staticSounds.PlaySound(staticSounds.Find("tvstatic"));
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
                ALM.LoadScene(menuSceneName);
            }
        }
    }

    IEnumerator StartSubtitles()
    {
        yield return new WaitForSeconds(delayForTechDif);
        StartCoroutine(SubtitlesCoroutine());
        newsImages.speed = 1f;
        newsImagesBox.speed = 1f;
        musicPlayer.source.Play();
        standbySprite.enabled = false;
        staticSounds.source.Stop();
        
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
        ALM.LoadScene(menuSceneName);
    }

    void DoEvent(int eventNum)
    {
        if (eventNum == 0)
        {
            ANIM.SetBool("isTalking", true);
        }
        if (eventNum == 1) {
            staticSprite.enabled = false;
            isRandomizingStatic = false;
        }
        if (eventNum == 16 || eventNum == 29 || eventNum == 30 || eventNum == 31 || eventNum == 35 || eventNum == 37)
        {
            ANIM.Play("OrcmanSmash");
        }
        if (eventNum == 37)
        {
            isFadingIn = true;
            newsImages.speed = 0f;
            newsImagesBox.speed = 0f;
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

    private IEnumerator RandomizeStatic()
    {
        while(isRandomizingStatic)
        {
            float r = Random.Range(0, 1f);
            if (r < .9)
            {
                staticSprite.enabled = true;
                //staticSounds.source.volume = staticBaseVolume * PlayerPrefs.GetFloat("effectsVolume");
            }
            else
            {
                staticSprite.enabled = false;
                //staticSounds.source.volume = 0;
            }
            staticSprite.transform.localScale = new Vector3(staticSprite.transform.localScale.x * -1, staticSprite.transform.localScale.y);

            float rx = Random.Range(0, 1f);
            if (rx < .5)
            {
                
            }

            float ry = Random.Range(0, 1f);
            if (ry < .5)
            {
                staticSprite.transform.localScale = new Vector3(staticSprite.transform.localScale.x, staticSprite.transform.localScale.y * -1);
            }

            yield return new WaitForSeconds(0.05f);
        }
        
    }
}
