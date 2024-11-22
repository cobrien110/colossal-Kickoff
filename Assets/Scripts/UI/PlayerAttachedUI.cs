using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttachedUI : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        ShowChargeBar(false);
        target = transform.parent;
        if (gameObject.GetComponentInParent<WarriorController>() != null)
        {
            allDots.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }

    public void UpdateChargeBar(float charge)
    {
        chargeBarFill.fillAmount = charge;
    }

    public void ShowChargeBar(bool state)
    {
        chargeBar.SetActive(state);
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



}
