using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySphinxCurse : AbilityScript
{
    public GameObject projectilePrefab;
    public float projectileSpeed;
    [SerializeField] private string soundName;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
    }

    public override void Activate()
    {
        if (!canActivate) return;

        if (timer >= cooldown)
        {
            timer = 0;
            CurseProjectile c = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up)).GetComponent<CurseProjectile>();
            Debug.Log("transform.forward: " + transform.forward);
            Debug.Log("Quaternion.LookRotation(transform.forward, Vector3.up): " + Quaternion.LookRotation(transform.forward, Vector3.up));
            Debug.Log("c.transform.forward" + c.transform.forward);
            c.speed = projectileSpeed;
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find(soundName));
            ANIM.Play(activatedAnimationName);
            ST.UpdateMAbUsed();
            UM.UpdateMonsterAbilitiesSB();
        }
    }
}
