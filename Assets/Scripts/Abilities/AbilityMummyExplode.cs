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

    public override void Activate()
    {
        // Debug.Log("Timer: " + timer + ", Cooldown: " + cooldown);
        if (timer < cooldown)
        {
            // Debug.Log("Not off cooldown - can't activate Mummy Explode");
            return; // Ability not off cooldown
        }
        timer = 0;

        // Ensure there are mummies in scene
        AIMummy[] mummies = FindObjectsOfType<AIMummy>();
        if (mummies.Length < 1)
        {
            Debug.Log("No mummies - can't activate Mummy Explode");
            return; // No mummies in scene
        }

        // Ensure ball owner is a warrior
        if (BP.ballOwner.GetComponent<WarriorController>() == null)
        {
            Debug.Log("Ball owner is not a warrior - can't activate Mummy Explode");
            return; // Ball owner is not a warrior
        }

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
        Debug.Log("PursueBallOwner start");

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

        Debug.Log("PursueBallOwner end");

        ExplodeMummy(pursuer);
    }

    private void ExplodeMummy(AIMummy pursuer)
    {
        if (pursuer == null) return;

        Debug.Log("Explode Mummy!");

        LayerMask warriorLayer = LayerMask.GetMask("Warrior");

        // Detect all objects within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(pursuer.gameObject.transform.position, explosionRadius, warriorLayer);

        foreach (Collider collider in hitColliders)
        {
            WarriorController warrior = collider.GetComponent<WarriorController>();
            if (warrior != null)
            {
                // Kill warrior
                Debug.Log($"Warrior {warrior.name} hit by explosion!");

                warrior.Die();
            }
        }

        // Create slow aura at point of explosion
        Instantiate(slowAura, pursuer.gameObject.transform.position, Quaternion.identity);

        // Destroy the mummy after exploding
        pursuer.Die(true);
    }
}
