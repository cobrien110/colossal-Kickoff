using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HSVSlider : MonoBehaviour
{
    [Header("UI References")]
    public Slider hueSlider;
    public Slider saturationSlider;
    public Slider valueSlider;
    public TMP_InputField hexInputField;
    public Image previewImage;

    [Header("Slider Backgrounds")]
    public Image hueBackground;
    public Image saturationBackground;
    public Image valueBackground;

    [Header("Output Color")]
    public Color selectedColor;

    [Header("Defaults")]
    [Tooltip("Set the starting color for the HSV picker.")]
    public Color defaultColor = new Color(1f, 0f, 0f);

    private bool isUpdating = false;
    private float lastHue = -1f;

    void Start()
    {
        hueSlider.onValueChanged.AddListener(_ => UpdateColorFromSliders());
        saturationSlider.onValueChanged.AddListener(_ => UpdateColorFromSliders());
        valueSlider.onValueChanged.AddListener(_ => UpdateColorFromSliders());
        hexInputField.onEndEdit.AddListener(UpdateColorFromHex);

        SetColor(defaultColor);
        GenerateHueGradient();
    }

    void UpdateColorFromSliders()
    {
        if (isUpdating) return;
        isUpdating = true;

        float h = hueSlider.value;
        float s = saturationSlider.value;
        float v = valueSlider.value;

        selectedColor = Color.HSVToRGB(h, s, v);
        hexInputField.text = ColorUtility.ToHtmlStringRGB(selectedColor);
        ApplyColorToShader(selectedColor);

        UpdateSliderBackgrounds(h, s, v);
        lastHue = h;

        isUpdating = false;
    }

    void UpdateColorFromHex(string hex)
    {
        if (isUpdating) return;
        isUpdating = true;

        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        if (ColorUtility.TryParseHtmlString(hex, out Color newColor))
        {
            selectedColor = newColor;
            Color.RGBToHSV(newColor, out float h, out float s, out float v);

            hueSlider.value = h;
            saturationSlider.value = s;
            valueSlider.value = v;

            ApplyColorToShader(newColor);
            UpdateSliderBackgrounds(h, s, v);
            lastHue = h;
        }

        isUpdating = false;
    }

    public void SetColor(Color color)
    {
        selectedColor = color;
        Color.RGBToHSV(color, out float h, out float s, out float v);

        isUpdating = true;
        hueSlider.value = h;
        saturationSlider.value = s;
        valueSlider.value = v;
        hexInputField.text = "#" + ColorUtility.ToHtmlStringRGB(color);

        ApplyColorToShader(color);
        UpdateSliderBackgrounds(h, s, v);
        lastHue = h;

        isUpdating = false;
    }

    void ApplyColorToShader(Color color)
    {
        if (previewImage != null && previewImage.material != null)
        {
            previewImage.material.SetColor("_ShirtColor", color);
        }
    }

    void UpdateSliderBackgrounds(float h, float s, float v)
    {
        // Saturation
        if (saturationBackground != null)
        {
            Color left = Color.HSVToRGB(h, 0f, v);
            Color right = Color.HSVToRGB(h, 1f, v);

            var tex = new Texture2D(2, 1);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(new[] { left, right });
            tex.Apply();

            saturationBackground.sprite = Sprite.Create(tex, new Rect(0, 0, 2, 1), new Vector2(0.5f, 0.5f));
            saturationBackground.type = Image.Type.Simple;
            saturationBackground.preserveAspect = false;
            saturationBackground.rectTransform.anchorMin = new Vector2(0, 0);
            saturationBackground.rectTransform.anchorMax = new Vector2(1, 1);
            saturationBackground.rectTransform.offsetMin = Vector2.zero;
            saturationBackground.rectTransform.offsetMax = Vector2.zero;
        }

        // Value
        if (valueBackground != null)
        {
            Color left = Color.black;
            Color right = Color.HSVToRGB(h, s, 1f);

            var tex = new Texture2D(2, 1);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(new[] { left, right });
            tex.Apply();

            valueBackground.sprite = Sprite.Create(tex, new Rect(0, 0, 2, 1), new Vector2(0.5f, 0.5f));
            valueBackground.type = Image.Type.Simple;
            valueBackground.preserveAspect = false;
            valueBackground.rectTransform.anchorMin = new Vector2(0, 0);
            valueBackground.rectTransform.anchorMax = new Vector2(1, 1);
            valueBackground.rectTransform.offsetMin = Vector2.zero;
            valueBackground.rectTransform.offsetMax = Vector2.zero;
        }
    }

    void GenerateHueGradient()
    {
        if (hueBackground == null) return;

        int width = 360;
        Texture2D tex = new Texture2D(width, 1);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[width];

        for (int i = 0; i < width; i++)
        {
            float h = (float)i / (width - 1);
            colors[i] = Color.HSVToRGB(h, 1f, 1f);
        }

        tex.SetPixels(colors);
        tex.Apply();

        hueBackground.sprite = Sprite.Create(tex, new Rect(0, 0, width, 1), new Vector2(0.5f, 0.5f));
        hueBackground.type = Image.Type.Simple;
        hueBackground.preserveAspect = false;
        hueBackground.rectTransform.anchorMin = new Vector2(0, 0);
        hueBackground.rectTransform.anchorMax = new Vector2(1, 1);
        hueBackground.rectTransform.offsetMin = Vector2.zero;
        hueBackground.rectTransform.offsetMax = Vector2.zero;
    }
}