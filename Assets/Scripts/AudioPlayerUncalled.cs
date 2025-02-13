using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioPlayerUncalled : MonoBehaviour
{
    private AudioSource source;
    private float volume;
    // Start is called before the first frame update
    void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        volume = source.volume;
        if (source != null) source.volume = volume * PlayerPrefs.GetFloat("effectsVolume", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
