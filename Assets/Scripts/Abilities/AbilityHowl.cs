using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHowl : AbilityScript
{
    [Header("Ability Specific Variables")]
    public float howlRadius = 1f;
    public float crystalRadius = 1f;
    public float stunTime = 1.5f;
    public float delayBeforeActivate = 0.5f;

    public string soundName;
    public IceCrystal currentCrystal;
    private AbilityAkhlutPassive AAP;
    /*
    
    1. Stop momentum of ball if in radius
    
    2. Stun all warrior in radius

    */

    public override void Activate()
    {
        if (timer < cooldown) return;
        timer = 0;
        Invoke("DelayedActivate", delayBeforeActivate);
        ANIM.Play(activatedAnimationName);
    }

    private void DelayedActivate()
    {
        attackVisualizer.SetActive(true);
        // Schedule the deactivation after 1 second
        Invoke(nameof(DeactivateVisualizer), 1f);
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), .6f);

        Debug.Log("Howl");

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, howlRadius);
        foreach (Collider obj in objectsInRange)
        {
            // Debug.Log("Objects in range of howl:" + obj.name);

            // Check for ball
            if (obj.GetComponent<BallProperties>() != null)
            {
                // Stop ball
                try
                {
                    MC.BP.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    MC.BP.GetComponent<Rigidbody>().rotation = Quaternion.identity;
                    // Debug.Log("Ball stopped by howl");
                }
                catch
                {
                    Rigidbody ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallProperties>().GetComponent<Rigidbody>();
                    ball.velocity = Vector3.zero;
                    ball.rotation = Quaternion.identity;
                }


            }

            // Check for warriors
            if (obj.GetComponent<WarriorController>() != null)
            {
                // Stun warrior
                obj.GetComponent<WarriorController>().Stun(stunTime);
                // Debug.Log("Stunned Warrior: " + obj.name);
            }
        }
        // if there is a crystal on the field
        if (currentCrystal != null)
        {
            currentCrystal.GetComponent<IceCrystal>().Echo();
            AAP.SetActive(false);
            AAP.SetCounter(0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        attackVisualizer.transform.localScale *= howlRadius * 1.7f;
        AAP = GetComponent<AbilityAkhlutPassive>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
        //Debug.Log("Cooldown: " + cooldown);
        //Debug.Log("Timer: " + timer);
        if (attackVisualizer.activeSelf)
        {
            Debug.Log("Attack Visual Active");
        }
     
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
        Gizmos.DrawWireSphere(transform.position, howlRadius);
    }

    public override void Deactivate()
    {
        CancelInvoke();
    }
}
