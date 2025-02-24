using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbilityDelayed : AbilityScript
{
    // Start is called before the first frame update
    void Start()
    {
        // Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateSetup();
    }

    public void CheckInputs(InputAction.CallbackContext context)
    {
        if (!canActivate) return;
        CheckInputsDelayed(context);
    }
    public abstract void CheckInputsDelayed(InputAction.CallbackContext context);
}
