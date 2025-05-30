using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchSettingsText : MonoBehaviour
{
    public string[] bodies;
    public TextMeshProUGUI bodyBox;

    [SerializeField] private float bodySpeed = 0.03f;
    [SerializeField] private float scaleSpeed = 2f;

    [Header("Toggle Features")]
    [SerializeField] private bool useTextScrawl = true;
    [SerializeField] private bool useScalability = true;

    private Coroutine bodyCoroutine;
    private Coroutine scaleCoroutine;

    private Vector3 hiddenScale = Vector3.zero;
    private Vector3 visibleScale = new Vector3(3.62f, 3.62f, 3.62f);

    private void Start()
    {
    }

    public void DisplayText(int index)
    {
        if (index < 0 || index >= bodies.Length)
        {
            return;
        }

        if (bodyCoroutine != null) StopCoroutine(bodyCoroutine);
        bodyBox.text = "";

        if (useScalability)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleOverTime(visibleScale));
        }

        if (useTextScrawl)
        {
            bodyCoroutine = StartCoroutine(TypeText(bodyBox, bodies[index], bodySpeed));
        }
        else
        {
            bodyBox.text = bodies[index];
        }
    }

    private IEnumerator TypeText(TextMeshProUGUI textBox, string text, float speed)
    {
        foreach (char letter in text)
        {
            // ADD SOUND EFFECTS SOMEWHERE HERE PLEASE
            textBox.text += letter;
            yield return new WaitForSeconds(speed);
        }
    }

    public void ClearText()
    {
        if (bodyCoroutine != null) StopCoroutine(bodyCoroutine);
        bodyBox.text = "";

        if (useScalability)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleOverTime(hiddenScale));
        }
    }

    public void TurnOff()
    {
        if (bodyCoroutine != null) StopCoroutine(bodyCoroutine);
        bodyBox.text = "";

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);

        transform.localScale = Vector3.zero;
    }

    private IEnumerator ScaleOverTime(Vector3 targetScale)
    {
        // ADD A SOUND EFFECT HERE FOR THE TEXTBOX APPEARING
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
