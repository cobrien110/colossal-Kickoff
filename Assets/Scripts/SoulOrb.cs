using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrb : MonoBehaviour
{
    public int damage = 1;
    public float sizeScaler = 0.05f;
    private Vector3 baseScale;

    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Make smaller over time
        if (transform.localScale.magnitude > baseScale.magnitude * .5f) transform.localScale -= Vector3.one * Time.deltaTime * sizeScaler;
    }

    private void OnTriggerEnter(Collider other)
    {
        WarriorController WC = other.GetComponent<WarriorController>();
        if (WC != null)
        {
            WC.DamageWithInstantInvincibility(damage);
            Destroy(gameObject);
        }
    }
}
