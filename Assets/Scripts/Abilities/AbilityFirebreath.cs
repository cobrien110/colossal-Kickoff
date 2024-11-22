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

    public void Start()
    {
        Setup();

        monsterGoal = GameObject.FindWithTag("MonsterGoal");
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
        // Do a series of slightly delayed overlap boxes
            // Starts at x pos of monster goal and z pos of monster
            // Goes to warrior spawn
        StartCoroutine(FireBreathCoroutine());

        // Delay it

        // Leave lava pools in path

        // Show visual

        // Balls and orbs get pushed forward



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
        Debug.Log("FireBreath ability activated");

        // Define the start and end positions
        Vector3 startPosition = new Vector3(monsterGoal.transform.position.x, transform.position.y, transform.position.z);
        Vector3 endPosition = new Vector3(distToStopAtX, transform.position.y, transform.position.z);

        // Define the size of the overlap box and the step increment
        Vector3 boxSize = new Vector3(1f, 1f, 1f); // Adjust the size as needed
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
            }

            // Spawn the magma pool at the current box position
            if (magmaPool != null)
            {
                Instantiate(magmaPool, boxCenter, Quaternion.identity);
            }

            // Move to the next position and wait briefly
            currentX += stepDistance;
            yield return new WaitForSeconds(delay);
        }

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
