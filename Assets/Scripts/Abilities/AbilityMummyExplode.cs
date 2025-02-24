using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityMummyExplode : AbilityScript
{
    [SerializeField] private float distanceUntilExplode = 1f;
    [SerializeField] private float pursueSpeed = 3f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private GameObject slowAura;
    [SerializeField] private string soundName;
    [SerializeField] private string explodeSoundName;

    public override void Activate()
    {
        if (!canActivate) return;

        // Debug.Log("Timer: " + timer + ", Cooldown: " + cooldown);
        if (timer < cooldown)
        {
            // Debug.Log("Not off cooldown - can't activate Mummy Explode");
            return; // Ability not off cooldown
        }

        // Ensure there are mummies in scene
        AIMummy[] mummies = FindObjectsOfType<AIMummy>();
        if (mummies.Length < 1)
        {
            Debug.Log("No mummies - can't activate Mummy Explode");
            return; // No mummies in scene
        }

        // Ensure ball owner is a warrior
        if (BP.ballOwner == null || BP.ballOwner.GetComponent<WarriorController>() == null)
        {
            Debug.Log("Ball owner is not a warrior - can't activate Mummy Explode");
            return; // Ball owner is not a warrior
        }

        timer = 0;
        // Find nearest mummy to warrior ball owner

        AIMummy nearestMummy = null;
        float closestDistance = 100f;
        foreach (AIMummy m in mummies)
        {
            float distance = Vector3.Distance(m.gameObject.transform.position, BP.ballOwner.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestMummy = m;
            }
        }

        WarriorController target = BP.ballOwner.GetComponent<WarriorController>();
        if (nearestMummy != null && target != null) StartCoroutine(PursueBallOwner(nearestMummy, target));

        // When close enough to ball owner, mummy explodes, killing ball owner

        // Play sound
        audioPlayer.PlaySoundRandomPitch(audioPlayer.Find(soundName));
        ANIM.Play(activatedAnimationName);
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
    }

    private IEnumerator PursueBallOwner(AIMummy pursuer, WarriorController target)
    {
        // Visually show pursuer
        SpriteRenderer SR = pursuer.gameObject.GetComponentInChildren<SpriteRenderer>();
        SR.color = new Color(255, 0, 0);

        // Debug.Log("PursueBallOwner start");

        Rigidbody pursuerRB = pursuer.gameObject.GetComponent<Rigidbody>();
        pursuer.SetIsPursuing(true);
        // While not in range to explode
        while (pursuer != null & target != null
            && Vector3.Distance(pursuer.transform.position, target.gameObject.transform.position) > distanceUntilExplode)
        {
            // Go toward warrior
            pursuerRB.velocity = (target.transform.position - pursuer.transform.position).normalized * pursueSpeed;

            yield return null;
        }

        pursuer.SetIsPursuing(false);

        // Debug.Log("PursueBallOwner end");

        ExplodeMummy(pursuer);
    }

    private void ExplodeMummy(AIMummy pursuer)
    {
        if (pursuer == null) return;

        Debug.Log("Explode Mummy!");

        LayerMask warriorLayer = LayerMask.GetMask("Warrior");

        // Detect all objects within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(pursuer.gameObject.transform.position, explosionRadius, warriorLayer);

        // Use a HashSet to avoid duplicate hits on the same warrior
        HashSet<WarriorController> hitWarriors = new HashSet<WarriorController>();

        foreach (Collider collider in hitColliders)
        {
            WarriorController warrior = collider.GetComponent<WarriorController>();
            if (warrior != null && !hitWarriors.Contains(warrior))
            {
                // Add to the set to ensure it doesn't get processed again
                hitWarriors.Add(warrior);

                // Kill warrior
                Debug.Log($"Warrior {warrior.name} hit by {pursuer.name} explosion!");

                warrior.Die();
            }
        }

        // Create slow aura at point of explosion
        Instantiate(slowAura, pursuer.gameObject.transform.position, Quaternion.identity);

        // Destroy the mummy after exploding
        pursuer.Die(true);

        // play sound
        audioPlayer.PlaySoundRandomPitch(audioPlayer.Find(explodeSoundName));
    }
}
