using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameplayManager GM;
    [SerializeField] private float spawnTimer = 0;
    [SerializeField] private float thisSpawnTime = 10;
    [SerializeField] private float carOffset = 2.0f;
    [SerializeField] private int flashNumber = 3;
    [SerializeField] private float timeBetweenFlashes = .5f;
    //[SerializeField] private float spawnDistanceX = -15;
    //[SerializeField] private float spawnDistanceZ = 0;
    [SerializeField] private GameObject leftField;
    [SerializeField] private GameObject rightField;
    [SerializeField] private GameObject leftWarning;
    [SerializeField] private GameObject rightWarning;

    private bool onCooldown = false;

    void Start()
    {
        spawnTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.IsPlayingGet())
        {
            if (!onCooldown)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= thisSpawnTime)
                {
                    bool spawnLeft = Random.Range(0, 2) == 0;

                    Vector3 carPosition;
                    GameObject warningLight;
                    if (spawnLeft)
                    {
                        carPosition = leftField.transform.position - new Vector3(0, 0, Random.Range(-carOffset, carOffset));
                        warningLight = leftWarning;
                    }
                    else
                    {
                        carPosition = rightField.transform.position - new Vector3(0, 0, carOffset);
                        warningLight = rightWarning;
                    }
                    StartCoroutine(FlashWarningAndSpawn(warningLight, carPosition, spawnLeft));
                    spawnTimer = 0;
                }
            }
        } else
        {
            spawnTimer = 0;
        }
    }

    private IEnumerator FlashWarningAndSpawn(GameObject warningLight, Vector3 carPosition, bool spawnLeft)
    {
        onCooldown = true;
        for (int i = 0; i < flashNumber; i++)
        {
            warningLight.SetActive(true);
            yield return new WaitForSeconds(timeBetweenFlashes);
            warningLight.SetActive(false);
            yield return new WaitForSeconds(timeBetweenFlashes);
        }
        GameObject tuk = Instantiate(carPrefab, carPosition, Quaternion.identity);
        tuk.GetComponent<Car>().isLeft = spawnLeft;
        yield return new WaitForSeconds(5); //5 seconds is arbiturary for the destruction time.
        onCooldown = false;
    }
}