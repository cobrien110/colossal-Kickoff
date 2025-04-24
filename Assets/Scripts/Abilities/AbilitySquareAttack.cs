using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitySquareAttack : AbilityChargeable
{
    [Header("Attack Stats")]
    public float attackRange = 1f;
    public float attackBaseRadius = 1f;
    public float attackBaseSize = 1f;

    [Header("Ability Specific Variables")]
    public float attackVisualOffsetY;
    public LayerMask affectedLayers;
    public bool canSpawnProjectile = true;
    public GameObject projectilePrefab;
    public int projectileDamage = 1;
    public float projectileSpeed = 450f;
    public Mesh M;
    private AbilityQuetzPassive AQP;
    [SerializeField] private SpriteRenderer chargeOrb;
    [SerializeField] private GameObject attackParticles;
    [SerializeField] private GameObject attackChargeParticles;

    public override void Activate()
    {
        if (!canActivate) return;

        Debug.Log("Attack!");
        if (BP != null && BP.ballOwner != gameObject && GM.isPlaying)
        {
            Debug.Log("Performing Attack");
            Debug.Log(chargeAmount);
            //Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
            //Collider[] colliders = Physics.OverlapSphere(origin + transform.forward * attackRange, attackBaseRadius + chargeAmount * chargeRate, affectedLayers);

            Vector3 direction = transform.forward;
            Vector3 size = new Vector3(attackBaseSize + chargeAmount * chargeRate, 0.05f, attackBaseSize + chargeAmount * chargeRate);
            Vector3 origin = new Vector3(transform.position.x + direction.x * (attackRange + (chargeAmount * chargeRate)),
            transform.position.y + direction.y + attackVisualOffsetY,
            (transform.position.z + direction.z * (attackRange + (chargeAmount * chargeRate))));
            Collider[] colliders = Physics.OverlapBox(origin, size / 2, transform.rotation * Quaternion.Euler(0, 90, 0), affectedLayers);

            foreach (Collider col in colliders)
            {
                // Handle collision with each collider
                Debug.Log("SquareCast hit " + col.gameObject.name);
                if (col.gameObject.CompareTag("Warrior"))
                {
                    WarriorController WC = col.GetComponent<WarriorController>();
                    if (!WC.isInvincible)
                        WC.Die();
                    //Temp code for Quetz to get bigger on kill
                    AbilitySnakeSegments ASS = this.gameObject.GetComponent<AbilitySnakeSegments>();
                    if (ASS != null)
                    {
                        ASS.AddSegment();
                    }
                    else
                        Debug.Log("Warrior is invincible");
                }
                if (col.gameObject.CompareTag("Ball") && BP.ballOwner == null)
                {
                    // SWIPE AWAY BALL - UNUSED FOR NOW
                    if (canHitBall)
                    {
                        Debug.Log("AXE HIT BALL!");
                        float kickForce = attackHitForce;
                        Vector3 posA = new Vector3(BP.gameObject.transform.position.x, 0f, BP.gameObject.transform.position.z);
                        Vector3 posB = new Vector3(transform.position.x, 0f, transform.position.z);
                        Vector3 dir = (posA - posB).normalized;
                        Vector3 forceToAdd = dir * kickForce;
                        BP.GetComponent<Rigidbody>().AddForce(forceToAdd);
                    }
                }
                if (col.gameObject.CompareTag("Bomb"))
                {
                    Debug.Log("hitBomb");
                    AQP.counterAmount++;
                    SnakeBomb SB = col.GetComponent<SnakeBomb>();
                    AbilitySnakeMines ASM = GetComponent<AbilitySnakeMines>();
                    ASM.ExplodeSpecificBomb(SB);
                }
                DamagePlayer magmaPool = col.gameObject.GetComponent<DamagePlayer>();
                if (magmaPool != null && canSpawnProjectile)
                {
                    SpawnShrapnel(magmaPool.transform.position);
                    Destroy(magmaPool.gameObject);
                }
            }

            if (chargeAmount < maxChargeSeconds)
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttack"), 0.7f);
            }
            else
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttackCharged"), 0.7f);
                if (canSpawnProjectile) SpawnShrapnel(transform.position);
            }
            timer = 0;
            chargeAmount = 0;
            isCharging = false;
            ANIM.Play(activatedAnimationName);

            StartCoroutine(MC.MoveDelay());
        }
    }

    private void LateUpdate()
    {
        if (chargeOrb == null) return;
        if (isCharging)
        {
            chargeOrb.enabled = true;
        } else
        {
            chargeOrb.enabled = false;
        }
    }

    void SpawnShrapnel(Vector3 point)
    {
        if (projectilePrefab == null) return;
        Vector3 pos = new Vector3(point.x, point.y, point.z);
        GameObject shrap = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
        WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
        WS.damage = projectileDamage;
        WS.speed = projectileSpeed;
    }

    public override void ResizeAttackVisual()
    {
        if (attackVisualizer == null) return;

        // Calculate the new size of the visual
        float newLength = attackBaseSize + chargeAmount * chargeRate * 2f;
        attackVisualizer.transform.localScale = new Vector3(1f, 0.05f, newLength);

        //Vector3 dir = transform.forward * attackRange;
        Vector3 direction = transform.forward;
        Vector3 origin = new Vector3(transform.position.x + direction.x * (attackRange + (chargeAmount * chargeRate)),
            transform.position.y + direction.y + attackVisualOffsetY,
            (transform.position.z + direction.z * (attackRange + (chargeAmount * chargeRate))));
        attackVisualizer.transform.position = origin;

        // Hide the visual if the monster owns the ball
        if (BP.ballOwner != null && BP.ballOwner.Equals(gameObject))
        {
            attackVisualizer.transform.localScale = Vector3.zero;
        }
    }



    private void OnDrawGizmos()
    {
        if (attackVisualizer == null) return;
        Vector3 direction = transform.forward;
        Gizmos.color = Color.red;
        //Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
        //Gizmos.DrawWireSphere(origin + direction * attackRange, attackBaseRadius + chargeAmount * chargeRate);

        Vector3 origin = new Vector3(transform.position.x + direction.x * (attackRange + (chargeAmount * chargeRate)), 
            transform.position.y + direction.y + attackVisualOffsetY,
            (transform.position.z + direction.z * (attackRange + (chargeAmount * chargeRate))));
        Vector3 size = new Vector3(attackBaseSize + chargeAmount * chargeRate * 2f, 0.05f, attackBaseSize);
        Quaternion rot = transform.rotation * Quaternion.Euler(0, 90, 0);

        Mesh boxMesh = attackVisualizer.GetComponent<MeshFilter>().GetComponent<Mesh>();
        Gizmos.DrawWireMesh(M, 0, origin, rot, size);
        //Gizmos.DrawWireCube(origin, size);
    }

    private void Start()
    {
        Setup();
        M = attackVisualizer.GetComponent<MeshFilter>().mesh;
        AQP = GetComponent<AbilityQuetzPassive>();
    }
}
