using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoWall : MonoBehaviour
{
    /*public Transform startPt;
    public Transform endPt;
    public GameObject shrapnelPrefab;
    public int numOfShrapnel;
    public int shrapnelDamage = 1;
    public float shrapnelSpeed = 500f;
    public float shrapnelSpawnDegrees = 35f;
    public float yOffset = .5f;
    public float speed = 1f;
    public float duration = 8f;
    private float startTime;
    private float journeyLength;
    private MonsterController MC;
    private bool movingBack = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPt.position;
        startTime = Time.time;
        MC = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>();
        numOfShrapnel = MC.shrapnelAmount;
        shrapnelDamage = MC.shrapnelDamage;
        shrapnelSpeed = MC.shrapnelSpeed;
        duration = MC.wallDuration;
        shrapnelSpawnDegrees = MC.shrapnelSpreadAngle;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPt.position, endPt.position);
        StartCoroutine("Swap", duration);
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
        if (other.CompareTag("Monster") && !movingBack)
        {
            SpawnProjectiles();
            AudioPlayer aud = other.gameObject.GetComponent<AudioPlayer>();
            aud.PlaySoundVolumeRandomPitch(aud.Find("minotaurWallSmash"), 0.35f);
            Destroy(this.gameObject);
        }
    }

    public void MoveBackToGround()
    {
        startTime = Time.time;
        Transform temp = endPt;
        endPt = startPt;
        startPt = temp;
        movingBack = true;
    }

    IEnumerator Swap()
    {
        yield return new WaitForSeconds(duration);
        MoveBackToGround();
    }

    private void SpawnProjectiles()
    {
        float angleIncrement = shrapnelSpawnDegrees / (numOfShrapnel - 1);
        float startAngle = -shrapnelSpawnDegrees / 2; // Start angle of the spread

        Vector3 pos = new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z);
        for (int i = 0; i < numOfShrapnel; i++)
        {
            GameObject shrap = Instantiate(shrapnelPrefab, pos, Quaternion.LookRotation(MC.movementDirection, Vector3.up));
            WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
            WS.damage = shrapnelDamage;
            WS.speed = shrapnelSpeed;

            // Calculate the angle for this projectile
            float angle = startAngle + (angleIncrement * i);

            shrap.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * shrap.transform.rotation;
        }
    }*/
}
