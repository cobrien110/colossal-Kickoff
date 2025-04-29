using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicatorArrow : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    private SpriteRenderer SR;
    private int playerNum;
    private WarriorController WC;
    private float alphaRate = 0.5f;
    private Transform p;
    private bool isHiding = false;
    private bool gotIsDead = false;
    private float currentA = 1f;
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        SR.enabled = false;
        p = transform.parent;
        currentA = SR.color.a;
        StartCoroutine(SetPlayer());
        StartCoroutine(BeginHiding(2.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (SR.enabled && currentA > 0 && isHiding)
        {
            currentA -= alphaRate * Time.deltaTime;
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, currentA);
            if (currentA <= 0) isHiding = false;
        } 

        if (SR.enabled && p != null && transform.parent == null && currentA > 0)
        {
            transform.position = new Vector3(p.position.x, transform.position.y, p.position.z);
        }

        if (WC != null && WC.GetIsDead())
        {
            StartCoroutine(ResetA());
        }
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    private IEnumerator SetPlayer()
    {
        yield return new WaitForSeconds(.1f);
        WC = GetComponentInParent<WarriorController>();
        playerNum = WC.playerID;
        SR.enabled = true;
        SR.sprite = sprites[playerNum];
        transform.parent = null;
    }

    public IEnumerator ResetA()
    {
        if (WC != null && !gotIsDead)
        {
            gotIsDead = true;
            yield return new WaitForSeconds(WC.GetRespawnTime());
            if (playerNum < 4)
            {
                SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, .5f);
                StartCoroutine(BeginHiding(.75f));
                //alphaRate = 1f;
            }
        }
    }

    private IEnumerator BeginHiding(float waitDuration)
    {
        Debug.Log("Hiding Indicator");
        yield return new WaitForSeconds(waitDuration);
        isHiding = true;
        gotIsDead = false;
    }
}
