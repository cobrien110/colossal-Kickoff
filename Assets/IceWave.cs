using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWave : MonoBehaviour
{
    MonsterController MC;
    public float launchForce = 5f;
    private Rigidbody RB;
    public float moveSpeed = 5f;
    public bool isReturning = true;
    public float rotationSpeed;
    public float maxSpeed = 5f;
    public float moveSpeedIncreasePerSecond = 10f;
    public float timeBeforeRotLock = 3f;
    // Start is called before the first frame update
    void Start()
    {
        MC = GameObject.FindAnyObjectByType<MonsterController>();
        RB = GetComponent<Rigidbody>();
        LaunchAway();
        Invoke("RotLock", timeBeforeRotLock);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReturning) MoveTowardsMonster();
        else RB.AddForce(moveSpeed * transform.forward * Time.deltaTime, ForceMode.Force);
        moveSpeed += Time.deltaTime * moveSpeedIncreasePerSecond;

        if (RB.velocity.magnitude > maxSpeed) RB.velocity = RB.velocity.normalized * maxSpeed;
    }

    private void LaunchAway()
    {
        Vector3 yAdjustedMCPos = new Vector3(MC.transform.position.x, transform.position.y, MC.transform.position.z);
        Vector3 dir = (transform.position - yAdjustedMCPos).normalized;
        Vector3 forceToAdd = dir * launchForce;
        RB.AddForce(forceToAdd, ForceMode.Impulse);
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private void MoveTowardsMonster()
    {
        Vector3 yAdjustedMCPos = new Vector3(MC.transform.position.x, transform.position.y, MC.transform.position.z);
        Vector3 dir = (yAdjustedMCPos - transform.position).normalized;
        Vector3 forceToAdd = dir * (moveSpeed * Time.deltaTime);
        RB.AddForce(forceToAdd, ForceMode.Force);
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(RB.velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReturning) return;
        if (other.GetComponent<MonsterController>())
        {
            isReturning = false;
        }
    }

    private void RotLock()
    {
        isReturning = false;
    }
}
