using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TVTextScroll : MonoBehaviour
{
    public class WeightedText
    {
        public string message;
        public int weight;
    }

    //Add your text here with appropriate weights :)
    //Low number means less chance to activate :3
    public List<WeightedText> weightedMessages = new List<WeightedText>
    {
        new WeightedText { message = "BREAKING NEWS: COLOSSSAL SOCCER GAMES NOW IN PROGRESS!", weight = 15 },
        new WeightedText { message = "HISTORIANS RECRUITED TO DEFENSE FORCES AS INVASION CONTINUES", weight = 15 },
        new WeightedText { message = "WARNING! MONSTERS ARE INVADING EARTH! SEEK SHELTER IMMEDIATELY!", weight = 25 },
        new WeightedText { message = "WHERE DID OUR USUAL REPORTER GO? NO ONE HAS SEEN HIM, SEND HELP", weight = 20 },
        new WeightedText { message = "REMEMBER TO JOIN THE DISCORD :)", weight = 23 },
        new WeightedText { message = "CRYPTIDS RUNNING RAMPANT IN AMERICA! COLOSSAL LEAGUE WORKING ON A RESPONSE.", weight = 5 },
        new WeightedText { message = "DID YOU KNOW YOU HAVE RIGHTS? MONSTERS SAY YOU DON'T", weight = 3 },
        new WeightedText { message = "AVERAGE COLOSSAL LEAGUE MEMBER CLAIMS THEY CAN \"1V1 A GORILLA\"?", weight = 13 },
        new WeightedText { message = "MINOTAUR SHARES COOKING RECIPIES ONLINE. MOTHERS OF THE DECEASED APPALLED", weight = 5 },
        new WeightedText { message = "A NEW \"CRASH OUT\" DISEASE CAUSING A NEW PANDEMIC.", weight = 1 },
        new WeightedText { message = "MISSING: PIXEL. PLEASE SEND INFORMATION TO (XXX) XXX - XXXX", weight = 1 },
        new WeightedText { message = "CREATURE SIGHTING: AMPHIBIAN SEEN TERRORISING MIDWESTERN USA", weight = 1 },
        new WeightedText { message = "ECONOMICS UPDATE: WIZARD FIRED FROM JOB FOR HUNDRETH TIME", weight = 1 }
    };

    public Transform warningTextTransform;
    public Transform spawnPoint; //Transform where elements spawn
    public Transform borderPoint; //Transform where elements are considered out of bounds
    public float speed = 1f; //Scrolling speed
    public TMP_Text scrollingText; //Text

    private bool isScrolling = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetWarning();
    }

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
        warningTextTransform.position = spawnPoint.position;
    }

    private IEnumerator ScrollWarning()
    {
        while (isScrolling)
        {
            //Debug.Log("Scrolling");
            warningTextTransform.position += Vector3.right * speed * Time.deltaTime;
            if (warningTextTransform.position.x > borderPoint.position.x)
            {
                ResetWarning();
            }
            yield return null;
        }
    }

    private void TextRandom()
    {
        scrollingText.text = GetRandomWeightedMessage();
    }

    //Picks a message based on its weight
    private string GetRandomWeightedMessage()
    {
        int totalWeight = 0;
        foreach (var item in weightedMessages)
        {
            totalWeight += item.weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var item in weightedMessages)
        {
            currentSum += item.weight;
            if (randomValue < currentSum)
            {
                return item.message;
            }
        }

        //fallback, should never hit this
        return weightedMessages[0].message;
    }
}