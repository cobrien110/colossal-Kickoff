using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehindCheck : MonoBehaviour
{
    public GameObject gameCamera;
    public GameObject[] objectsToDisable;

    private void Update()
    {
        if (gameCamera != null && objectsToDisable != null)
        {
            float cameraZPosition = gameCamera.transform.position.z;

            if (transform.position.z > cameraZPosition)
            {
                foreach (GameObject obj in objectsToDisable)
                {
                    MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
            }
            else
            {
                foreach (GameObject obj in objectsToDisable)
                {
                    MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = true;
                    }
                }
            }
        }
    }
}