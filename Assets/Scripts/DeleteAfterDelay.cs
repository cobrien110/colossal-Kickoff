using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterDelay : MonoBehaviour
{
    public float deathTimer = 5f;
    private float counter;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Kill", deathTimer);
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }

    public void NewTimer(float time)
    {
        CancelInvoke();
        Invoke("Kill", time);
    }
}
