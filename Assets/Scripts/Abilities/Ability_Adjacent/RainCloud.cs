using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainCloud : MonoBehaviour
{
    //public GameObject shadow;
    //public float shadowHeight = -.2f;
    // Start is called before the first frame update
    void Start()
    {
        //shadow.transform.position = new Vector3(shadow.transform.position.x, shadowHeight, shadow.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        AbilityQuetzPassive AQP = other.gameObject.GetComponent<AbilityQuetzPassive>();
        if (AQP != null)
        {
            AQP.EatCloud();
            Destroy(gameObject);
        }
    }
}
