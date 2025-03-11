using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScrollingUI : MonoBehaviour
{
    public List<GameObject> creditElements = new List<GameObject>(); //List of credit objects
    public Transform spawnPoint; //Transform where elements spawn
    public Transform borderPoint; //Transform where elements are considered out of bounds
    public float speed = 100f; //Scrolling speed
    public float offset = 20f; //Vertical spacing between elements

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
        //Position elements at the spawn point with correct offsets
        float currentY = spawnPoint.position.y;
        for (int i = 0; i < creditElements.Count; i++)
        {
            RectTransform rectTransform = creditElements[i].GetComponent<RectTransform>();
            float elementHeight = rectTransform.rect.height; //Get actual height of the UI element

            creditElements[i].transform.position = new Vector3(spawnPoint.position.x, currentY, spawnPoint.position.z);
            currentY -= (elementHeight + offset); //Apply spacing
        }
    }

    private IEnumerator ScrollCredits()
    {
        while (isScrolling)
        {
            for (int i = 0; i < creditElements.Count; i++)
            {
                GameObject element = creditElements[i];
                RectTransform rectTransform = element.GetComponent<RectTransform>();
                float elementHeight = rectTransform.rect.height;

                while (element.transform.position.y < borderPoint.position.y)
                {
                    element.transform.position += Vector3.up * speed * Time.deltaTime;
                    yield return null;
                }

                //Move element to the bottom of the list and reposition
                creditElements.RemoveAt(i);
                creditElements.Add(element);

                //Set new position at the bottom
                float lowestY = creditElements[creditElements.Count - 2].transform.position.y;
                element.transform.position = new Vector3(spawnPoint.position.x, lowestY - (elementHeight + offset), spawnPoint.position.z);

                yield return null;
            }
        }
    }
}