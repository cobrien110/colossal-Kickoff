using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ConsoleCommands : MonoBehaviour
{
    private int test = 0;
    private GameplayManager GM;
    private UIManager UM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteCommand(string command)
    {
        string[] subs = null;
        //Debug.Log(command);
        //Debug.Log(command.Split(' ', 2));
        if (command != null) {
            subs = command.Split(' ', 2);
        }

        if (subs[0].Equals("settest"))
        {
            int start = command.IndexOf("<") + 1;
            int end = command.IndexOf(">");

            String result = command.Substring(start, end - start);
            //Debug.Log(result);

            try
            {
                int test = Int32.Parse(result);
                Debug.Log(test);
            }
            catch (FormatException)
            {
                Debug.Log("Bad");
            }
        }

        // Ends game (sets time to 3 seconds)
        if (subs[0].Equals("endgame"))
        {
            if (UM != null) UM.SetTimeRemaining(3);
        }

        // Copies stats from game to clipboard
        if (subs[0].Equals("copystats"))
        {
            string textToCopy = "";

            if (UM.GetTimeRemaining() <= 0)
            {
                textToCopy = "Stats from Game:\n";
            }
            else
            {
                textToCopy = "Stats from " + UM.GetTimeRemaining() + " seconds:\n";
            }

            textToCopy += "\nWinner: ";

            textToCopy += "\nScore: ";

            textToCopy += "\nKills: ";

            textToCopy += "\nAdd More Here";

            CopyToClipboard(textToCopy); 
        }

        else
        {
            Debug.Log("INVALID COMMAND");
        }
    }

    void CopyToClipboard(string s)
    {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }
}
