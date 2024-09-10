using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 offset;
    public float maxZOffset;
    private Vector3 velocity;
    public float smoothTime = .5f;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;
    public float minFOV = 40f;
    public float maxFOV = 90f;
    public float maxGreatestDistance = 7f;
    public bool useJumpCutoff = true;
    public float zJumpCutoff = 1f;

    public Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        if (targets.Count == 0) return;
        Move();
        Zoom();
    }

    private void Move()
    {
        Vector3 centerPt = GetCenterPoint();
        Vector3 newPosition = centerPt + offset;
        if (newPosition.z < maxZOffset)
        {
            newPosition.z = maxZOffset;
        }

        if (CheckBigGap() && (useJumpCutoff && GetCenterPoint().z > zJumpCutoff))
        {
            newPosition.z = maxZOffset;
        } 
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistanceX() / zoomLimiter);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, newZoom, Time.deltaTime);

        //float mFOV = CheckActorsOutOfFrame() ? minZoom : maxFOV;
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
    }

    private bool CheckBigGap()
    {
        if (GetGreatestDistanceZ() >= maxGreatestDistance)
        {
            return true;
        }
        return false;
    }

    private bool CheckActorsOutOfFrame()
    {
        return false;
        /*
        if (targets.Count <= 1) return false;
        else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Vector3 t1 = targets[i].position;
                Vector3 t2 = transform.forward;
                float a = Vector3.Angle(t2, t1);
                Mathf.Abs(a);
                if (a > mainCamera.fieldOfView / 2)
                {
                    return true;
                }
            }
        }
        return false;
        */
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 0) return Vector3.zero;
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(Vector3.zero, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            /*
            if (targets[i].gameObject.tag.Equals("Ball"))
            {
                BallProperties bp = targets[i].GetComponent<BallProperties>();
                if (bp.ballOwner == null)
                {
                    bounds.Encapsulate(targets[i].position);
                }
            } else
            {
                bounds.Encapsulate(targets[i].position);
            }
            */
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    private float GetGreatestDistanceX()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }

    private float GetGreatestDistanceZ()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.z;
    }

    public void AddTarget(Transform targetTransform)
    {
        targets.Add(targetTransform);
    }

    public void RemoveTarget(Transform targetTransform)
    {
        targets.Remove(targetTransform);
    }
}
