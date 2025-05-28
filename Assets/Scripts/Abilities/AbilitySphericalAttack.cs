using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitySphericalAttack : AbilityChargeable
{
    [Header("Attack Stats")]
    public float attackRange = 1f;
    public float attackBaseRadius = 1f;

    [Header("Ability Specific Variables")]
    public float attackVisualOffsetY;
    public LayerMask affectedLayers;
    public bool canSpawnProjectile = true;
    public GameObject projectilePrefab;
    public GameObject attackParticles;
    public int projectileDamage = 1;
    public float projectileSpeed = 450f;

    [SerializeField] private float sphericalAttackPosY = -0.3f;

    new void Start()
    {
        base.Start();
        attackParticles = GM.SceneIM.GetSlamParticles();
    }

    public override void Activate()
    {
        if (!canActivate)
        {
            Debug.Log("canActivate: " + canActivate);
            return;
        }

        Debug.Log("Attack!");

        if (BP != null && BP.ballOwner != gameObject && GM.isPlaying)
        {
            Debug.Log("Performing Attack");
            Debug.Log(chargeAmount);
            Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
            Instantiate(attackParticles,  new Vector3(0, 0.216f, 0) + origin + transform.forward * attackRange, Quaternion.identity);
            Collider[] colliders = Physics.OverlapSphere(origin + transform.forward * attackRange, attackBaseRadius + chargeAmount * chargeRate, affectedLayers);
            

            foreach (Collider col in colliders)
            {
                // Handle collision with each collider
                Debug.Log("SphereCast hit " + col.gameObject.name);
                bool hitWarrior = false;

                if (col.gameObject.CompareTag("Warrior"))
                {
                    WarriorController WC = col.GetComponent<WarriorController>();
                    if (!WC.isInvincible)
                    {
                        WC.Die();
                        hitWarrior = true;
                    }
                        
                    //Temp code for Quetz to get bigger on kill
                    AbilitySnakeSegments ASS = this.gameObject.GetComponent<AbilitySnakeSegments>();
                    if (ASS != null)
                    {
                        ASS.AddSegment();
                    }
                    else
                        Debug.Log("Warrior is invincible");
                }
                if (col.gameObject.CompareTag("Ball") && BP.ballOwner == null && !hitWarrior)
                {
                    // SWIPE AWAY BALL
                    if (canHitBall)
                    {
                        Debug.Log("AXE HIT BALL!");
                        float kickForce = attackHitForce;
                        Vector3 posA = new Vector3(BP.gameObject.transform.position.x, 0f, BP.gameObject.transform.position.z);
                        Vector3 posB = new Vector3(transform.position.x, 0f, transform.position.z);
                        Vector3 dir = (posA - posB).normalized;
                        Vector3 forceToAdd = dir * kickForce;
                        BP.GetComponent<Rigidbody>().AddForce(forceToAdd);
                        Debug.Log("Setting ball previous kicker to monster");
                        BP.previousKicker = gameObject;
                        BP.playerTest = BP.previousKicker;
                    }
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

    void SpawnShrapnel(Vector3 point)
    {
        if (projectilePrefab == null) return;

        if (MC.GetComponent<AbilityMinotaurBoost>() != null && MC.GetComponent<AbilityMinotaurBoost>().counterAmount > 0)
        {
            float angleIncrement = 45 / (3 - 1);
            float startAngle = -45 / 2; // Start angle of the spread

            Vector3 pos = new Vector3(point.x, point.y, point.z);
            for (int i = 0; i < 3; i++)
            {
                GameObject shrap = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
                WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
                WS.damage = projectileDamage;
                WS.speed = projectileSpeed;

                // Calculate the angle for this projectile
                float angle = startAngle + (angleIncrement * i);

                shrap.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * shrap.transform.rotation;
            }
            
        } else
        {
            Vector3 pos = new Vector3(point.x, point.y, point.z);
            GameObject shrap = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
            WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
            WS.damage = projectileDamage;
            WS.speed = projectileSpeed;
        }
        
    }

    public override void ResizeAttackVisual()
    {
        if (attackVisualizer == null) return;
        attackVisualizer.transform.localScale = new Vector3(attackBaseRadius * 2f + chargeAmount * chargeRate * 2f,
            0.05f, attackBaseRadius * 2f + chargeAmount * chargeRate * 2f);
        Vector3 dir = transform.forward * attackRange;
        attackVisualizer.transform.position = new Vector3(transform.position.x, sphericalAttackPosY, transform.position.z) + dir;
        if (BP.ballOwner != null && BP.ballOwner.Equals(gameObject))
        {
            attackVisualizer.transform.localScale = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = transform.forward;
        Gizmos.color = Color.red;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
        Gizmos.DrawWireSphere(origin + direction * attackRange, attackBaseRadius + chargeAmount * chargeRate);
    }
}
