using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGashaPassive : PassiveAbility
{
    public GameObject soulOrbPrefab;
    public string chargeSound;
    public int bonusOnOrb = 1;
    public int bonusOnKill = 4;
    public float orbSpawnLaunchSpeed = 1f;
    public float passiveTickTime = 3f;
    private float passiveTickTimer = 0f;

    public override void Deactivate()
    {
        counterAmount = 0;
    }

    public void Add(int amount)
    {
        if (counterAmount >= counterMax) return;
        counterAmount += amount;
        if (counterAmount > counterMax) counterAmount = counterMax;
        if (counterAmount == counterMax) audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(chargeSound), 0.5f);
    }

    public void AddAndSpawnOrb(int amount, Vector3 pos)
    {
        Add(amount);
        if (soulOrbPrefab == null) return;
        SoulOrb SO = Instantiate(soulOrbPrefab, pos, Quaternion.identity).GetComponent<SoulOrb>();
        SO.Launch(orbSpawnLaunchSpeed * SO.GetRandomLaunchForce());
    }

    private void Update()
    {
        UpdateChargeBar();
        passiveTickTimer += Time.deltaTime;
        if (passiveTickTimer > passiveTickTime)
        {
            passiveTickTimer = 0f;
            counterAmount += 1;
            if (counterAmount > counterMax) counterAmount = counterMax;
        }
    }
}
