using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    //camera positions for each part of the menu are saved here
    [Header("Camera Positions")]
    public Transform titleTransform;
    public Transform versusSetupTransform;
    public Transform settingsTransform;
    public Transform creditsTransform;
    public Transform quittingTransform;

    public Transform arcadeTransform;
    public Transform statsTransform;
    public Transform statsTransformZoom;
    public Transform howToTransform;
    public Transform howToTransformZoom;
    public Transform sandboxTransform;
    public Transform sandboxTransformZoom;

    [Header("Variables")]
    //camera movement speed
    public float speed = 30.0f;
    //camera rotation speed
    public float angleSpeed = 4.0f;
    //position we're moving towards, if any
    private Vector3 targetPos;
    //angle we're rotating towards, if any
    private Quaternion targetAngle;
    //whether we're moving or not
    [SerializeField] private bool isMoving = false;

    void Start()
    {
        if (titleTransform != null)
        {
            transform.position = titleTransform.position;
            transform.rotation = titleTransform.rotation;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, angleSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.001f && transform.rotation == targetAngle)
            {
                isMoving = false;
            }
        }
    }

    public void goToTitle()
    {
        SetTarget(titleTransform);
    }

    public void goToVersusSetup()
    {
        SetTarget(versusSetupTransform);
    }

    public void goToSettings()
    {
        SetTarget(settingsTransform);
    }

    public void goToCredits()
    {
        SetTarget(creditsTransform);
    }

    public void goToQuitting()
    {
        SetTarget(quittingTransform);
    }

    public void goToExtras()
    {
        SetTarget(arcadeTransform);
    }

    public void goToSandbox()
    {
        SetTarget(sandboxTransform);
    }

    public void goToSandboxZoom()
    {
        SetTarget(sandboxTransformZoom);
    }

    public void goToHowTo()
    {
        SetTarget(howToTransform);
    }

    public void goToHowToZoom()
    {
        SetTarget(howToTransformZoom);
    }

    public void goToStats()
    {
        SetTarget(statsTransform);
    }

    public void goToStatsZoom()
    {
        SetTarget(statsTransformZoom);
    }



    private void SetTarget(Transform targetTransform)
    {
        if (targetTransform != null)
        {
            targetPos = targetTransform.position;
            targetAngle = targetTransform.rotation;
            isMoving = true;
        }
    }
}