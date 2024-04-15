using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 offset;

    private void LateUpdate()
    {
        if (targets.Count == 0) return;
        Vector3 centerPt = GetCenterPoint();
        Vector3 newPosition = centerPt + offset;

        transform.position = new Vector3(newPosition.x, offset.y, newPosition.z);
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
        }
        return bounds.center;
    }
}
