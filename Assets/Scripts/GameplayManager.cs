using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public bool isPlaying = false;
    [SerializeField] private UIManager UI = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Kickoff());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlaying()
    {
        isPlaying = true;
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }

    private IEnumerator Kickoff()
    {
        StartCoroutine(UI.Countdown());
        yield return new WaitForSeconds(3f);
        StartPlaying();
    }
}
