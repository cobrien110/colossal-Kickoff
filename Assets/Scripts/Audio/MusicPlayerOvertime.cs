using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerOvertime : MusicPlayer
{
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

        GM = GameObject.FindObjectOfType<GameplayManager>();
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
        //if (source != null) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
        if (GM != null && !GM.overtimeStarted && source.isPlaying)
        {
            curVol -= Time.deltaTime * volGainRate / 2f;
            Mathf.Clamp(curVol, 0f, volume);
            source.volume = curVol * PlayerPrefs.GetFloat("musicVolume", 1);
        }
        else if (curVol < volume && source != null)
        {
            if (!isPaused) curVol += Time.deltaTime * volGainRate;
            Mathf.Clamp(curVol, 0f, volume);
            source.volume = curVol * PlayerPrefs.GetFloat("musicVolume", 1);
        }
    }

    public void SetJukeboxGM()
    {
        Debug.Log("MPO setting GM Jukebox");
        if (GM != null)
        {
            GM.SetMP(this);
        }
    }
}
