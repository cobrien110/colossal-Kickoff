using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lighningscript : MonoBehaviour
{
    public Animator lightningAnimator; // Reference to the Animator that handles the lightning flash
    public string lightningTriggerName = "Flash"; // The name of the trigger in the Animator

    public float minInterval = 10f; // Minimum time between flashes
    public float maxInterval = 30f; // Maximum time between flashes

    public AudioSource audioSource; // Assign an AudioSource component
    public AudioClip lightningSound; // Assign the sound clip for the lightning

    private void Start()
    {
        StartCoroutine(LightningLoop());
    }

    // This function will be called by the animation event
    public void PlayLightningSound()
    {
        if (audioSource != null && lightningSound != null)
        {
            audioSource.PlayOneShot(lightningSound);
        }
    }

    private IEnumerator LightningLoop()
    {
        while (true)
        {
            // Wait for a random interval between minInterval and maxInterval
            float randomInterval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(randomInterval);

            // Trigger the lightning animation
            lightningAnimator.SetTrigger(lightningTriggerName);
        }
    }
}
