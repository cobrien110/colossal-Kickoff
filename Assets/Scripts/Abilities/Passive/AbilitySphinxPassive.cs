using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySphinxPassive : PassiveAbility
{
    public GameObject auraObject;
    public GameObject auraPrefab;
    public float sizePerStack = 1f;
    private Vector3 baseScale;
    public float timeBeforeCounterLoss = 6f;
    private SlowAura aura;
    public float slowRate = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        if (auraPrefab != null && auraObject == null)
        {
            auraObject = Instantiate(auraPrefab, transform);
        }
        aura = auraObject.GetComponent<SlowAura>();
        baseScale = auraObject.transform.localScale;
        aura.slowAmount = slowRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (auraObject == null) return;
        Vector3 newScale = new Vector3(baseScale.x + sizePerStack * counterAmount, baseScale.y, baseScale.z + sizePerStack * counterAmount);
        auraObject.transform.localScale = Vector3.Lerp(auraObject.transform.localScale, newScale, Time.deltaTime * 1.25f);
        if (counterAmount > 0)
        {
            timer += Time.deltaTime;
        }
        if (timer >= timeBeforeCounterLoss && counterAmount > 0)
        {
            counterAmount--;
            timer = 0;
        }

        UpdateChargeBar();
    }

    public void AddCounter()
    {
        counterAmount++;
        if (counterAmount > counterMax) counterAmount = counterMax;
        //timer = 0;
    }
}
