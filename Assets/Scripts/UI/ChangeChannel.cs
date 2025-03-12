using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeChannel : MonoBehaviour
{
    public GameObject creditsTV;
    public GameObject earthTV;

    public Material newsMat;
    public Material earthMat;
    public Material creditsMat;
    public Material staticMat;

    //Time delay for static effect
    [SerializeField] private float timeToChange = 0.5f; 

    private Renderer creditsRenderer;
    private Renderer earthRenderer;

    private void Start()
    {
        if (creditsTV != null) creditsRenderer = creditsTV.GetComponent<Renderer>();
        if (earthTV != null) earthRenderer = earthTV.GetComponent<Renderer>();
    }

    public void SwitchToNews()
    {
        StartCoroutine(SwapMaterial(newsMat));
    }

    public void SwitchToEarth()
    {
        StartCoroutine(SwapMaterial(earthMat));
    }

    public void SwitchToCredits()
    {
        StartCoroutine(SwapMaterial(creditsMat));
    }

    private IEnumerator SwapMaterial(Material newMat)
    {
        SetMaterial(staticMat);

        yield return new WaitForSeconds(timeToChange);

        SetMaterial(newMat);
    }

    private void SetMaterial(Material mat)
    {
        if (creditsRenderer != null) creditsRenderer.material = mat;
        if (earthRenderer != null) earthRenderer.material = mat;
    }
}