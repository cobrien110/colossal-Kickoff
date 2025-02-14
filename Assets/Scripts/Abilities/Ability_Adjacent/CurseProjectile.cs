using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseProjectile : MonoBehaviour
{
    public float speed = 300f;
    //public float spreadHori = 30f;
    //public int damage = 1;
    //public int index = 0;
    //public float spreadVert = 15f;
    private Rigidbody rb;

    //List<WarriorController> hitPlayers = new List<WarriorController>();

    //bool hasCollided = false;

    // Start is called before the first frame update
    void Start()
    {
        //float hSpread = Random.Range(-spreadHori, spreadHori);
        //float vSpread = Random.Range(-spreadVert, spreadVert);
        //transform.Rotate(0f, hSpread, 0f);
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * speed);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localScale = transform.localScale * 0.975f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Warrior"))
        {
            Debug.Log("Curse Hit Warrior");
            WarriorController WC = other.GetComponent<WarriorController>();
            if (WC.isInvincible) return;
            WC.isCursed = true;
            WC.BecomeBomb();
            //hitPlayers.Add(WC);
            Destroy(gameObject);
        }
    }
}
