using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameSettings : MonoBehaviour
{
    public List<TMP_FontAsset> fonts;
    public List<GradientData> gradients;
    private int currentFontIndex;
    private int currentGradientIndex;

    private const string FONT_INDEX_KEY = "FontIndex";
    private const string GRADIENT_INDEX_KEY = "GradientIndex";
    
    public Image background;
    private void Start()
    {
        LoadSettings();
        UpdateFonts();
        UpdateGradients();
    }
    private void LoadSettings()
    {
        currentFontIndex = PlayerPrefs.GetInt(FONT_INDEX_KEY, 0);
        currentGradientIndex = PlayerPrefs.GetInt(GRADIENT_INDEX_KEY, 0);
    }
    public void IncrementFontIndex()
    {
        currentFontIndex = (currentFontIndex + 1) % fonts.Count;
        PlayerPrefs.SetInt(FONT_INDEX_KEY, currentFontIndex);
        UpdateFonts();
    }

    private void UpdateFonts()
    {
        if (fonts.Count == 0)
        {
            Debug.LogWarning("No fonts set in GameSettings.");
            return;
        }

        TMP_FontAsset currentFont = fonts[currentFontIndex];

        // Get all TextMeshProUGUI components that are descendants of this GameObject
        TextMeshProUGUI[] textMeshPros = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI tmp in textMeshPros)
        {
            tmp.font = currentFont;
        }
    }
    
    public void IncrementGradientIndex()
    {
        currentGradientIndex = (currentGradientIndex + 1) % gradients.Count;
        PlayerPrefs.SetInt(GRADIENT_INDEX_KEY, currentGradientIndex);
        UpdateGradients();
    }
    
    private void UpdateGradients()
    {
        if (gradients.Count == 0)
        {
            Debug.LogWarning("No gradients set in GameSettings.");
            return;
        }

        GradientData currentGradient = gradients[currentGradientIndex];

        // Get all Renderer components that are descendants of this GameObject
        
        if (background.material.HasProperty("_ColorTop") && background.material.HasProperty("_ColorBottom"))
        {
            background.material.SetColor("_ColorTop", currentGradient.topColor);
            background.material.SetColor("_ColorBottom", currentGradient.bottomColor);
        }
    }
}
[System.Serializable]
public class GradientData
{
    public string name;
    public Color topColor;
    public Color bottomColor;
}