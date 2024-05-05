using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimVisualizer : MonoBehaviour
{
    WarriorController wc;
    MonsterController mc;
    private Vector3 aimDir;
    [SerializeField] private GameObject arrow;
    [SerializeField] private MeshRenderer mr;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;

    // Start is called before the first frame update
    void Start()
    {
        wc = transform.parent.gameObject.GetComponent<WarriorController>();
        mc = transform.parent.gameObject.GetComponent<MonsterController>();
        //mr = GetComponent<MeshRenderer>();
        Ball = GameObject.Find("Ball");
        BP = (BallProperties)Ball.GetComponent("BallProperties");
    }

    // Update is called once per frame
    void Update()
    {
        if (wc != null)
        {
            aimDir = wc.GetAimDirection();
            if (aimDir != Vector3.zero && BP.ballOwner == transform.parent.gameObject)
            {
                arrow.SetActive(true);
                transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
                //mr.enabled = true;
            } else
            {
                //mr.enabled = false;
                arrow.SetActive(false);
            }

        }
        if (mc != null)
        {
            aimDir = mc.GetAimDirection();
            if (aimDir != Vector3.zero && BP.ballOwner == transform.parent.gameObject)
            {
                arrow.SetActive(true);
                transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
                //mr.enabled = true;
            }
            else
            {
                arrow.SetActive(false);
                //mr.enabled = false;
            }

        }
    }

}
