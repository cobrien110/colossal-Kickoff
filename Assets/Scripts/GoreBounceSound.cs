using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoreBounceSound : MonoBehaviour
{
    public AudioSource AS;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            if (!AS.isPlaying) AS.Play();
        }
    }
}
