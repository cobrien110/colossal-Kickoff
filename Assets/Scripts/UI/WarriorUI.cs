using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarriorUI : MonoBehaviour
{
    //Charge Meter
    [SerializeField] private GameObject chargeBar = null;
    [SerializeField] private Image chargeBarFill = null;

    //Camera
    [SerializeField] private Camera camera = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 offset;

    [SerializeField] private GameObject callForPass = null;
    private bool isAI = false;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.parent;
        chargeBar.SetActive(false);
        callForPass.SetActive(false);
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

    public void ShowCallForPass(bool state)
    {
        if (!isAI) callForPass.SetActive(state);
    }

    public void SetAI()
    {
        isAI = true;
    }

}
