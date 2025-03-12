using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNews : MonoBehaviour
{

    public float minTalkingTime = .5f;
    public float maxTalkingTime = 3;

    public float minTalkingInterval = 1f;
    public float maxTalkingInterval = 4f;

    public Animator orcAni;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OrcmanTalking());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator OrcmanTalking()
    {
        while (true)
        {
            orcAni.SetBool("isTalking", true);
            float talkingTime = Random.Range(minTalkingTime, maxTalkingTime);
            yield return new WaitForSeconds(talkingTime);
            orcAni.SetBool("isTalking", false);
            float talkingInterval = Random.Range(minTalkingInterval, maxTalkingInterval);
            yield return new WaitForSeconds(talkingInterval);
        }
    }
}
