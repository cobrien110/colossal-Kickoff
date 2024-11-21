using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrb : MonoBehaviour
{
    public int damage = 1;
    public float sizeScaler = 0.05f;
    private Vector3 baseScale;
    public Rigidbody RB;
    [HideInInspector] GameObject closestShrine;
    private float timeBetweenChecks = 0.25f;
    public float driftSpeed = 1f;
    [HideInInspector] public bool launchable = false;
    public float timeBeforeCanBeLaunched = 1f;
    public float launchSpeed;
    public Material monMat;
    public Material warMat;
    public GameObject innerOrb;
    private MeshRenderer MR;
    public bool isMonsterTeam = true;

    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
        RB = GetComponent<Rigidbody>();
        Invoke("CheckShrines", timeBetweenChecks);
        Invoke("SetLaunchable", timeBeforeCanBeLaunched);
        MR = innerOrb.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Make smaller over time
        if (transform.localScale.magnitude > baseScale.magnitude * .5f) transform.localScale -= Vector3.one * Time.deltaTime * sizeScaler;

        // float towards closest object
        // Calculate the direction towards the target
        if (closestShrine == null) return;
        Vector3 direction = (closestShrine.transform.position - transform.position).normalized;

        // Apply a force towards the target
        if (launchable) RB.AddForce(direction * driftSpeed, ForceMode.Force);

        // if close enough to shrine, launch again
        float distance = Vector3.Distance(transform.position, closestShrine.transform.position);
        if (!launchable || distance > 0.3f) return;
        Launch(GetRandomLaunchForce());
        //SetTeam(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        WarriorController WC = other.GetComponent<WarriorController>();

        if (isMonsterTeam && WC != null) // if monster controlled
        {
            if (!WC.isSliding) // and warrior not sliding
            {
                // hurt warrior and destroy
                WC.DamageWithInstantInvincibility(damage);
                Destroy(gameObject);
            } else // else swap teams
            {
                Vector3 dir = (transform.position - WC.transform.position).normalized;
                dir.y = 0f;
                Launch(dir * launchSpeed);
                SetTeam(false);
            }
        }

        // if on warrior team and hit monster, convert it
        SoulOrb SO = other.GetComponent<SoulOrb>();
        if (SO == null) return;
        if (!isMonsterTeam && SO.isMonsterTeam)
        {
            SO.Launch(GetRandomLaunchForce());
        }
    }

    public void Launch(Vector3 force)
    {
        Debug.Log("Rigidbody: " + RB);
        if (RB == null) return;

        RB.velocity = Vector3.zero;

        Debug.Log("Adding force: " + force);
        RB.AddForce(force, ForceMode.Impulse);

        launchable = false;
        Invoke("SetLaunchable", timeBeforeCanBeLaunched);
    }

    public Vector3 GetRandomLaunchForce()
    {
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 force = launchSpeed * dir;
        return force;
    }

    private void CheckShrines()
    {
        GameObject[] shrines = GameObject.FindGameObjectsWithTag("Shrine");
        for (int i = 0; i < shrines.Length; i++)
        {
            if (closestShrine == null)
            {
                closestShrine = shrines[i];
            }
            else
            {
                if (Vector3.Distance(transform.position, shrines[i].transform.position) <
                    Vector3.Distance(transform.position, closestShrine.transform.position))
                {
                    closestShrine = shrines[i];
                }
            }
        }

        Invoke("CheckShrines", timeBetweenChecks);
    }

    private void SetLaunchable()
    {
        launchable = true;
    }

    public void SetTeam(bool b)
    {
        isMonsterTeam = b;
        if (b)
        {
            MR.material = monMat;
        } else
        {
            MR.material = warMat;
        }
    }
}
