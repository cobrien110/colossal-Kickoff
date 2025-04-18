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
    private bool useComVol = false;
    private void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
        if (source != null && !useComVol) source.volume = volume * PlayerPrefs.GetFloat("effectsVolume", 1);
        else if (source != null) source.volume = volume * PlayerPrefs.GetFloat("commentatorVolume", 1);
    }

    private void Update()
    {
        if (!isPlaying())
        {
            if (source == null) return;
            source.volume = volume;
            source.pitch = 1;
            if (useComVol) useComVol = false;
        }
    }

    public void PlaySound(AudioClip sound)
    {
        if (source == null) return;
        if (sound == null) return;
        source.clip = sound;
        //Debug.Log("SFX PREFS: " + PlayerPrefs.GetFloat("effectsVolume", 1));
        if (!useComVol) source.volume = volume * PlayerPrefs.GetFloat("effectsVolume", 1);
        else source.volume = volume * PlayerPrefs.GetFloat("commentaryVolume", 1);
        source.Play();
        //Debug.Log("Playing sound: " + sound.name);
    }

    public void PlaySoundRandomPitch(AudioClip sound)
    {
        if (source == null) return;
        SetRandomPitch();
        PlaySound(sound);
    }

    public void PlaySoundSpecificPitch(AudioClip sound, float pitch)
    {
        if (source == null) return;
        source.pitch = pitch;
        PlaySound(sound);
    }

    public void PlaySoundVolume(AudioClip sound, float tempVolume)
    {
        if (sound == null) return;
        if (source == null) return;
        source.clip = sound;
        if (!useComVol) source.volume = volume * tempVolume * PlayerPrefs.GetFloat("effectsVolume", 1);
        else source.volume = volume * tempVolume * PlayerPrefs.GetFloat("commentaryVolume", 1);
        source.Play();
        //Debug.Log(source.volume);
        //PlaySound(sound);
    }

    public void PlaySoundVolumeRandomPitch(AudioClip sound, float tempVolume)
    {
        if (source == null) return;
        SetRandomPitch();
        PlaySoundVolume(sound, tempVolume);
    }

    public void PlaySoundRandom()
    {
        if (source == null) return;
        int clipIndex = Random.Range(0, sounds.Length - 1);
        PlaySoundRandomPitch(sounds[clipIndex]);
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
        if (source != null) source.pitch = Random.Range(pitchRangeLow, pitchRangeHigh);
    }

    public bool isPlaying()
    {
        if (source == null) return false;
        return source.isPlaying;
    }

    public void setUseComVol(bool b)
    {
        useComVol = b;
    }
}
