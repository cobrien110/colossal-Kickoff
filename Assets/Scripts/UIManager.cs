using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text countdown = null;
    [SerializeField] private TMP_Text scoreText = null;
    private int warriorScore = 0;
    private int monsterScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Countdown()
    {
        countdown.text = "3";
        countdown.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdown.text = "2";
        yield return new WaitForSeconds(1f);
        countdown.text = "1";
        yield return new WaitForSeconds(1f);
        countdown.text = "GO";
        yield return new WaitForSeconds(1f);
        countdown.gameObject.SetActive(false);
    }

    public void warriorPoint()
    {
        warriorScore++;
        updateScore(); 
    }

    public void monsterPoint()
    {
        monsterScore++;
        updateScore();
    }

    private void updateScore()
    {
        scoreText.text = warriorScore + " - " + monsterScore;
    }
}
