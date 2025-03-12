using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMinotaurWall : AbilityScript
{
    [Header("Ability Specific Variables")]
    public GameObject wallPrefab;
    public float wallSpawnDistance;
    public float wallDuration;
    public string wallSoundEffect = "minotaurCreateWall";

    public GameObject shrapnelPrefab;
    public int shrapnelDamage;
    public float shrapnelSpeed;
    public int shrapnelAmount;
    public float shrapnelSpreadAngle;
    public float chargeShrapnelMult = 2;
    public bool willSpawnWallAtGoal = true;
    public Vector3 goalWallSpawnPos;

    private void Start()
    {
        Setup();
        goalWallSpawnPos = new Vector3(-6.5f, transform.position.y, transform.position.z);
    }

    public override void Activate()
    {
        if (!canActivate) return;

        if (timer >= cooldown)
        {
            timer = 0f;
            Vector3 spawnLocation = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            Vector3 dir = MC.movementDirection;
            if (dir.Equals(Vector3.zero))
            {
                dir = MC.GetRB().transform.forward.normalized;
                // return;
            }
            spawnLocation = transform.position + (dir * wallSpawnDistance);
            spawnRotation = Quaternion.LookRotation(dir, Vector3.up);
            Instantiate(wallPrefab, spawnLocation, spawnRotation);
            if (willSpawnWallAtGoal)
            {
                Instantiate(wallPrefab, goalWallSpawnPos, Quaternion.LookRotation(new Vector3(-1, 0, 0), Vector3.up));
            }

            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(wallSoundEffect), 0.2f);
            ST.UpdateMAbUsed();
            UM.UpdateMonsterAbilitiesSB();
            ANIM.Play(activatedAnimationName);
            StartCoroutine(MC.MoveDelay());
        }
    }

    /*
    public void SpawnShrapnel()
    {
        if (shrapnelPrefab == null) return;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject shrap = Instantiate(shrapnelPrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
        WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
        WS.damage = shrapnelDamage;
        WS.speed = shrapnelSpeed;
    }
    */
}
