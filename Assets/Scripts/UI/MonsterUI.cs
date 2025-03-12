using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterUI : MonoBehaviour
{
    //Charge Meter
    [SerializeField] private GameObject chargeBar = null;
    [SerializeField] private Image chargeBarFill = null;

    //Ability Dots
    [SerializeField] private GameObject allDots = null;
    [SerializeField] private GameObject dot1 = null;
    [SerializeField] private GameObject dot2 = null;

    //Camera
    [SerializeField] private Camera camera = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 offset;

    private bool isAI = false;

    // Start is called before the first frame update
    void Start()
    {
        ShowChargeBar(false);
        target = transform.parent;
        chargeBar.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }

    public void UpdateChargeBar(float charge)
    {
        if (!isAI) chargeBarFill.fillAmount = charge;
    }

    public void ShowChargeBar(bool state)
    {
        if (!isAI) chargeBar.SetActive(state);
    }

    public void UpdateDot1(bool state)
    {
        dot1.SetActive(state);
    }

    public void UpdateDot2(bool state)
    {
        dot2.SetActive(state);
    }

    public void ShowDots(bool state)
    {
        allDots.SetActive(state);
    }
    public void SetAI()
    {
        isAI = true;
    }

}
