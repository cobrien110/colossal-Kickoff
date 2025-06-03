using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinder : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isContinuous = false;
    public bool detatchAndFollowParent = false;
    private Transform followTarg;

    // Start is called before the first frame update
    void Start()
    {
        if (detatchAndFollowParent)
        {
            followTarg = transform.parent;
            transform.parent = null;
            transform.eulerAngles = Vector3.zero;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.025f, transform.position.z);
        }
    }

    private void Update()
    {
        if (detatchAndFollowParent && followTarg == null)
        {
            Destroy(gameObject);
            return;
        }
        if (!isContinuous) return;

        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            if (!detatchAndFollowParent)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + 0.025f, transform.position.z);
            } else
            {
                transform.position = new Vector3(followTarg.position.x, hit.point.y + 0.025f, followTarg.position.z);
            }
            
        }
    }

}
