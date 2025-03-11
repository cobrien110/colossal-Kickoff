using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingBetweenFloats : MonoBehaviour
{
    public float minScale = 1f;
    public float maxScale = 2f;
    public float minTime = 1f;
    public float maxTime = 3f;

    private Vector3 targetScaleUpper;
    private Vector3 targetScaleLower;
    private Vector3 baseScale;
    private float duration;
    private bool scalingUp = true;

    void Start()
    {
        baseScale = transform.localScale; 

        targetScaleUpper = baseScale * Random.Range(minScale, maxScale);
        targetScaleLower = baseScale * Random.Range(minScale, maxScale);

        StartCoroutine(ScaleRoutine());
    }

    IEnumerator ScaleRoutine()
    {
        while (true)
        {
            duration = Random.Range(minTime, maxTime);

            Vector3 targetScale;
            if (scalingUp)
            {
                targetScale = targetScaleUpper;
            }
            else
            {
                targetScale = targetScaleLower;
            }

            Vector3 initialScale = transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localScale = targetScale;
            scalingUp = !scalingUp; 
        }
    }
}