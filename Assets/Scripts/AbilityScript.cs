using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityScript : MonoBehaviour
{
    public string abilityName = "NoName";
    public int abilityNum = 0;
    [HideInInspector] public MonsterController MC;
    [HideInInspector] public AudioPlayer audioPlayer;
    [HideInInspector]  public Animator ANIM;
    [HideInInspector] public UIManager UM;
    protected float timer;
    public float cooldown;

    // Start is called before the first frame update
    private void Start()
    {
        MC = GetComponent<MonsterController>();
        audioPlayer = GetComponent<AudioPlayer>();
        ANIM = MC.GetAnimator();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        timer = cooldown;
    }

    private void Update()
    {
        if (timer < cooldown) timer += Time.deltaTime;
        UpdateUI();
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
