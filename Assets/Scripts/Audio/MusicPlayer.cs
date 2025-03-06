using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] sounds;
    public float volume = 1f;
    public bool isStageTheme = true;
    public bool isPersistent = false;
    public float timeDEBUG;
    public float timeDEBUG2;

    private float elapsedTime = 0f;
    public bool testMusic = false;
    private float curVol = 0f;
    private float volGainRate = 1.5f;
    private bool isPaused = false;
    private UIManager UI;
    // Start is called before the first frame update
    void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        if (source != null)
        {
            curVol = source.volume;
            source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
            if (testMusic) source.volume = 1f;
            // If this is a level, pause the music before the round starts
            if (isStageTheme) {
                source.Play();
                PauseMusic();
            };
        }
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (isPersistent) DontDestroyOnLoad(this.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != 1)
        {
            gameObject.SetActive(false);
        } else
        {
            gameObject.SetActive(true);
            source.time = 0f;
            UnPauseMusic();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (testMusic) source.volume = 1f;
        else if (source != null && !isStageTheme) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
        else if (curVol < volume && source != null && isStageTheme)
        {
            if (!isPaused) curVol += Time.deltaTime * volGainRate;
            Mathf.Clamp(curVol, 0f, volume);
            source.volume = curVol * PlayerPrefs.GetFloat("musicVolume", 1);
        }
        timeDEBUG = source.time;

        if (UI != null)
        {
            timeDEBUG2 = UI.GetTimeRemaining();
        }
    }

    public void PauseMusic()
    {
        Debug.Log("Pausing music");
        elapsedTime = Mathf.Floor(source.time);
        source.time = elapsedTime;
        curVol = 0f;
        source.Pause();
        isPaused = true;
    }

    public void PauseMusicNoFloor()
    {
        Debug.Log("Pausing music");
        elapsedTime = source.time;
        curVol = 0f;
        source.Pause();
        isPaused = true;
    }

    public void UnPauseMusic()
    {
        //float reverseTime = elapsedTime - 3f;
        Debug.Log("Resuming music at:" + elapsedTime);

        source.UnPause();
        isPaused = false;
    }
}
