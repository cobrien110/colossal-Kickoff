using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWarriorController : MonoBehaviour
{
    WarriorController wc;
    Rigidbody rb;
    [SerializeField] Vector3 targetLocation = Vector3.zero;
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
        targetLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"transform.position: {transform.position}, targetLocation: {targetLocation}, distance: {Vector3.Distance(transform.position, targetLocation)}");
        if (Vector3.Distance(transform.position, targetLocation) > 0.1f) 
        {
            BaseMovement(new Vector2(targetLocation.x, targetLocation.z));
        } else
        {
            wc.movementDirection = Vector3.zero;
        }

        //PerformMovement();
    }

    private void PerformMovement()
    {

        if (!wc.IsSliding()) rb.velocity = wc.movementDirection * wc.warriorSpeed;

        if (rb.velocity != Vector3.zero && !wc.IsSliding())
        {
            Quaternion newRotation = Quaternion.LookRotation(wc.movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (rb.velocity.magnitude < 1)
        {
            wc.movementDirection = Vector3.zero;
        }

        if (wc.movementDirection == Vector3.zero) wc.ANIM.SetBool("isWalking", false);
    }

    void BaseMovement(Vector2 targetPos)
    {
        if (wc.isSliding) return;

        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            wc.movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            wc.aimingDirection = wc.movementDirection;
            //Debug.Log("MovementDirection: " +  wc.movementDirection);
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
