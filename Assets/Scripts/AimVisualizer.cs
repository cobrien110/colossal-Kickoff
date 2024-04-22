using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimVisualizer : MonoBehaviour
{
    WarriorController wc;
    MonsterController mc;
    private Vector3 aimDir;
    [SerializeField] private MeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        wc = transform.parent.gameObject.GetComponent<WarriorController>();
        mc = transform.parent.gameObject.GetComponent<MonsterController>();
        //mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wc != null)
        {
            aimDir = wc.GetAimDirection();
            if (aimDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
                //mr.enabled = true;
            } else
            {
                //mr.enabled = false;
            }
            
        }
        if (mc != null)
        {
            aimDir = mc.GetAimDirection();
            if (aimDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
                //mr.enabled = true;
            }
            else
            {
                //mr.enabled = false;
            }

        }
    }

}
