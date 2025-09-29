using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWarriorController : MonoBehaviour
{
    WarriorController wc;
    Rigidbody rb;
    AudioPlayer audioPlayer;
    //[SerializeField] Vector3 targetPoint = Vector3.zero;

    [Header("Roam Cycle Variables")]
    [SerializeField] private bool shouldRoamCycle = false;
    [SerializeField] private float roamWaitTime = 0;
    [SerializeField] List<Transform> targetPoints = new List<Transform>();

    [Space]

    [Header("Dodge Cycle Variables")]
    [SerializeField] private bool shouldDodgeCycle = false;
    [SerializeField] private float dodgeWaitTime = 0;
    [SerializeField] private SlideMode slideMode = SlideMode.Dodge;
    private enum SlideMode
    {
        Dodge,
        Regular
    }

    // Start is called before the first frame update
    void Start()
    {
        wc = GetComponent<WarriorController>();
        rb = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioPlayer>();
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
        if (shouldRoamCycle) StartCoroutine(RoamCycle());
        else if (shouldDodgeCycle) StartCoroutine(DodgeCycle());
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

    private IEnumerator DodgeCycle()
    {
        while (true)
        {
            // Dodge
            wc.movementDirection = new Vector3(1, 0, 0);
            Slide();
            wc.movementDirection = Vector3.zero;

            // Wait
            yield return new WaitForSeconds(dodgeWaitTime);
        }
    }

    private void Slide()
    {

        Debug.Log(gameObject.name + ": Sliding");
        wc.isSliding = true;
        wc.isInvincible = true;

        // Increase hitbox for sliding
        //capsuleCollider.radius *= slideHitboxRadius;

        // Check if this is a dodge slide or a regular slide
        float slideDuration;
        float slideSpeed;
        // AudioClip audioClip;
        string anim;
        ForceMode forceMode;
        if (slideMode == SlideMode.Dodge)
        {
            Debug.Log("Dodge slide");
            // Dodge slide
            slideDuration = wc.slideDurationDodge;
            slideSpeed = wc.slideSpeedDodge;
            forceMode = ForceMode.Force;
            //ANIM.SetBool("isJuking", true);
            //isJuking = true;

            // audioClip = ???
            anim = "isJuking";
            wc.ANIM.Play(anim);
        }
        else
        {
            Debug.Log("Regular slide");
            // Regular slide
            slideDuration = wc.slideDurationRegular;
            slideSpeed = wc.slideSpeedRegular;
            forceMode = ForceMode.Force;

            // audioClip = ???
            anim = "isSliding";
        }

        //Debug.Log("slideSpeed: " + slideSpeed);

        // Add force in direction of the player input for this warrior (movementDirection)
        Vector3 slideVelocity = wc.movementDirection.normalized * slideSpeed;
        rb.AddForce(slideVelocity, forceMode);
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("slide"), 0.5f); // Maybe replace argument with "audioClip" variable

        // Set isSliding to false after a delay
        Invoke("StopSliding", slideDuration);

        // If owner is kicking, if kicking
        //if (BP.ballOwner != null && BP.ballOwner == gameObject && kickCharge > 1)
        //{
        //    Debug.Log("Cancel Kick");

        //    // Cancel kick
        //    isCharging = false;
        //    kickCharge = 1;
        //    aimingDirection = Vector3.zero;
        //    WUI.UpdateChargeBar(0f); // Update ui
        //    rightStickInput = Vector3.zero;
        //    superKicking = false; // Reset superKicking if true

        //    // Disable aim input for a moment to allow the right stick to be released without causing a kick to occur
        //    canReadAimInput = false;
        //    Invoke("ResetCanReadAimInput", 0.45f);

        //}

        // Update the last slide time
        //lastSlideTime = Time.time;
        wc.ANIM.SetBool(anim, true); // Maybe replace argument with "anim" variable

    }

    private void StopSliding()
    {
        // Debug.Log("No longer sliding");
        wc.ANIM.SetBool("isSliding", false);
        wc.ANIM.SetBool("isJuking", false);
        // ANIM.SetBool("isDodging", false);
        //isSliding = false;
        //isJuking = false;
        //isInvincible = false;
        //capsuleCollider.radius = baseHitboxRadius;
    }

    private IEnumerator RoamCycle()
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
            }
            else
            {
                i = 0;
            }

            yield return new WaitForSeconds(roamWaitTime);
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
            Debug.Log("MovementDirection: " + wc.movementDirection + ", targetPos: " + targetDir);
        }

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
