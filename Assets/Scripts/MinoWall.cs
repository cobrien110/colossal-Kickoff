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
    public float shrapnelSpawnDegrees = 35f;
    public float chargeShrapnelMult = 2;
    public float yOffset = .5f;
    public float speed = 1f;
    public float duration = 8f;
    public LayerMask players;
    private float startTime;
    private float journeyLength;
    private AbilityMinotaurWall ABW;
    private MonsterController MC;
    private bool movingBack = false;
    private BoxCollider BC;

    // Start is called before the first frame update
    void Start()
    {
        BC = GetComponent<BoxCollider>();
        transform.position = startPt.position;
        startTime = Time.time;
        ABW = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilityMinotaurWall>();
        MC = ABW.MC;
        numOfShrapnel = ABW.shrapnelAmount;
        shrapnelDamage = ABW.shrapnelDamage;
        shrapnelSpeed = ABW.shrapnelSpeed;
        duration = ABW.wallDuration;
        shrapnelSpawnDegrees = ABW.shrapnelSpreadAngle;
        chargeShrapnelMult = ABW.chargeShrapnelMult;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPt.position, endPt.position);
        StartCoroutine("Swap", duration);

        if (Physics.Raycast(transform.position, Vector3.up, players)) {
            BC.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!BC.enabled)
        {
            if (Physics.Raycast(transform.position, Vector3.up, players))
            {
                BC.enabled = false;
            } else
            {
                BC.enabled = true;
            }
        }
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(transform.position, endPt.position, fractionOfJourney);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") && !movingBack)
        {
            float mult = 1f;
            if (MC.isDashing || !MC.canBeStunned)
            {
                mult = chargeShrapnelMult;
                AbilityMinotaurBoost AMB = other.GetComponent<AbilityMinotaurBoost>();
                AMB.Activate();
            }
            SpawnShrapnel(mult);
            //AudioPlayer aud = GetComponent<AudioPlayer>();
            //aud.PlaySoundVolumeRandomPitch(aud.Find("minotaurWallSmash"), 0.35f);
            AudioPlayer aud2 = other.gameObject.GetComponent<AudioPlayer>();
            aud2.PlaySoundVolumeRandomPitch(aud2.Find("minotaurWallSmash"), 0.45f);
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

    private void SpawnShrapnel(float mult)
    {
        int num = numOfShrapnel * (int) mult;
        float a = shrapnelSpawnDegrees;
        if (mult > 1) a = 359;
        float angleIncrement = a / (num - 1);
        float startAngle = -a / 2; // Start angle of the spread

        Vector3 pos = new Vector3(transform.position.x, transform.position.y - yOffset, transform.position.z);
        for (int i = 0; i < num; i++)
        {
            GameObject shrap = Instantiate(shrapnelPrefab, pos, Quaternion.LookRotation(MC.movementDirection, Vector3.up));
            WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
            WS.damage = shrapnelDamage;
            WS.speed = shrapnelSpeed;

            // Calculate the angle for this projectile
            float angle = startAngle + (angleIncrement * i);

            shrap.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * shrap.transform.rotation;
        }
    }
}
