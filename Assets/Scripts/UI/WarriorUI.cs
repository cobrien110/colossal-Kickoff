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

    [SerializeField] private Image staminaFill = null;

    //Camera
    [SerializeField] private Camera camera = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private Vector3 offset;

    [SerializeField] private Image callForPass = null;
    private bool isAI = false;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.parent;
        chargeBar.SetActive(false);
        callForPass.enabled = false;
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

    public void UpdateStaminaBar(float charge)
    {
        if (!isAI) staminaFill.fillAmount = charge;
    }

    public void ShowCallForPass(bool state)
    {
        // Debug.Log("ShowCallForPass start. callForPass.enabled: " + callForPass.enabled);
        if (!isAI) Debug.Log(gameObject.transform.parent.name + " Showing call for pass arrow, is AI = " + isAI + ", setting to " + state);
        if (!isAI) callForPass.enabled = state;
        // Debug.Log("ShowCallForPass end. callForPass.enabled: " + callForPass.enabled);
    }

    public void SetAI()
    {
        isAI = true;
    }

}
