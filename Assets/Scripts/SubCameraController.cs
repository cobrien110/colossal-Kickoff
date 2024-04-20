using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    Camera camParent;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        camParent = transform.parent.gameObject.GetComponent<Camera>();
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.fieldOfView = camParent.fieldOfView;
    }
}
