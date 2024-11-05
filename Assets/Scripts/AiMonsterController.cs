using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiMonsterController : MonoBehaviour
{
    protected float ability1Chance = 0.0f;
    protected float ability2Chance = 0.0f;
    protected float ability3Chance = 0.0f;
    protected float shootChance = 0.0f;
    protected MonsterController mc;
    protected Rigidbody rb;
    protected GameplayManager GM;
    protected GameObject warriorGoal;
    private bool isUsingAbility = false;
    // protected List<WarriorController> warriors;

    // Stats
    [SerializeField] protected float aiShootSpeed;
    [SerializeField] private float performActionChanceFrequency = 0.25f;
    [SerializeField] protected float maxShootingRange = 16f; // 16 is an estimate of the width of the whole field
    [SerializeField] protected float maxProximityRange = 8f; // Distance from nearest warrior that is considered "absolutely safe to shoot from"
    [SerializeField] protected float midFieldPoint = 0f; // Represent the value on x axis that is midfield

    // Not necessarily all of these will use the chargeAmount
    protected abstract void PerformAbility1Chance(float chargeAmount);
    protected abstract void PerformAbility2Chance(float chargeAmount);
    protected abstract void PerformAbility3Chance(float chargeAmount);
    protected abstract void PerformShootChance(float chargeAmount);
    protected abstract void MonsterBehaviour();
    protected abstract void Shoot();

    protected bool IsInWarriorHalf(GameObject gameObject)
    {
        return gameObject.transform.position.x > midFieldPoint;
    }

    IEnumerator PerformActionChances()
    {
        while (true)
        {
            if (!isUsingAbility)
            {
                // Debug.Log("Performing action chances");
                PerformAbility1Chance(0);
                PerformAbility2Chance(0);
                PerformAbility3Chance(0);
                PerformShootChance(0);
            }
            yield return new WaitForSeconds(performActionChanceFrequency);
        }
    }

    private void StartPerformActionChances()
    {
        Debug.Log("Start PerformActionChances");
        StartCoroutine(PerformActionChances());
    }

    protected void Setup()
    {
        mc = GetComponent<MonsterController>();
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        warriorGoal = GameObject.FindWithTag("WarriorGoal");

        StartCoroutine(PerformActionChances());
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
