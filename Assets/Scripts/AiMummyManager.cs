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
    [SerializeField] private GameObject mummyPrefab;
    [SerializeField] private float respawnDelay;
    [SerializeField] private int startingMummyAmount = 2;

    // Start is called before the first frame update
    void Awake()
    {
        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        spawnPosition1 = new Vector3(monsterGoal.transform.position.x + spawnPositionOffsetX, 0f, spawnPositionOffsetZ);
        spawnPosition2 = new Vector3(monsterGoal.transform.position.x + spawnPositionOffsetX, 0f, -spawnPositionOffsetZ);
    }

    public void ResetMummies()
    {
        // Debug.Log("Reset Mummies");

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
    }
}
