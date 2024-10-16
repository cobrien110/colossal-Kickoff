using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFly : AbilityScript
{
    [Header("Ability Specific Variables")]
    public bool isActive = false;
    [HideInInspector] public float activeDuration = 0f;
    public float duration = 1.5f;
    public float speed = 5f;
    private float baseSpeed;
    public float slamRadius = 3f;

    private GameObject sprite;
    private float spritePositionY;
    public float maxSpriteYOffset = 5f;
    private float sY;
    private Quaternion spriteRotation;

    public string activatedSoundName;
    public string slamSoundName;

    void Start()
    {
        Setup();

        sprite = MC.spriteObject;
        spritePositionY = sprite.transform.position.y;
        sY = spritePositionY + maxSpriteYOffset;
        spriteRotation = sprite.transform.rotation;
        attackVisualizer.transform.localScale *= slamRadius * 1.7f;
        baseSpeed = MC.monsterSpeed;
    }

    void Update()
    {
        UpdateSetup();

        if (isActive)
        {
            MC.isIntangible = true;
            MC.monsterSpeed = speed;
            activeDuration += Time.deltaTime;
            timerPaused = true;
            sprite.transform.position = Vector3.Lerp(sprite.transform.position, new Vector3(sprite.transform.position.x, sY, sprite.transform.position.z), Time.deltaTime);
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
            sprite.transform.position = Vector3.Lerp(sprite.transform.position, new Vector3(sprite.transform.position.x, spritePositionY, sprite.transform.position.z), Time.deltaTime * 10);
            //sprite.transform.SetPositionAndRotation(new Vector3(sprite.transform.position.x, spritePositionY, sprite.transform.position.z), sprite.transform.rotation);
        }
    }

    public override void Activate()
    {
        if (timer < cooldown) return;
        if (!isActive)
        {
            timer = 0;
            isActive = true;
            //sprite.transform.SetPositionAndRotation(new Vector3(sprite.transform.position.x, 10, sprite.transform.position.z), sprite.transform.rotation);
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(activatedSoundName), 0.95f);
        }
    }

    private void Slam()
    {
        attackVisualizer.SetActive(true);
        // Schedule the deactivation after 1 second
        Invoke(nameof(DeactivateVisualizer), 0.5f);

        Debug.Log("Slam");

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
    }

    // Method to deactivate the visualizer
    private void DeactivateVisualizer()
    {
        attackVisualizer.SetActive(false);
    }

    // Draw gizmo to visualize howl radius
    private void OnDrawGizmosSelected()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the object's position with the radius of howlRadius
        Gizmos.DrawWireSphere(transform.position, slamRadius);
    }
}
