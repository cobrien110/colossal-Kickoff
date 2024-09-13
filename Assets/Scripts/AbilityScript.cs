using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityScript : MonoBehaviour
{
    [Header("Basic Info")]
    public string abilityName = "NoName";
    [Range(0,3)] public int abilityNum = 0;
    protected float timer;
    public float cooldown;

    [Header("Visuals")]
    public GameObject attackVisualizer;
    public GameObject attackVisualizerPrefab;
    public Transform attackVisHolder;
    public string activatedAnimationName = "minotaurAxeCharge";

    protected GameplayManager GM;
    protected BallProperties BP;
    [HideInInspector] public MonsterController MC;
    [HideInInspector] public AudioPlayer audioPlayer;
    [HideInInspector] public Animator ANIM;
    [HideInInspector] public UIManager UM;

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
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        timer = cooldown;
        if (attackVisualizerPrefab != null) attackVisualizer = Instantiate(attackVisualizerPrefab, attackVisHolder);
        Debug.Log(attackVisualizer);
    }

    private void Update()
    {
        UpdateSetup();
    }

    protected void UpdateSetup()
    {
        if (timer < cooldown) timer += Time.deltaTime;
        UpdateUI();
        BP = MC.BP;
    }

    public string GetName()
    {
        return abilityName;
    }

    public void UpdateUI()
    {
        switch (abilityNum) {
            case 1:
                UM.UpdateMonsterAbility1Bar(1 - (timer / cooldown));
                break;
            case 2:
                UM.UpdateMonsterAbility2Bar(1 - (timer / cooldown));
                break;
            case 3:
                UM.UpdateMonsterAbility3Bar(1 - (timer / cooldown));
                break;
            default:
                return;
        }
    }

    public abstract void Activate();
}
