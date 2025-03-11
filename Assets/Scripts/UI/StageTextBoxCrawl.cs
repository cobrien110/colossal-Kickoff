using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageTextBoxCrawl : MonoBehaviour
{
    public string[] headers;
    public string[] bodies;
    public TextMeshProUGUI headerBox;
    public TextMeshProUGUI bodyBox;

    [SerializeField] private float headerSpeed = 0.05f;
    [SerializeField] private float bodySpeed = 0.03f;
    [SerializeField] private float scaleSpeed = 2f;

    private Coroutine headerCoroutine;
    private Coroutine bodyCoroutine;
    private Coroutine scaleCoroutine;

    private Vector3 hiddenScale = Vector3.zero;
    private Vector3 visibleScale = new Vector3(3.62f, 3.62f, 3.62f);

    private void Start()
    {
        transform.localScale = hiddenScale; //start hidden
    }

    public void DisplayText(int index)
    {
        if (index < 0 || index >= headers.Length || index >= bodies.Length)
        {
            return;
        }

        if (headerCoroutine != null) StopCoroutine(headerCoroutine);
        if (bodyCoroutine != null) StopCoroutine(bodyCoroutine);

        headerBox.text = "";
        bodyBox.text = "";

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleOverTime(visibleScale));

        headerCoroutine = StartCoroutine(TypeText(headerBox, headers[index], headerSpeed));
        bodyCoroutine = StartCoroutine(TypeText(bodyBox, bodies[index], bodySpeed));
    }

    private IEnumerator TypeText(TextMeshProUGUI textBox, string text, float speed)
    {
        foreach (char letter in text)
        {
            //ADD SOUND EFFECTS SOMEWHERE HERE PLEASE
            textBox.text += letter;
            yield return new WaitForSeconds(speed);
        }
    }

    public void ClearText()
    {
        if (headerCoroutine != null) StopCoroutine(headerCoroutine);
        if (bodyCoroutine != null) StopCoroutine(bodyCoroutine);

        headerBox.text = "";
        bodyBox.text = "";

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleOverTime(hiddenScale));
    }

    private IEnumerator ScaleOverTime(Vector3 targetScale)
    {
        //ADD A SOUND EFFECT HERE FOR THE TEXTBOX APPEARING
        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
