using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    public int health = 3;
    public BoxCollider BC;
    public Vector3 scale;
    public GameObject[] healthSprites;
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
            try
            {
                healthSprites[health].SetActive(false);
            }
            catch
            {
                Debug.LogWarning("Tried to access healthsprite out of bounds of array");
            }
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
            for (int i = 0; i < healthSprites.Length; i++)
            {
                healthSprites[i].SetActive(true);
            }
        } else
        {
            BC.enabled = false;
            transform.localScale = Vector3.zero;
        }
    }
}
