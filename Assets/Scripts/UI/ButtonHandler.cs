using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    //private Button thisButton = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.gameObject);
        //thisButton = this.gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        this.gameObject.GetComponentInChildren<TMP_Text>().fontSize *= 1.1f;
        Debug.Log("button highlight");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.gameObject.GetComponentInChildren<TMP_Text>().fontSize /= 1.1f;
        Debug.Log("button unhighlight");
    }
}
