using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BallProperties BP = other.GetComponent<BallProperties>();
        if (BP != null && BP.isInteractable)
        {
            BP.ResetBall();
        }
    }
}
