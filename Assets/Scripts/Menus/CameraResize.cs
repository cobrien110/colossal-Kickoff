using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResize : MonoBehaviour
{
    [SerializeField] Camera camA;
    [SerializeField] Camera camB;
    [SerializeField] RectTransform rawA;
    [SerializeField] RectTransform rawB;

    void Update()
    {
        // World-to-screen scaling
        float aspectA = rawA.rect.width / rawA.rect.height;
        float aspectB = rawB.rect.width / rawB.rect.height;

        camA.aspect = aspectA;
        camB.aspect = aspectB;
    }
}