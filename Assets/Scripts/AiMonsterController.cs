using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiMonsterController : MonoBehaviour
{
    protected float ability1Chance = 0.0f;
    protected float ability2Chance = 0.0f;
    protected float ability3Chance = 0.0f;
    protected float kickChance = 0.0f;
    protected MonsterController mc;
    protected Rigidbody rb;
    protected GameplayManager GM;

    // Stats
    [SerializeField] protected float aiKickSpeed;

    // Not necessarily all of these will use the chargeAmount
    protected abstract void PerformAbility1Chance(float chargeAmount);
    protected abstract void PerformAbility2Chance(float chargeAmount);
    protected abstract void PerformAbility3Chance(float chargeAmount);
    protected abstract void PerformKickChance(float chargeAmount);
    protected abstract void MonsterBehaviour();
    protected abstract void Kick();

    protected void Setup()
    {
        mc = GetComponent<MonsterController>();
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
