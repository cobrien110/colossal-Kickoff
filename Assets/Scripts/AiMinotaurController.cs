using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMinotaurController : AiMonsterController
{
    // Ability Order
        // Basic Attack - 1
        // Dash - 2
        // Wall - 3

    protected override void PerformAbility1Chance(float chargeAmount)
    {
        // Debug.Log("PerformAbility1Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability1Chance) mc.abilities[0].Activate();
    }

    protected override void PerformAbility2Chance(float chargeAmount)
    {
        // Debug.Log("PerformAbility2Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability2Chance) mc.abilities[1].Activate();
    }

    protected override void PerformAbility3Chance(float chargeAmount)
    {
        // Debug.Log("PerformAbility3Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability3Chance) mc.abilities[2].Activate();
    }

    protected override void PerformKickChance(float chargeAmount)
    {
        // Debug.Log("PerformKickChance. chargeAmount: " + chargeAmount);
        if (Random.value < kickChance) Kick();
    }

    /*
     * Decides the overall logic for the AiMonster.
     * Accounts for movement of monster, and chances
     * that monster will perform any given ability or if
     * it will kick at any given moment, based on state of
     * the ball (who possesses it, if anyone) and other factors
     * (such as proximity between warriors and monster, where each
     * are on the field, etc
     */
    protected override void MonsterBehaviour()
    {
        // Debug.Log("MonsterBehaviour");

        // If no one has ball...
        if (mc.BP.ballOwner == null)
        {
            // Logic
            BallNotPossessed();
        }
        // If I have ball...
        else if (mc.BP.ballOwner == gameObject)
        {
            // Logic
            MonsterHasBall();
        }
        // If warrior has ball...
        else if (mc.BP.ballOwner.GetComponent<WarriorController>() != null)
        {
            // Logic
            WarriorHasBall();
        }
        else
        {
            Debug.Log("Error in MonsterBehaviour logic");
        }
    }

    private void WarriorHasBall()
    {
        // Debug.Log("WarriorHasBall");
    }

    private void MonsterHasBall()
    {
        // Debug.Log("MonsterHasBall");
    }

    private void BallNotPossessed()
    {
        // Debug.Log("BallNotPossessed");
    }

    protected override void Kick()
    {
        if (mc.BP.ballOwner == gameObject)
        {
            Debug.Log("Kick!");
            mc.BP.ballOwner = null;
            // Debug.Log(transform.forward);
            mc.BP.GetComponent<Rigidbody>().AddForce(transform.forward * aiKickSpeed);
            //audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
        }
    }

    void MoveTo(Vector2 targetPos)
    {
        // Debug.Log("MoveTo: " + targetPos);
        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            mc.movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            mc.aimingDirection = mc.movementDirection;
        }


        rb.velocity = GM.isPlaying ? mc.movementDirection * mc.monsterSpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(mc.movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (mc.movementDirection != Vector3.zero && GM.isPlaying)
        {
            mc.ANIM.SetBool("isWalking", true);
        }
        else
        {
            mc.ANIM.SetBool("isWalking", false);
        }

    }

    private void FixedUpdate()
    {
        // Decides movement behaviour,
        // Also decides chances that abilities and kicking will occur at any given moment
        MonsterBehaviour();

        // Chance to perform abilities or kick
        PerformAbility1Chance(0);
        PerformAbility2Chance(0);
        PerformAbility3Chance(0);
        PerformKickChance(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets up variable assignments for this gameobject
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure mc.BP is assigned a value
        if (mc.BP == null)
        {
            mc.BP = FindObjectOfType<BallProperties>();
        }
    }

}
