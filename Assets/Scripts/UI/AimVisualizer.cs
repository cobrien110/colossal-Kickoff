using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimVisualizer : MonoBehaviour
{
    WarriorController wc;
    MonsterController mc;
    private Vector3 aimDir;
    [SerializeField] private GameObject arrow;
    [SerializeField] private SpriteRenderer SR;
    [SerializeField] private Color arrowColor;
    //[SerializeField] private MeshRenderer mr;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;

    // Start is called before the first frame update
    void Start()
    {
        //wc = GetComponentInParent<WarriorController>();
        //mc = GetComponentInParent<MonsterController>();
        wc = transform.parent.gameObject.GetComponent<WarriorController>();
        mc = transform.parent.gameObject.GetComponent<MonsterController>();
        //mr = GetComponent<MeshRenderer>();
        SR = arrow.GetComponent<SpriteRenderer>();
        arrowColor = SR.color;

        NewBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (wc != null)
        {
            aimDir = wc.GetAimDirection();
            if (aimDir != Vector3.zero && BP?.ballOwner == transform.parent.gameObject)
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
        if (Ball == null)
        {
            NewBall();
        }
    }

    void NewBall()
    {
        Ball = GameObject.FindGameObjectWithTag("Ball");
        BP = (BallProperties)Ball?.GetComponent("BallProperties");
    }

    public void SuperKickColor(Color newColor)
    {
        if (SR.color.Equals(newColor))
        {
            return;
        }
        SR.color = newColor;
        Debug.Log("Changing Color");
    }

    public void RevertColor()
    {
        SR.color = arrowColor;
        Debug.Log("Reverting Color");
    }

}
