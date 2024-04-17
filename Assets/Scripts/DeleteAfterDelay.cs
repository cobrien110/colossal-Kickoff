using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterDelay : MonoBehaviour
{
    public float deathTimer = 5f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Kill", deathTimer);
    }

    void Kill()
    {
        Destroy(this.gameObject);
    }
}
