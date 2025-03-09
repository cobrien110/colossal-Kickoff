using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerResults : MonoBehaviour
{
    public AudioSource source;
    
    //public AudioClip[] sounds;
    public float volume = 1f;

    private float curVol = 0f;
    private float volGainRate = 1.5f;
    private bool isPaused = false;
    //private GameplayManager GM;
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

            source.Play();
            PauseMusic();

        }

        //GM = GameObject.FindObjectOfType<GameplayManager>();
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
        if (source != null) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
        else if (curVol < volume && source != null)
        {
            if (!isPaused) curVol += Time.deltaTime * volGainRate;
            Mathf.Clamp(curVol, 0f, volume);
            source.volume = curVol * PlayerPrefs.GetFloat("musicVolume", 1);
        }
    }

    public void PauseMusic()
    {
        Debug.Log("Pausing music");
        //elapsedTime = Mathf.Floor(source.time);
        //source.time = elapsedTime;
        curVol = 0f;
        source.Pause();
        isPaused = true;
    }

    public void PauseMusicNoFloor()
    {
        Debug.Log("Pausing music");
        //elapsedTime = source.time;
        curVol = 0f;
        source.Pause();
        isPaused = true;
    }

    public void UnPauseMusic()
    {
        //float reverseTime = elapsedTime - 3f;
        Debug.Log("Resuming music at:" + 0f);

        source.UnPause();
        isPaused = false;
    }

    /*
    public bool GetGameOver()
    {
        if (GM != null)
        {
            return GM.isGameOver;
        }
        return false;
    }
    */
}
