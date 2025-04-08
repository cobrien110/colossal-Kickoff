using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveHazard : MonoBehaviour
{
    [SerializeField] private BoxCollider BC;
    [SerializeField] private Animator Ani;

    [SerializeField] private MeshRenderer Body;
    [SerializeField] private MeshRenderer[] pieces;

    [SerializeField] private AnimationClip clip;

    public float secondsTillReform = 15f;
    private float clipLength;
    private Coroutine Waiting;

    private void Start()
    {
        clipLength = clip.length;
        FlipRenderers(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallProperties BP = collision.gameObject.GetComponent<BallProperties>();
            if (BP.ballOwner != null) return;
            Deconstruct();
        }
    }

    private void Deconstruct()
    {
        BC.enabled = false;
        SetAnimatorBool(true);
        FlipRenderers(false);
        Waiting = StartCoroutine(WaitingPeriod());
    }

    IEnumerator WaitingPeriod()
    {
        yield return new WaitForSeconds(secondsTillReform);
        BC.enabled = true;
        SetAnimatorBool(false);
        yield return new WaitForSeconds(clipLength);
        FlipRenderers(true);
    }

    public void ResetGrave()
    {
        StopCoroutine(Waiting);
        BC.enabled = true;
        SetAnimatorBool(false); 
        FlipRenderers(true);
    }

    private void SetAnimatorBool(bool state)
    {
        Ani.SetBool("Break", state);
    }

    private void FlipRenderers(bool flipState)
    {
        Body.enabled = flipState;

        foreach (MeshRenderer renderer in pieces)
        {
            renderer.enabled = !flipState;
        }
    }

}
