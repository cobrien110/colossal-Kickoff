using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] sounds;
    public float volume = 1f;
    public bool isStageTheme = true;

    private float elapsedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        if (source != null)
        {
            source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
            // If this is a level, pause the music before the round starts
            if (isStageTheme) PauseMusic();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (source != null) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
    }

    public void PauseMusic()
    {
        elapsedTime = source.time;
        source.Pause();
    }

    public void UnPauseMusic()
    {
        if (source.time.Equals(0f))
        {
            source.Play();
            return;
        }
        source.UnPause();
    }
}
