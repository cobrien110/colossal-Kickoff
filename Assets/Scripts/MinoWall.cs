using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoWall : MonoBehaviour
{
    public Transform startPt;
    public Transform endPt;
    public GameObject shrapnelPrefab;
    public int numOfShrapnel;
    public int shrapnelDamage = 1;
    public float shrapnelSpeed = 500f;
    public float yOffset = .5f;
    public float speed = 1f;
    private float startTime;
    private float journeyLength;
    private MonsterController MC;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPt.position;
        startTime = Time.time;
        MC = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>();
        numOfShrapnel = MC.shrapnelAmount;
        shrapnelDamage = MC.shrapnelDamage;
        shrapnelSpeed = MC.shrapnelSpeed;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPt.position, endPt.position);
    }

    // Update is called once per frame
    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(transform.position, endPt.position, fractionOfJourney);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z);
            for (int i = 0; i < numOfShrapnel; i++)
            {
                GameObject shrap = Instantiate(shrapnelPrefab, pos, Quaternion.LookRotation(MC.movementDirection, Vector3.up));
                WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
                WS.damage = shrapnelDamage;
                WS.speed = shrapnelSpeed;
            }
            AudioPlayer aud = other.gameObject.GetComponent<AudioPlayer>();
            aud.PlaySoundRandomPitch(aud.Find("minotaurWallSmash"));
            Destroy(this.gameObject);
        }
    }
}