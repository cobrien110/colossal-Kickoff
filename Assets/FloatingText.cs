using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public string[] textToShow;
    public List<Color> colors;

    public float moveSpeed = 0.5f;
    public float colorCycleSpeed = 0.2f;
    private float colorTimer = 0;
    public float alphaLossPerSecond = 0.25f;
    public float alphaTracker = 1.5f;

    public TextMeshPro text;
    private int count = 0;

    private AudioPlayer AP;
    // Start is called before the first frame update
    void Start()
    {
        if (colors.Count < 1)
        {
            colors.Add(Color.white);
        }
        ChangeColor();

        text.text = textToShow[Random.Range(0, textToShow.Length)];

        AP = GetComponent<AudioPlayer>();
        AP.PlaySoundRandomPitch(AP.sounds[0]);
    }

    // Update is called once per frame
    void Update()
    {
        colorTimer += Time.deltaTime;
        if (colorTimer >= colorCycleSpeed)
        {
            ChangeColor();
            colorTimer = 0f;
        }

        text.transform.position = new Vector3(text.transform.position.x, text.transform.position.y + (moveSpeed * Time.deltaTime),
            text.transform.position.z);

        alphaTracker -= alphaLossPerSecond * Time.deltaTime;
    }

    private void ChangeColor()
    {
        Color c = colors[count];
        text.color = new Color(c.r, c.g, c.b, alphaTracker);

        count++;
        if (count >= colors.Count) count = 0;
    }
}
