using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private GameObject okayMessage;
    public bool confirmed = false;
    void Start()
    {
        confirmed = false;
    }

    public void confirm() {
        okayMessage.SetActive(true);
        confirmed = true;
    }

    public void unconfirm() {
        okayMessage.SetActive(false);
        confirmed = false;
    }
}
