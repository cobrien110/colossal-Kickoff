using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour
{
    public GameObject dangerZoneIndicator;
    public float indicatorSpawnHeight = .5f;
    public float indicatorScaleIncrement = 0.05f;
    [SerializeField] private GameObject shardsPrefab;
    // Start is called before the first frame update
    void Start()
    {
        float dx = dangerZoneIndicator.transform.position.x;
        float dz = dangerZoneIndicator.transform.position.z;
        dangerZoneIndicator.transform.position = new Vector3(dx, indicatorSpawnHeight, dz);
        transform.DetachChildren();
    }

    // Update is called once per frame
    void Update()
    {
        dangerZoneIndicator.transform.localScale += new Vector3(indicatorScaleIncrement, 0f, indicatorScaleIncrement) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool willDestroy = false;
        if (other.CompareTag("Warrior"))
        {
            WarriorController WC = other.GetComponent<WarriorController>();
            WC.Damage(1);
            willDestroy = true;
        }
        if (other.CompareTag("Monster"))
        {
            MonsterController MC = other.GetComponent<MonsterController>();
            MC.Stun();
            willDestroy = true;
        }

        if (other.CompareTag("Indicator"))
        {
            Destroy(dangerZoneIndicator);
            willDestroy = true;
        }

        if (willDestroy)
        {
            Instantiate(shardsPrefab, transform.position - new Vector3(0,.5f,0), Quaternion.identity);
            Destroy(dangerZoneIndicator);
            Destroy(gameObject);
        }
    }
}
