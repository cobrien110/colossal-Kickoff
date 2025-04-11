using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVTextScroll : MonoBehaviour
{
    public Transform warningTextTransform;
    public Transform spawnPoint; //Transform where elements spawn
    public Transform borderPoint; //Transform where elements are considered out of bounds
    public float speed = 1f; //Scrolling speed
    public TMP_Text scrollingText; //Text

    private bool isScrolling = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetWarning();    }

    public void WarningStart()
    {
        //Debug.Log("Start Called");
        if (!isScrolling)
        {
            isScrolling = true;
            //Debug.Log("Should Be scrolling");
            StartCoroutine(ScrollWarning());
        }
    }

    public void WarningEnd()
    {
        //Debug.Log("End Called");
        isScrolling = false;
        StopAllCoroutines();
        ResetWarning();
    }

    public void ResetWarning()
    {
        TextRandom();
        warningTextTransform.transform.position = spawnPoint.transform.position;
    }

    private IEnumerator ScrollWarning()
    {
        while (isScrolling)
        {
            //Debug.Log("Scrolling");
            warningTextTransform.transform.position += Vector3.right * speed * Time.deltaTime;
            if (warningTextTransform.transform.position.x > borderPoint.position.x)
            {
                ResetWarning();
            }
            yield return null;

        }
    }

    //Add your text here and remember to increase the random range :)
    private void TextRandom()
    {
        int x = Random.Range(0, 100);
        if (x < 15)
        {
            scrollingText.text = "BREAKING NEWS: COLOSSSAL SOCCER GAMES NOW IN PROGRESS!";
        }
        else if (x < 30)
        {
            scrollingText.text = "HISTORIANS RECRUITED TO DEFENSE FORCES AS INVASION CONTINUES";
        }
        else if (x < 55)
        {
            scrollingText.text = "WARNING! MONSTERS ARE INVADING EARTH! SEEK SHELTER IMMEDIATELY!";
        }
        else if (x < 75)
        {
            scrollingText.text = "WHERE DID OUR USUAL REPORTER GO? NO ONE HAS SEEN HIM, SEND HELP";
        }
        else if (x < 98)
        {
            scrollingText.text = "REMEMBER TO JOIN THE DISCORD :)";
        } 
        else if (x == 98)
        {
            scrollingText.text = "CREATURE SIGHTING: AMPHIBIAN SEEN TERRORISING MIDWESTERN USA";
        }
        else if (x == 99)
        {
            scrollingText.text = "ECONOMICS UPDATE: WIZARD FIRED FROM JOB FOR HUNDRETH TIME";
        }
    }
}
