using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] sounds;
    public float volume = 1f;
    public bool isStageTheme = true;
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
        
        if (Time.timeScale == 0 && source.isPlaying)
        {
            PauseMusicNoFloor();
        } 
        if (Time.timeScale == 1 && !source.isPlaying)
        {
            UnPauseMusic();
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

    private void PauseMusicNoFloor()
    {
        Debug.Log("Pausing music");
        //elapsedTime = Mathf.Floor(source.time);
        source.time = elapsedTime;
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
