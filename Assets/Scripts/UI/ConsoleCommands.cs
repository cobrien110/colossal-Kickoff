using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ConsoleCommands : MonoBehaviour
{
    private int test = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteCommand(string command)
    {
        Debug.Log(command);
        
        if (command.Substring(0, 8).Equals("settest "))
        {
            int start = command.IndexOf("<") + 1;
            int end = command.IndexOf(">");

            String result = command.Substring(start, end - start);
            Debug.Log(result);

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
        else
        {
            Debug.Log("INVALID COMMAND");
        }
    }
}
