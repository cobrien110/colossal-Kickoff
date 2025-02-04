using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] sounds;
    public float volume = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        if (source != null) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (source != null) source.volume = volume * PlayerPrefs.GetFloat("musicVolume", 1);
    }
}
