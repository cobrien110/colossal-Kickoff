using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityScript : MonoBehaviour
{
    [Header("Basic Info")]
    public string abilityName = "NoName";
    [Range(0,2)] public int abilityNum = 0;
    protected float timer;
    public float cooldown;
    public bool usableWhileIntangible = false;
    public bool usableWhileDribbling = false;

    [Header("Visuals")]
    public GameObject attackVisualizer;
    public GameObject attackVisualizerPrefab;
    public Transform attackVisHolder;
    public string activatedAnimationName = "none";
    public Sprite icon;

    protected GameplayManager GM;
    protected BallProperties BP;
    protected bool timerPaused = false;
    [HideInInspector] public MonsterController MC;
    [HideInInspector] public AudioPlayer audioPlayer;
    [HideInInspector] public Animator ANIM;
    [HideInInspector] public UIManager UM;

    private PlayerAttachedUI PAUI = null;

    [HideInInspector] public StatTracker ST;

    // Start is called before the first frame update
    private void Start()
    {
        Setup();
    }

    protected void Setup()
    {
        MC = GetComponent<MonsterController>();
        audioPlayer = GetComponent<AudioPlayer>();
        ANIM = MC.GetAnimator();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();

        PAUI = GetComponentInChildren<PlayerAttachedUI>();
        if (PAUI != null) Debug.Log("Dots can be read");

        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        timer = cooldown;
        if (attackVisualizerPrefab != null)
        {
            attackVisualizer = Instantiate(attackVisualizerPrefab, attackVisHolder);
            attackVisualizer.transform.SetParent(gameObject.transform);
            attackVisualizer.transform.localPosition = Vector3.zero;
        }
        //Debug.Log(attackVisualizer);
        MC.abilities.Insert(abilityNum, this);
    }

    private void Update()
    {
        UpdateSetup();
    }

    protected void UpdateSetup()
    {
        if (timer < cooldown && !timerPaused) timer += Time.deltaTime;
        UpdateUI();
        if (BP == null) BP = MC.BP;
        if (MC == null) MC = GetComponent<MonsterController>();
    }

    public string GetName()
    {
        return abilityName;
    }

    public void UpdateUI()
    {
        switch (abilityNum) {
            case 0:
                UM.UpdateMonsterAbility1Bar(1 - (timer / cooldown));
                if (timer >= cooldown)
                {
                    PAUI.UpdateDot1(true);
                }
                else
                {
                    PAUI.UpdateDot1(false);
                }
                break;
            case 1:
                UM.UpdateMonsterAbility2Bar(1 - (timer / cooldown));
                break;
            case 2:
                UM.UpdateMonsterAbility3Bar(1 - (timer / cooldown));
                if (timer >= cooldown)
                {
                    PAUI.UpdateDot2(true);
                }
                else
                {
                    PAUI.UpdateDot2(false);
                }
                break;
            default:
                return;
        }
    }

    public abstract void Activate();
    public virtual void Deactivate()
    {
        // Do nothing;
    }

    public float GetTimer()
    {
        return timer;
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public bool AbilityOffCooldown()
    {
        return timer >= cooldown;
    }
}
