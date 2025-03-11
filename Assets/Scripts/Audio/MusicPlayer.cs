using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;
    [SerializeField] private MusicPlayerResults MPR;
    [SerializeField] private MusicPlayerOvertime MPO;

    //public AudioClip[] sounds;
    public float volume = 1f;
    public bool isStageTheme = true;
    public bool isResultsTheme = false;
    //public bool isCutsceneTheme = false;
    //public bool isPersistent = false;
    [SerializeField] private float timeDEBUG;
    [SerializeField] private float timeDEBUG2;

    protected float elapsedTime = 0f;
    public bool testMusic = false;
    protected float curVol = 0f;
    protected float volGainRate = 1.5f;
    protected bool isPaused = false;
    protected bool isFadingOut = false;
    protected bool overtimeSwitch = false;
    private UIManager UI;
    protected GameplayManager GM;
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
            }
        }
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (isStageTheme) GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();

        if (MPR == null)
        {
            MPR = GetComponentInChildren<MusicPlayerResults>();
        }
        if (MPO == null)
        {
            MPO = GetComponentInChildren<MusicPlayerOvertime>();
        }
        //if (isPersistent && isCutsceneTheme) DontDestroyOnLoad(this.gameObject);
    }

    /*
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
    */

    // Update is called once per frame
    void Update()
    {
        if (testMusic) source.volume = 1f;
        if (source == null) return;
        else if (!isStageTheme) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
        else if (isFadingOut)
        {
            curVol -= Time.deltaTime * volGainRate / 2f;
            Mathf.Clamp(curVol, 0f, volume);
            source.volume = curVol * PlayerPrefs.GetFloat("musicVolume", 1);
            if (curVol <= 0 && !MPR.source.isPlaying && GM != null && GM.isGameOver)
            {
                //isFadingOut = false;
                MPR.UnPauseMusic();
            }
        }
        else if (curVol < volume && isStageTheme)
        {
            if (!isPaused) curVol += Time.deltaTime * volGainRate;
            Mathf.Clamp(curVol, 0f, volume);
            source.volume = curVol * PlayerPrefs.GetFloat("musicVolume", 1);
        }

        if (UI != null)
        {
            //timeDEBUG2 = UI.GetTimeRemaining();
            //timeDEBUG = 180 - elapsedTime;
        }
    }

    public void PauseMusic()
    {
        Debug.Log("MP Pausing music");
        elapsedTime = Mathf.Floor(source.time);
        source.time = elapsedTime;
        curVol = 0f;
        source.Pause();
        isPaused = true;
    }

    public void PauseMusicNoFloor()
    {
        Debug.Log("MP Pausing music");
        elapsedTime = source.time;
        curVol = 0f;
        source.Pause();
        isPaused = true;
    }

    public void UnPauseMusic()
    {
        //float reverseTime = elapsedTime - 3f;
        Debug.Log("MP Resuming music at:" + elapsedTime);

        source.UnPause();
        isPaused = false;
    }

    public void PlayResults()
    {
        Debug.Log("MP switching to results music");
        isFadingOut = true;
        if (MPR != null) MPR.UnPauseMusic();
    }

    public void SwitchToOvertime()
    {
        if (MPO == null) return;
        Debug.Log("MP switching to overtime music");
        //PauseMusicNoFloor();
        isFadingOut = true;
        overtimeSwitch = true;
        MPO.SetJukeboxGM();
    }
}
