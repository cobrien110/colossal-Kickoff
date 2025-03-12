using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVTextScroll : MonoBehaviour
{
    public Transform warningTextTransform;
    public Transform spawnPoint; //Transform where elements spawn
    public Transform borderPoint; //Transform where elements are considered out of bounds
    public float speed = 1f; //Scrolling speed

    private bool isScrolling = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetWarning();    }

    public void WarningStart()
    {
        Debug.Log("Start Called");
        if (!isScrolling)
        {
            isScrolling = true;
            Debug.Log("Should Be scrolling");
            StartCoroutine(ScrollWarning());
        }
    }

    public void WarningEnd()
    {
        Debug.Log("End Called");
        isScrolling = false;
        StopAllCoroutines();
        ResetWarning();
    }

    public void ResetWarning()
    {
        warningTextTransform.transform.position = spawnPoint.transform.position;
    }

    private IEnumerator ScrollWarning()
    {
        while (isScrolling)
        {
            Debug.Log("Scrolling");
            warningTextTransform.transform.position += Vector3.right * speed * Time.deltaTime;
            if (warningTextTransform.transform.position.x > borderPoint.position.x)
            {
                ResetWarning();
            }
            yield return null;

        }
    }
}
