using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TutorialUIManager : MonoBehaviour
{
    
    [SerializeField] private TMP_Text playerScoredText = null;
    [SerializeField] private GameObject pauseScreen = null;
    [SerializeField] private GameObject hideWhenPaused = null;
    [SerializeField] private GameObject statsScoreboard = null;
    [Header("ChargeMeter")]
    [SerializeField] private GameObject chargeBar = null;
    [SerializeField] private Image chargeBarFill = null;
    [SerializeField] private TMP_Text chargeBarText = null;
    [SerializeField] private AudioPlayer AP;
    private bool isSuperFlashing = false;
    
    Coroutine timerCoroutine;
    /*Coroutine ability1TimerCoroutine;
    Coroutine ability2TimerCoroutine;
    Coroutine ability3TimerCoroutine;*/
    GameplayManager GM;
    StatTracker ST;
    //TMP_InputField console;

    public bool overtime = false;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        
        ShowChargeBar(false);
        //ShowPlayerUI(false, 1);
        //ShowPlayerUI(false, 2);
        //ShowPlayerUI(false, 3);
        //ShowPassMeter(false);
        UpdateChargeBarText("");


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPlayerScoredText(bool state)
    {
        playerScoredText.gameObject.SetActive(state);
    }

    public void UpdatePlayerScoredText(int player)
    {
        if (player == 0) playerScoredText.text = "CPU SCORED!";
        else if (player > 0) playerScoredText.text = "PLAYER " + (player) + " SCORED!";
        else playerScoredText.text = "PLAYER " + (player * -1) + " OWN GOALED!";
    }

    public void ShowChargeBar(bool state)
    {
        chargeBar.gameObject.SetActive(state);
    }

    public void UpdateChargeBarText(string text)
    {
        chargeBarText.text = text;
    }


    public void PauseScreen(bool isPaused, int playerID)
    {
        statsScoreboard.SetActive(isPaused);
        pauseScreen.SetActive(isPaused);
        HideWhenPaused(!isPaused);
        if (isPaused)
        {
            if (AP != null) AP.PlaySoundRandomPitch(AP.Find("pauseWhistle"));
            GameObject.Find("WhoPaused").GetComponent<TMP_Text>().text = "Player " + (playerID + 1) + " Paused";
        }
        else
        {
            if (AP != null) AP.PlaySoundRandomPitch(AP.Find("pauseWhistle2"));
        }
    }

    private void HideWhenPaused(bool state)
    {
        hideWhenPaused.SetActive(state);
    }

}
