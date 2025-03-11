using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScrollingUI : MonoBehaviour
{
    public Transform creditElements;
    public Transform spawnPoint; //Transform where elements spawn
    public Transform borderPoint; //Transform where elements are considered out of bounds
    public float speed = 1f; //Scrolling speed

    private bool isScrolling = false;

    void Start()
    {
        ResetCredits();
    }

    public void CreditsStart()
    {
        if (!isScrolling)
        {
            isScrolling = true;
            StartCoroutine(ScrollCredits());
        }
    }

    public void CreditsEnd()
    {
        isScrolling = false;
        StopAllCoroutines();
        ResetCredits();
    }

    private void ResetCredits()
    {
        creditElements.transform.position = spawnPoint.transform.position;
    }

    private IEnumerator ScrollCredits()
    {
        while (isScrolling)
        {

            creditElements.transform.position += Vector3.up * speed * Time.deltaTime;
            if (creditElements.transform.position.y > borderPoint.position.y)
            {
                ResetCredits();
            }
            yield return null;

        }
    }
}