using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMummyCommunication : MonoBehaviour
{
    [SerializeField] private AIMummy bigBrother;

    public void ChangeStillnessMethod()
    {
        bigBrother.ChangeStillness();
    }

    public void MakeDieMethod()
    {
        bigBrother.MakeDie();
    }
}
