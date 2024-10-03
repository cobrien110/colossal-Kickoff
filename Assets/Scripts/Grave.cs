using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    public int health = 3;
    public BoxCollider BC;
    public Vector3 scale;

    private void Start()
    {
        scale = transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallProperties BP = collision.gameObject.GetComponent<BallProperties>();
            if (BP.ballOwner != null) return;
            health--;
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void setStatus(bool isActive)
    {
        if (isActive)
        {
            health = 3;
            BC.enabled = true;
            transform.localScale = scale;
        } else
        {
            BC.enabled = false;
            transform.localScale = Vector3.zero;
        }
    }
}
