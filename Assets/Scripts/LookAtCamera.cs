using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    //[SerializeField] private bool isPlayerSprite = false;
    void Start()
    {
        if (cam == null) cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if (isPlayerSprite) return;
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        transform.rotation = new Quaternion(0f, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        if (transform.parent == null) return;
        if (transform.parent.rotation.eulerAngles.y >= 180f)
        {
            //transform.rotation = new Quaternion(transform.rotation.x, -transform.rotation.y, transform.rotation.z, transform.rotation.w);
            transform.RotateAround(transform.position, transform.up, 180f);
        }
    }
}
