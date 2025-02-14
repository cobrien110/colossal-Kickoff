using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentatorSoundManager : MonoBehaviour
{
    List<List<List<AudioClip>>> allSounds = new List<List<List<AudioClip>>>();
    List<List<AudioClip>> comSoundsA = new List<List<AudioClip>>();
    List<List<AudioClip>> comSoundsB = new List<List<AudioClip>>();
    List<List<AudioClip>> comSoundsAB = new List<List<AudioClip>>();

    public float goalSoundFreq = 1f;
    public float deathSoundFreq = 0.35f;
    public float stunSoundFreq = 1f;
    public float goalSoundDelay = 1f;
    public float killSoundDelay = 0.75f;
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
        bool willPlay = true;
        if (!canPlaySounds) willPlay = false;
        float r = Random.Range(0.0f, 1.0f);
        if (r >= goalSoundFreq) willPlay = false;

        if (willPlay)
        {
            StartCoroutine(GoalSoundHelper(isWarriorGoal));
        } 
    }
    private IEnumerator GoalSoundHelper(bool isWarriorGoal)
    {
        yield return new WaitForSeconds(goalSoundDelay);
        AudioClip clip = comSoundsA_WarriorGoal[0];
        int rVoice = Random.Range(0, allSounds.Count);
        int teamClips = isWarriorGoal ? 0 : 1;
        List<List<AudioClip>> commentatorClips = allSounds[rVoice];
        List<AudioClip> theseClips = commentatorClips[teamClips];

        int clipIndex = Random.Range(0, theseClips.Count);
        clip = theseClips[clipIndex];

        if (!AP.isPlaying()) AP.PlaySoundVolume(clip, volume);
    }

    public void PlayDeathSound(bool isWarriorDeath)
    {
        bool willPlay = true;
        if (!canPlaySounds) willPlay = false;
        float r = Random.Range(0.0f, 1.0f);
        float freq = isWarriorDeath ? deathSoundFreq : stunSoundFreq;
        if (r >= freq) willPlay = false;

        if (willPlay)
        {
            StartCoroutine(DeathSoundHelper(isWarriorDeath));
        }
    }

    private IEnumerator DeathSoundHelper(bool isWarriorDeath)
    {
        yield return new WaitForSeconds(killSoundDelay);
        AudioClip clip = comSoundsA_WarriorGoal[0];
        int rVoice = Random.Range(0, allSounds.Count);
        int teamClips = isWarriorDeath ? 2 : 3;
        List<List<AudioClip>> commentatorClips = allSounds[rVoice];
        List<AudioClip> theseClips = commentatorClips[teamClips];

        int clipIndex = Random.Range(0, theseClips.Count);
        clip = theseClips[clipIndex];

        if (!AP.isPlaying()) AP.PlaySoundVolume(clip, volume);
    }
}
