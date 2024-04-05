using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource source;
    public AudioClip[] sounds;
    public float volume = 1f;
    public float pitchRangeLow = .85f;
    public float pitchRangeHigh = 1.15f;

    private void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!source.isPlaying)
        {
            source.volume = volume;
            source.pitch = 1;
        }
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null) return;
        source.clip = sound;
        source.Play();
        //Debug.Log("Playing sound: " + sound.name);
    }

    public void PlaySoundRandomPitch(AudioClip sound)
    {
        SetRandomPitch();
        PlaySound(sound);
    }

    public void PlaySoundSpecificPitch(AudioClip sound, float pitch)
    {
        source.pitch = pitch;
        PlaySound(sound);
    }

    public void PlaySoundVolume(AudioClip sound, float tempVolume)
    {
        source.volume = volume * tempVolume;
        Debug.Log(source.volume);
        PlaySound(sound);
    }

    public void PlaySoundVolumeRandomPitch(AudioClip sound, float tempVolume)
    {
        SetRandomPitch();
        PlaySoundVolume(sound, tempVolume);
    }

    public AudioClip Find(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == soundName)
            {
                return sounds[i];
            }
        }
        return null;
    }

    private void SetRandomPitch()
    {
        source.pitch = Random.Range(pitchRangeLow, pitchRangeHigh);
    }

}
