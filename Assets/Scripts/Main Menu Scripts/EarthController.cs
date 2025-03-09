using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    [Header("Assignments")]
    public Transform earthModel;
    public Animator earthAnimator;

    [Header("Stage Identifiers")]
    public GameObject[] Markers;
    public GameObject[] TextBoxes;

    private GameObject previousMark;

    //Preset rotation targets for different locations (set these in the inspector)
    [Header("Rotations")]
    public Quaternion[] countryRotations;
    public float rotationSpeed = 2.0f;

    private Coroutine rotationCoroutine;
    private bool noCountry = true;
    void Start()
    {
        if (earthModel == null || countryRotations.Length == 0)
        {
            Debug.LogWarning("EarthController: Missing references!");
            return;
        }

        foreach (GameObject marker in Markers)
        {
            try
            {
                marker.GetComponent<SpriteRenderer>().color = new Color(.9f, .9f, .9f);
            } catch
            {
                Debug.LogWarning($"EarthController: Marker {marker.name} is missing a SpriteRenderer component!");
            }
        }
    }

    //Call this method to rotate the Earth to a specific country
    public void RotateToCountry(int countryIndex)
    {
        StopIdleAnimation();
        SwitchMarker(countryIndex);
        if (countryIndex < 0 || countryIndex >= countryRotations.Length)
        {
            Debug.LogWarning("Invalid country index!");
            return;
        }

        if (rotationCoroutine != null)
            StopCoroutine(rotationCoroutine);

        rotationCoroutine = StartCoroutine(SmoothRotate(earthModel.localRotation, countryRotations[countryIndex]));
    }

    //Smoothly rotates the Earth to the target rotation
    IEnumerator SmoothRotate(Quaternion startRotation, Quaternion targetRotation)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            earthModel.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }
        if (noCountry)
        {
            earthAnimator.SetBool("isIdle", true);
        } else {
            earthAnimator.SetBool("isIdle", false);
        }
    }

    public void SwitchMarker(int countryIndex)
    {
        if (previousMark != null)
        {
            previousMark.GetComponent<SpriteRenderer>().color = new Color(.9f, .9f, .9f);
        }
        if (countryIndex > 0)
        {
            GameObject currentMark = Markers[countryIndex - 1];
            currentMark.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
            previousMark = currentMark;
        }
        
    }

    //Call when hovering over the main menu
    public void StartIdleAnimation()
    {
        RotateToCountry(0);
        noCountry = true;
    }

    //Call when main menu is no longer selected
    public void StopIdleAnimation()
    {
        noCountry = false;
    }
}