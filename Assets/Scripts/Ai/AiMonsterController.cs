using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    protected GameObject monsterGoal;
    protected bool isPerformingAbility = false;
    [HideInInspector] public List<GameObject> warriors;
    // protected List<WarriorController> warriors;

    // Stats
    [Header("AI Monster Stats & Behaviour")]
    [SerializeField] protected float aiShootSpeed;
    [SerializeField] private float performActionChanceFrequency = 0.25f; // How often monster checks to perform ability
    [SerializeField] protected float maxShootingRange = 16f; // 16 is an estimate of the width of the whole field
    [SerializeField] protected float maxProximityRange = 8f; // Distance from nearest warrior that is considered "absolutely safe to shoot from"
    [SerializeField] protected float midFieldPoint = 0f; // Represent the value on x axis that is midfield
    [SerializeField] protected float leftBoundary = -4f; // Left boundary for monster roaming purposes
    [SerializeField] protected float fieldDepth = 3f; // field depth for monster roaming purposes

    // Not necessarily all of these will use the chargeAmount
    protected abstract void PerformAbility1Chance();
    protected abstract void PerformAbility2Chance();
    protected abstract void PerformAbility3Chance();
    protected abstract void PerformShootChance();
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
            if (GM.IsPlayingGet() && mc != null && mc.BP != null && mc.BP.isInteractable)
            {
                // Debug.Log("isPlaying");
                if (!isPerformingAbility) PerformAbility1Chance();
                if (!isPerformingAbility) PerformAbility2Chance();
                if (!isPerformingAbility) PerformAbility3Chance();
                PerformShootChance();
            } else if (mc != null)
            {
                // Debug.Log("IsPlayingGet: " + GM.IsPlayingGet());
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
        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        warriors = FindObjectsOfType<WarriorController>().Select(w => w.gameObject).ToList();


        StartCoroutine(PerformActionChances());
        // StartCoroutine(PrintWarriors());
    }

    public void SetIsPerformingAbility(bool isPerformingAbility)
    {
        this.isPerformingAbility = isPerformingAbility;
    }

    public IEnumerator SetIsPerformingAbilityDelay(bool isPerformingAbility)
    {
        yield return new WaitForSeconds(0.5f);

        this.isPerformingAbility = isPerformingAbility;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("AiMonsterController start");
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("AiMonsterController update");
    }

    IEnumerator PrintWarriors()
    {
        Debug.Log("Print Warriors");
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Warriors: " + warriors.Count);
        }
    }
}
