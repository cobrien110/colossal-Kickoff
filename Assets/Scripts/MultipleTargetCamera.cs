using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 offset;
    private Vector3 velocity;
    public float smoothTime = .5f;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;

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

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, newZoom, Time.deltaTime);
    }

    private Vector3 GetCenterPoint()
    {
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

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
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
