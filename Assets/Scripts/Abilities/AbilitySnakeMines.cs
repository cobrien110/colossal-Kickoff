using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySnakeMines : AbilityScript
{
    private AbilitySnakeSegments ASS;
    public float radius = 1f;
    public float delayBeforeExplosion = 0.1f;
    public Vector3 centerOffset = new Vector3(0f, -.25f, 0f);
    public bool willTeleport = false;
    public GameObject teleEffect;
    public Transform slamParticlesTransform;
    public float telePortTime = 1f;
    private bool hasTeleported = false;
    public bool willPushBall = false;
    public float ballPushForce = 50f;

    public string soundName;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        ASS = GetComponent<AbilitySnakeSegments>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
    }

    public override void Activate()
    {
        if (!canActivate)
        {
            return;
        }

        if (willTeleport && !MC.isIntangible && timer <= telePortTime && !hasTeleported)
        {
            transform.position = ASS.GetTailPosition();
            ASS.RebuildSegments();
            hasTeleported = true;

            if (teleEffect != null)
            {
                GameObject particleInstance = Instantiate(teleEffect, slamParticlesTransform.position, Quaternion.Euler(90, 0, 0), transform);
                Vector3 originalScale = particleInstance.transform.localScale;
                particleInstance.transform.localScale = originalScale * 1.5f;
            }
        }

        if (timer >= cooldown)
        {

            if (ASS.cutSegments.Count >= 1)
            {
                timer = 0;
                hasTeleported = false;
                for (int i = ASS.cutSegments.Count - 1; i >= 0; i--)
                {
                    SnakeBomb bombToDestroy = ASS.cutSegments[i].GetComponent<SnakeBomb>();
                    bombToDestroy.radius = radius;
                    bombToDestroy.pushForce = ballPushForce;
                    bombToDestroy.delay = delayBeforeExplosion;
                    bombToDestroy.centerOffset = centerOffset;
                    ASS.cutSegments.RemoveAt(i);
                    bombToDestroy.PrimeExplosion(willPushBall);
                    bombToDestroy.ResetObjectsInRadius();
                }
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), 0.85f);
                ST.UpdateMAbUsed();
                UM.UpdateMonsterAbilitiesSB();
            }
            
        }

        
    }

    public void ExplodeSpecificBomb(SnakeBomb SB)
    {
        ASS.cutSegments.Remove(SB.gameObject);
        SB.radius = radius;
        SB.pushForce = ballPushForce;
        SB.delay = delayBeforeExplosion;
        SB.centerOffset = centerOffset;
        SB.PrimeExplosion(willPushBall);
    }

    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the object's position with the radius
        if (ASS == null) return;
        if (ASS.cutSegments == null) return;
        for (int i = 0; i < ASS.cutSegments.Count; i++)
        {
            Vector3 pos = ASS.cutSegments[i].transform.position + centerOffset;
            Gizmos.DrawWireSphere(pos + centerOffset, radius);
        }      
    }
}
