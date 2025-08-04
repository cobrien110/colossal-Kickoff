using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWarriorController : MonoBehaviour
{
    WarriorController wc;
    Rigidbody rb;
    //[SerializeField] Vector3 targetPoint = Vector3.zero;

    [Header("Roam Variables")]
    [SerializeField] private bool shouldRoam = false;
    [SerializeField] List<Transform> targetPoints = new List<Transform>();
    [SerializeField] private float waitTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        wc = GetComponent<WarriorController>();
        rb = GetComponent<Rigidbody>();
        wc.GM = new GameplayManager();
        wc.GM.isPlaying = true;
        wc.GM.isPaused = false;
        wc.GM.isGameOver = false;
        wc.GM.overtimeStarted = false;
        wc.GM.hasScored = false;
        wc.GM.automaticAISpawn = false;
        wc.GM.automaticStart = false;
        wc.GM.barriersAreOn = false;
        wc.GM.usePlayerPrefs = false;
        if (shouldRoam) StartCoroutine(Roam());
        //targetLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"transform.position: {transform.position}, targetPoint: {targetPoint}, distance: {Vector3.Distance(transform.position, targetPoint)}");
        //if (Vector3.Distance(transform.position, targetPoint) > 0.1f) 
        //{
        //    Vector3 dirToTarget = (targetPoint - transform.position).normalized;
        //    BaseMovement(new Vector2(dirToTarget.x, dirToTarget.z));
        //} else
        //{
        //    wc.movementDirection = Vector3.zero;
        //}

        //PerformMovement();
    }

    private IEnumerator Roam()
    {
        int i = 0;
        while (true)
        {
            Vector3 targetPoint = targetPoints[i].position; // Temporary code. Should choose next point
            while (Vector3.Distance(transform.position, targetPoint) > 0.1f)
            {
                Vector3 dirToTarget = (targetPoint - transform.position).normalized;
                BaseMovement(new Vector2(dirToTarget.x, dirToTarget.z));
                yield return null;
            }

            // targetPoint reached

            wc.movementDirection = Vector3.zero;

            if (i < targetPoints.Count - 1)
            {
                i++;
            } else
            {
                i = 0;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    void BaseMovement(Vector2 targetDir)
    {
        if (wc.isSliding) return;

        if (targetDir != Vector2.zero)
        {
            //usingKeyboard = true;
            wc.movementDirection = new Vector3(targetDir.x, 0, targetDir.y).normalized;
            wc.aimingDirection = wc.movementDirection;
            Debug.Log("MovementDirection: " +  wc.movementDirection + ", targetPos: " + targetDir);
        }


        //rb.velocity = GM.isPlaying ? wc.movementDirection * wc.warriorSpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        //if (rb.velocity != Vector3.zero)
        //{
        //    Quaternion newRotation = Quaternion.LookRotation(wc.movementDirection.normalized, Vector3.up);
        //    transform.rotation = newRotation;
        //}

        if (wc.movementDirection != Vector3.zero && !wc.GetIsDead())
        {
            wc.ANIM.SetBool("isWalking", true);
        }
        else
        {
            wc.ANIM.SetBool("isWalking", false);
        }

    }
}
