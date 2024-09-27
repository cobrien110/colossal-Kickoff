using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashadokuroHand : MonoBehaviour
{
    AbilityCreateHands createHandsScript;
    // Start is called before the first frame update
    void Start()
    {
        createHandsScript = FindObjectOfType<AbilityCreateHands>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger && collider.gameObject.GetComponent<BallProperties>() != null)
        {
            Debug.Log("Hand hit ball: " + collider.name);

            Vector3 hitballDirection =
                (new Vector3(collider.gameObject.transform.position.x, 0, collider.gameObject.transform.position.z)
                - new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z)).normalized;
            collider.gameObject.GetComponent<Rigidbody>().AddForce(hitballDirection * createHandsScript.hitballSpeed);
        }
    }
}
