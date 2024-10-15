using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowAura : MonoBehaviour
{
    [SerializeField] private float slowAmount = 0.4f;
    [SerializeField] private float auraLifespan = 3f;

    private List<WarriorController> warriorsInside = new List<WarriorController>();

    void Start()
    {
        StartCoroutine(StartDespawnTimer());
    }

    private void OnTriggerEnter(Collider other)
    {
        WarriorController wc = other.gameObject.GetComponent<WarriorController>();
        if (wc != null && !warriorsInside.Contains(wc))
        {
            Debug.Log("Warrior entered slow aura");
            wc.warriorSpeed = WarriorController.baseMovementSpeed * slowAmount;
            warriorsInside.Add(wc);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WarriorController wc = other.gameObject.GetComponent<WarriorController>();
        if (wc != null && warriorsInside.Contains(wc))
        {
            Debug.Log("Warrior exited slow aura");
            wc.warriorSpeed = WarriorController.baseMovementSpeed;
            warriorsInside.Remove(wc);
        }
    }

    private IEnumerator StartDespawnTimer()
    {
        yield return new WaitForSeconds(auraLifespan);

        Debug.Log("Destroying Slow Aura");
        ResetWarriorSpeeds();  // Reset warriors' speeds before destruction
        Destroy(gameObject);
    }

    private void ResetWarriorSpeeds()
    {
        foreach (WarriorController wc in warriorsInside)
        {
            if (wc == null) continue;
            wc.warriorSpeed = WarriorController.baseMovementSpeed;
        }
        warriorsInside.Clear();
    }
}
