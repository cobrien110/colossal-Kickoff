using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPref : MonoBehaviour
{
    //public string[] playerPrefNames;
    public float threshold = 1f;
    private float runningTotal = 0;
    public StatTracker ST;
    // Start is called before the first frame update
    void Start()
    {
        if (ST == null) ST = GameObject.FindAnyObjectByType<StatTracker>();

        runningTotal = ST.saveData.mWins + ST.saveData.wWins;
        if (runningTotal >= threshold) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
