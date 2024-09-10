using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMinotaurWall : AbilityScript
{
    public GameObject wallPrefab;
    public float wallSpawnDistance;
    public float wallDuration;

    public GameObject shrapnelPrefab;
    public int shrapnelDamage;
    public float shrapnelSpeed;
    public int shrapnelAmount;
    public float shrapnelSpreadAngle;

    public override void Activate()
    {
        if (MC.isStunned) return;
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

            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurCreateWall"), 0.2f);
            ANIM.Play("MinotaurWall");
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
