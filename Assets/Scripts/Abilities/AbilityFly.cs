using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityFly : AbilityScript
{
    [Header("Ability Specific Variables")]
    public bool isActive = false;
    [HideInInspector] public float activeDuration = 0f;
    public float duration = 1.5f;
    public float speedBonus = 5f;
    [HideInInspector] public float baseSpeed;
    public float slamRadius = 3f;
    [SerializeField] private GameObject slamParticles;
    [SerializeField] private Transform slamParticlesTransform;

    private GameObject sprite;
    [SerializeField] private Transform tranformForCam;
    private float spritePositionY;
    public float maxSpriteYOffset = 5f;
    private float sY;
    private Quaternion spriteRotation;

    public string activatedSoundName;
    public string slamSoundName;
    private MultipleTargetCamera MTC;
    private CapsuleCollider col;
    private float visualizerOffsetY = -0.3f;

    void Start()
    {
        Setup();

        MTC = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MultipleTargetCamera>();
        col = GetComponent<CapsuleCollider>();
        sprite = MC.spriteObject;
        spritePositionY = sprite.transform.position.y;
        sY = spritePositionY + maxSpriteYOffset;
        spriteRotation = sprite.transform.rotation;
        baseSpeed = MC.monsterSpeed;
        attackVisualizer.transform.position =
            new Vector3(attackVisualizer.transform.position.x, attackVisualizer.transform.position.y + visualizerOffsetY, attackVisualizer.transform.position.z);
    }

    void Update()
    {
        UpdateSetup();
        attackVisualizer.transform.localScale = new Vector3(1, 0.05f, 1) * slamRadius * 2f;

        if (isActive)
        {
            MC.isIntangible = true;
            MC.monsterSpeed = baseSpeed + speedBonus;
            activeDuration += Time.deltaTime;
            timerPaused = true;
            
            if (activeDuration >= duration)
            {
                isActive = false;
                Slam();
            }
        } else
        {
            MC.isIntangible = false;
            MC.monsterSpeed = baseSpeed;
            timerPaused = false;
            activeDuration = 0;
            
            //sprite.transform.SetPositionAndRotation(new Vector3(sprite.transform.position.x, spritePositionY, sprite.transform.position.z), sprite.transform.rotation);
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            sprite.transform.position = Vector3.Lerp(sprite.transform.position, 
                new Vector3(sprite.transform.position.x, sY, sprite.transform.position.z), Time.deltaTime * 1.2f);
            col.center = new Vector3(0, sprite.transform.position.y, 0);
        } else if (MC.canUseAbilities)
        {
            sprite.transform.position = Vector3.Lerp(sprite.transform.position, 
                new Vector3(sprite.transform.position.x, spritePositionY, sprite.transform.position.z), Time.deltaTime * 10);
            col.center = new Vector3(0, 0.5f, 0);
        }
        
    }

    public override void Activate()
    {
        if (!canActivate) return;
        if (timer < cooldown) return;
        if (!isActive)
        {
            timer = 0;
            isActive = true;
            ST.UpdateMAbUsed();
            UM.UpdateMonsterAbilitiesSB();
            //sprite.transform.SetPositionAndRotation(new Vector3(sprite.transform.position.x, 10, sprite.transform.position.z), sprite.transform.rotation);
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(activatedSoundName), 0.95f);

            if (MTC.CheckTarget(transform))
            {
                MTC.AddTarget(tranformForCam);
                //MTC.RemoveTarget(transform);
            }
        }
    }

    private void Slam()
    {
        attackVisualizer.SetActive(true);
        // Schedule the deactivation after 1 second
        Invoke(nameof(DeactivateVisualizer), 0.5f);

        Debug.Log("Slam");

        if (slamParticles != null)
        {
            GameObject particleInstance = Instantiate(slamParticles, slamParticlesTransform.position, Quaternion.Euler(90, 0, 0), transform);
            Vector3 originalScale = particleInstance.transform.localScale;
            particleInstance.transform.localScale = originalScale * slamRadius * 1f;
        }

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, slamRadius);
        foreach (Collider obj in objectsInRange)
        {
            // Check for warriors
            if (obj.GetComponent<WarriorController>() != null)
            {
                //  warrior
                obj.GetComponent<WarriorController>().Die();
                // Debug.Log("Stunned Warrior: " + obj.name);
            }
        }
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(slamSoundName), 0.75f);

        if (MTC.CheckTarget(tranformForCam))
        {
            //MTC.AddTarget(transform);
            MTC.RemoveTarget(tranformForCam);
        }
    }

    public override void Deactivate()
    {
        //Debug.Log("Deactivating " + abilityName);
        activeDuration = 0;
        isActive = false;
        sprite.transform.position = new Vector3(sprite.transform.position.x, spritePositionY, sprite.transform.position.z);

        if (MTC.CheckTarget(sprite.transform))
        {
            MTC.AddTarget(transform);
            MTC.RemoveTarget(sprite.transform);
        }
    }

    // Method to deactivate the visualizer
    private void DeactivateVisualizer()
    {
        attackVisualizer.SetActive(false);
    }

    // Draw gizmo to visualize howl radius
    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the object's position with the radius of howlRadius
        Gizmos.DrawWireSphere(transform.position, slamRadius);
    }

    public float GetMaxHeight()
    {
        return maxSpriteYOffset;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

}
