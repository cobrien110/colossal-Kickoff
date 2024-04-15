using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentatorSoundManager : MonoBehaviour
{
    List<List<List<AudioClip>>> allSounds = new List<List<List<AudioClip>>>();
    List<List<AudioClip>> comSoundsA = new List<List<AudioClip>>();
    List<List<AudioClip>> comSoundsB = new List<List<AudioClip>>();
    List<List<AudioClip>> comSoundsAB = new List<List<AudioClip>>();

    public bool canPlaySounds = true;
    public float volume = 0.5f;
    [Header("Commentator A Sounds")]
    public List<AudioClip> comSoundsA_WarriorGoal;
    public List<AudioClip> comSoundsA_MonsterGoal;
    public List<AudioClip> comSoundsA_WarriorDeath;
    public List<AudioClip> comSoundsA_MonsterDeath;
    [Header("Commentator B Sounds")]
    public List<AudioClip> comSoundsB_WarriorGoal;
    public List<AudioClip> comSoundsB_MonsterGoal;
    public List<AudioClip> comSoundsB_WarriorDeath;
    public List<AudioClip> comSoundsB_MonsterDeath;
    [Header("Combo Sounds")]
    public List<AudioClip> comSoundsC_WarriorGoal;
    public List<AudioClip> comSoundsC_MonsterGoal;
    public List<AudioClip> comSoundsC_WarriorDeath;
    public List<AudioClip> comSoundsC_MonsterDeath;

    private AudioPlayer AP;

    private void Start()
    {
        AP = GetComponent<AudioPlayer>();

        comSoundsA.Add(comSoundsA_WarriorGoal);
        comSoundsA.Add(comSoundsA_MonsterGoal);
        comSoundsA.Add(comSoundsA_WarriorDeath);
        comSoundsA.Add(comSoundsA_MonsterDeath);

        comSoundsB.Add(comSoundsB_WarriorGoal);
        comSoundsB.Add(comSoundsB_MonsterGoal);
        comSoundsB.Add(comSoundsB_WarriorDeath);
        comSoundsB.Add(comSoundsB_MonsterDeath);

        comSoundsAB.Add(comSoundsC_WarriorGoal);
        comSoundsAB.Add(comSoundsC_MonsterGoal);
        comSoundsAB.Add(comSoundsC_WarriorDeath);
        comSoundsAB.Add(comSoundsC_MonsterDeath);

        allSounds.Add(comSoundsA);
        allSounds.Add(comSoundsB);
        allSounds.Add(comSoundsAB);
    }

    public void PlayGoalSound(bool isWarriorGoal)
    {
        if (!canPlaySounds) return;
        AudioClip clip = comSoundsA_WarriorGoal[0];
        int rVoice = Random.Range(0, allSounds.Count);
        int teamClips = isWarriorGoal ? 0 : 1;
        List<List<AudioClip>> commentatorClips = allSounds[rVoice];
        List<AudioClip> theseClips = commentatorClips[teamClips];

        int clipIndex = Random.Range(0, theseClips.Count - 1);
        clip = theseClips[clipIndex];

        if (!AP.isPlaying()) AP.PlaySoundVolume(clip, volume);
    }

    public void PlayDeathSound(bool isWarriorDeath)
    {
        /*
        AudioClip clip = comSoundsA_WarriorDeath[0];
        int rVoice = Random.Range(0, 3);
        int teamClips = isWarriorDeath ? 2 : 3;
        List<AudioClip[]> commentatorClips = allSounds[rVoice];
        AudioClip[] theseClips = commentatorClips[teamClips];

        int clipIndex = Random.Range(0, theseClips.Length);
        clip = theseClips[clipIndex];

        AP.PlaySound(clip);
        */
    }
}
