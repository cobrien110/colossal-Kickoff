using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Transform focusTarget;
    private bool isFocusing = false;
    public Vector3 offset;
    public float maxZOffset;
    private Vector3 velocity;
    public float smoothTime = .5f;
    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomLimiter = 50f;
    public float minFOV = 40f;
    public float maxFOV = 90f;
    public float zoomFOV = 30f;
    public float maxGreatestDistance = 7f;
    public bool useJumpCutoff = true;
    public float zJumpCutoff = 1f;

    public Camera mainCamera;

    [SerializeField] private float screenShakeDuration = 1.0f;
    public bool shouldShake = true;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        shouldShake = (PlayerPrefs.GetInt("screenshake", 1) != 0);
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
        if (isFocusing && !CheckForDeadPlayer() && focusTarget != null) centerPt = focusTarget.position;
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
        //if (isFocusing) newZoom = Mathf.Lerp(zoomFOV, minZoom, GetGreatestDistanceX() / zoomLimiter);

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, newZoom, Time.deltaTime);

        //float mFOV = CheckActorsOutOfFrame() ? minZoom : maxFOV;
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
    }

    private bool CheckBigGap()
    {
        if (GetGreatestDistanceZ() >= maxGreatestDistance && !isFocusing)
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
            if (targets[i] != null) bounds.Encapsulate(targets[i].position);
        }
        return bounds.center;
    }

    private float GetGreatestDistanceX()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        if (isFocusing && !CheckForDeadPlayer())
        {
            bounds.center = focusTarget.position;
            bounds.Encapsulate(focusTarget.position);
        } else
        {
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }
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
        if (targetTransform == null) return;
        targets.Add(targetTransform);
    }

    public void RemoveTarget(Transform targetTransform)
    {
        targets.Remove(targetTransform);
    }

    public void FocusOn(Transform fTarget)
    {
        if (fTarget == null)
        {
            Debug.Log("No focus target found");
            return;
        }
        isFocusing = true;
        focusTarget = fTarget;
        Invoke("EndFocus", 3f);
    }

    private void EndFocus()
    {
        isFocusing = false;
    }

    public bool CheckTarget(Transform targetTransform)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targetTransform.Equals(targets[i]))
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerator ScreenShake(float intensity)
    {
        if (shouldShake)
        {
            float elapsedTime = 0.0f;
            Vector3 startPosition = transform.position;

            while (elapsedTime < screenShakeDuration)
            {
                elapsedTime += Time.deltaTime;
                Vector3 random = Random.insideUnitSphere;
                random = new Vector3(Mathf.Clamp(random.x, -0.05f, 0.05f), Mathf.Clamp(random.y, -0.05f, 0.05f), Mathf.Clamp(random.z, -0.05f, 0.05f));
                transform.position = startPosition + random * intensity;
                yield return null;
            }

            transform.position = startPosition;
        }
    }

    private bool CheckForDeadPlayer()
    {
        if (focusTarget == null) return true;
        WarriorController WC = focusTarget.GetComponent<WarriorController>();
        AIMummy Mum = focusTarget.GetComponent<AIMummy>();
        if ((WC != null && WC.GetIsDead()) || Mum != null)
        {
            return true;
        }
        return false;
    }
}
