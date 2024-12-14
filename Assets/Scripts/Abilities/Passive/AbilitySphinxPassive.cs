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

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        if (auraPrefab != null && auraObject == null)
        {
            auraObject = Instantiate(auraPrefab, transform);
        }
        baseScale = auraObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (auraObject == null) return;
        Vector3 newScale = new Vector3(baseScale.x + sizePerStack * counterAmount, baseScale.y, baseScale.z + sizePerStack * counterAmount);
        auraObject.transform.localScale = newScale;
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
        timer = 0;
    }
}
