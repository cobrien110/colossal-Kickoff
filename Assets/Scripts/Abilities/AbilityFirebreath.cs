using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFirebreath : AbilityScript
{
    public GameObject fireballPrefab;
    public Transform spawnPoint;
    public float speedOfFireball = 10f;
    public int numToSpawn = 5;
    public float delayBetweenShots = 0.25f;
    public string soundName;

    private float burstTimer;
    private bool isFiring;
    private int shotsFired = 0;

    private GameObject monsterGoal;
    [SerializeField] private float distToStopAtX = 5f;
    [SerializeField] private GameObject magmaPool;
    [SerializeField] private float fireBreathDelay = 0.5f;
    private float fireBreathWidth = 1f;
    [SerializeField] private float baseFireBreathWidth = 1f;
    [SerializeField] private float hitBallPower = 5f;
    [SerializeField] private float hitOrbPower = 5f;
    private AbilityGashaPassive AGP;
    private AbilityCreateHands ACH;
    [SerializeField] private GameObject orb;
    [SerializeField] private int spawnOrbQty = 3;

    

    public void Start()
    {
        Setup();

        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        attackVisualizer.transform.parent = null;
        AGP = GetComponent<AbilityGashaPassive>();
        ACH = GetComponent<AbilityCreateHands>();
    }

    public void Update()
    {
        UpdateSetup();

        /*if (isFiring)
        {
            burstTimer += Time.deltaTime;
            if (burstTimer >= delayBetweenShots)
            {
                FireShot();
            }
        }*/
    }

    public override void Activate()
    {
        if (timer < cooldown) return;

        timer = 0;

        // Set up the attack visualizer
        Vector3 startPosition = new Vector3(monsterGoal.transform.position.x, transform.position.y, transform.position.z);
        Vector3 endPosition = new Vector3(distToStopAtX, transform.position.y, transform.position.z);

        // Start the coroutine for the fire breath attack
        StartCoroutine(FireBreathCoroutine());

        // play animation
        ACH.headAnimator.Play(activatedAnimationName);

        /*if (!isFiring && timer >= cooldown)
        {
            isFiring = true;
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), 0.75f);
            timer = 0;
        }
        if (spawnPoint == null)
        {
            spawnPoint = GetComponent<AbilityCreateHands>().head.transform;
            if (spawnPoint == null) // FAILSAFE IF NO HAND CREATION CODE
            {
                spawnPoint = GameObject.Find("MonsterGoal").transform;
            }
        }*/
    }

    private IEnumerator FireBreathCoroutine()
    {
        // Initial delay before the fire breath starts
        yield return new WaitForSeconds(fireBreathDelay);

        Debug.Log("FireBreath ability activated");

        // If ability is charged, then shoot out some orbs toward the warrior side
        if (AGP.counterAmount == AGP.counterMax)
        {
            Debug.Log("Charged ability activated! Spawning orbs.");

            fireBreathWidth *= 2f;

            for (int i = 0; i < spawnOrbQty; i++)
            {
                // Randomize spawn position slightly
                Vector3 randomOffset = new Vector3(0, Random.Range(-0.2f, 0.2f), Random.Range(-0.5f, 0.5f));
                Vector3 spawnPosition = transform.position + new Vector3(1f, 0, 0) + randomOffset;

                // Instantiate the orb
                GameObject orbInstance = Instantiate(orb, spawnPosition, Quaternion.identity);

                // Apply force to the orb in the positive x-direction with some randomness
                Vector3 force = new Vector3(1f * hitOrbPower, 0, Random.Range(-0.2f, 0.2f));
                //orbInstance.GetComponent<Rigidbody>().AddForce(forceDirection * hitOrbPower, ForceMode.Impulse);
                orbInstance.GetComponent<SoulOrb>().Launch(force);
            }

            AGP.counterAmount = 0;
        }

        // Dynamically define the start and end positions based on the monster's current position
        Vector3 startPosition = new Vector3(monsterGoal.transform.position.x, transform.position.y, transform.position.z);
        Vector3 endPosition = new Vector3(distToStopAtX, transform.position.y, transform.position.z);

        float attackLength = Mathf.Abs(endPosition.x - startPosition.x);

        // Update the attack visualizer's position and scale
        attackVisualizer.transform.position = startPosition + new Vector3(attackLength / 2, -0.3f, 0); // Center the visualizer
        attackVisualizer.transform.localScale = new Vector3(attackLength, 0.1f, fireBreathWidth); // Adjust its size
        attackVisualizer.SetActive(true);

        // Define the size of the overlap box and the step increment
        Vector3 boxSize = new Vector3(1f, 1f, fireBreathWidth); // Adjust the size as needed
        float stepDistance = 1f; // Distance between each overlap box
        float delay = 0.05f; // Delay between each overlap box
        float currentX = startPosition.x;

        while (currentX <= endPosition.x)
        {
            Vector3 boxCenter = new Vector3(currentX, startPosition.y, startPosition.z);

            // Perform the overlap box
            Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2);
            foreach (Collider collider in hitColliders)
            {
                // Check for warriors or other objects to damage
                WarriorController warrior = collider.GetComponent<WarriorController>();
                if (warrior != null)
                {
                    Debug.Log($"FireBreath hit: {warrior.name}");
                    warrior.Die();
                }

                BallProperties ball = collider.GetComponent<BallProperties>();
                if (ball != null)
                {

                    ball.GetComponent<Rigidbody>().AddForce(new Vector3(1f, 0, 0) * hitBallPower, ForceMode.Impulse);
                }

                SoulOrb orb = collider.GetComponent<SoulOrb>();
                if (orb != null)
                {

                    orb.GetComponent<Rigidbody>().AddForce(new Vector3(1f, 0, 0f) * hitOrbPower, ForceMode.Impulse);
                }
            }

            // Spawn the magma pool at the current box position
            if (magmaPool != null)
            {
                GameObject magmaPoolObj = Instantiate(magmaPool, boxCenter, Quaternion.identity);
                magmaPoolObj.transform.position += new Vector3(0, -0.3f, 0);
            }

            // Move to the next position and wait briefly
            currentX += stepDistance;
            yield return new WaitForSeconds(delay);
        }

        // Deactivate the attack visualizer after the fire breath completes
        attackVisualizer.SetActive(false);
        fireBreathWidth = baseFireBreathWidth;
        Debug.Log("FireBreath ability completed");
    }

    private void OnDrawGizmos()
    {
        // Define the start and end positions for the firebreath
        Vector3 startPosition = new Vector3(monsterGoal.transform.position.x, transform.position.y, transform.position.z);
        Vector3 endPosition = new Vector3(distToStopAtX, transform.position.y, transform.position.z);

        // Define the size of each box
        Vector3 boxSize = new Vector3(1f, 1f, 1f); // Adjust the size as needed

        // Calculate the number of steps based on the firebreath length and step distance
        float stepDistance = 1f; // Distance between each overlap box
        int steps = Mathf.CeilToInt((endPosition.x - startPosition.x) / stepDistance);

        // Draw each box along the firebreath's length
        for (int i = 0; i <= steps; i++)
        {
            float currentX = startPosition.x + i * stepDistance;
            Vector3 boxCenter = new Vector3(currentX, startPosition.y, startPosition.z);

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Semi-transparent orange
            Gizmos.DrawCube(boxCenter, boxSize); // Draw the filled cube
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(boxCenter, boxSize); // Draw the wireframe cube
        }
    }


    public void FireShot()
    {
        Fireball f = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Fireball>();
        f.speed = speedOfFireball;
        f.MC = MC;
        f.Activate();
        shotsFired++;
        burstTimer = 0;

        if (shotsFired >= numToSpawn)
        {
            isFiring = false;
            shotsFired = 0;
        }
    }
}
