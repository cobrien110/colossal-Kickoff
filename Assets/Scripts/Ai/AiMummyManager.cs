using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMummyManager : MonoBehaviour
{

    private Vector3 spawnPosition1;
    private Vector3 spawnPosition2;
    private float spawnPositionOffsetX = 4f;
    private float spawnPositionOffsetZ = 1.7f;
    private bool usePosition1 = true; // Use for logic that alternates spawn positions
    private GameObject monsterGoal;
    private int activeMummyCount; // How many mummies are alive or queued to respawn
    private int livingMummyCount = 0; // How many mummies are alive
    [SerializeField] private int maxMummyCount = 6; // Max number of mummies that can be active
    [SerializeField] private GameObject mummyPrefab;
    [SerializeField] private float respawnDelay;
    [SerializeField] private int startingMummyAmount = 2;
    UIManager UM;

    // Start is called before the first frame update
    void Awake()
    {
        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        spawnPosition1 = new Vector3(monsterGoal.transform.position.x + spawnPositionOffsetX, 0f, spawnPositionOffsetZ);
        spawnPosition2 = new Vector3(monsterGoal.transform.position.x + spawnPositionOffsetX, 0f, -spawnPositionOffsetZ);
        activeMummyCount = startingMummyAmount;
    }

    void Start()
    {
        UM = FindObjectOfType<UIManager>();
        ResetMummies();
    }

    public void ResetMummies()
    {
        // Debug.Log("Reset Mummies");

        // Reset activeMummyCount
        activeMummyCount = startingMummyAmount;

        // Get all old (currently in the scene) mummies
        AIMummy[] mummies = FindObjectsOfType<AIMummy>();

        // Destroy all old mummies
        foreach (AIMummy mummy in mummies)
        {
            mummy.Die(false);  // Ensure old mummies are destroyed before respawning
        }

        // Now spawn new mummies
        for (int i = 0; i < startingMummyAmount; i++)
        {
            Respawn();
        }

        // Debug.Log("Mummies reset completed");
    }

    public IEnumerator TriggerDelayedRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        Respawn();
    }

    private void Respawn()
    {
        if (UM != null && UM.GetTimeRemaining() <= 0)
        {
            Debug.Log("Game over - Don't respawn mummies");
            return;
        }

        if (livingMummyCount >= activeMummyCount)
        {
            Debug.Log("Living mummy count already at activeMummyCount - Don't respawn");
            return;
        }

        Debug.Log("Respawn mummy");

        // Determine the location to spawn based on alternating positions
        if (usePosition1)
        {
            // Spawn in position 1
            Instantiate(mummyPrefab, spawnPosition1, Quaternion.identity);
        } else
        {
            // Spawn in position 2
            Instantiate(mummyPrefab, spawnPosition2, Quaternion.identity);
        }

        // Alternate next spawn position
        usePosition1 = !usePosition1;

        livingMummyCount++;
    }

    public void DecrementLivingMummyCount()
    {
        livingMummyCount--;
    }
    
    public void CurseMummySpawn(Vector3 position)
    {
        // Debug.Log("ActiveMummyCount: " + activeMummyCount + ", maxMummyCount: " + maxMummyCount);
        if (activeMummyCount < maxMummyCount)
        {
            // Debug.Log("Curse - Spawning Mummy");
            activeMummyCount++;
            GameObject mummyInstance = Instantiate(mummyPrefab, position, Quaternion.identity);
            mummyInstance.GetComponent<AIMummy>().isCursed = true;
            livingMummyCount++;
        }
    }
}
