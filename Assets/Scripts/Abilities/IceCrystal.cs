using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCrystal : MonoBehaviour
{
    private DeleteAfterDelay DAD;
    public float stunTime = 1f;
    public float radius = 1f;
    public MonsterController MC;

    public GameObject attackVisualizer;

    public void Awake()
    {
        attackVisualizer.SetActive(false);
        DAD = GetComponent<DeleteAfterDelay>();
    }

    public void Echo()
    {
        DAD.NewTimer(1f);
        attackVisualizer.SetActive(true);
        attackVisualizer.transform.localScale *= radius;

        // Schedule the deactivation after 1 second
        Invoke(nameof(DeactivateVisualizer), 1f);

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider obj in objectsInRange)
        {
            // Debug.Log("Objects in range of howl:" + obj.name);
            // Check for ball
            if (obj.GetComponent<BallProperties>() != null)
            {
                // Stop ball
                try
                {
                    MC.BP.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    MC.BP.GetComponent<Rigidbody>().rotation = Quaternion.identity;
                    // Debug.Log("Ball stopped by howl");
                }
                catch
                {
                    Rigidbody ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallProperties>().GetComponent<Rigidbody>();
                    ball.velocity = Vector3.zero;
                    ball.rotation = Quaternion.identity;
                }
            }

            // Check for warriors
            if (obj.GetComponent<WarriorController>() != null)
            {
                // Stun warrior
                obj.GetComponent<WarriorController>().Stun(stunTime);
                // Debug.Log("Stunned Warrior: " + obj.name);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        WarriorController WC = other.gameObject.GetComponent<WarriorController>();
        if (WC != null && WC.isSliding)
        {
            DAD.Kill();
        }
    }

    private void DeactivateVisualizer()
    {
        attackVisualizer.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the object's position with the radius of howlRadius
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
