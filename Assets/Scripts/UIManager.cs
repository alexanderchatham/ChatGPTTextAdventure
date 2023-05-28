using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_InputField characterNameInputField;
    public TMP_Dropdown themeDropdown;
    public TMP_Dropdown characterGenderDropdown;
    public Slider storyLengthSlider;
    public TMP_Dropdown genreDropdown;

    private string theme;
    private string characterName;
    private string characterGender;
    private string storyLength;
    private string genre;

    // Initialize settings with default values
    void Start()
    {
        theme = themeDropdown.options[themeDropdown.value].text;
        characterName = characterNameInputField.text;
        characterGender = characterGenderDropdown.options[characterGenderDropdown.value].text;
        storyLength = getLengthString((int)storyLengthSlider.value);
        genre = genreDropdown.options[genreDropdown.value].text;
    }
    public string getLengthString(int i)
    {
        switch (i)
        {
            case 0:
                return "small";
            case 1:
                return "medium";
            case 2:
                return "large";
            default:
                return "small";
        }
    }

    // These methods are called when the corresponding UI element's value changes
    public void OnThemeChange()
    {
        theme = themeDropdown.options[themeDropdown.value].text;
    }

    public void OnCharacterNameChange()
    {
        characterName = characterNameInputField.text;
    }

    public void OnCharacterGenderChange()
    {
        characterGender = characterGenderDropdown.options[characterGenderDropdown.value].text;
    }

    public void OnStoryLengthChange()
    {
        storyLength = getLengthString((int)storyLengthSlider.value);
        
    }

    public void OnGenreChange()
    {
        genre = genreDropdown.options[genreDropdown.value].text;
    }


    // Getter methods to access these values from other scripts
    public string GetTheme() { return theme; }
    public string GetCharacterName() { return characterName; }
    public string GetCharacterGender() { return characterGender; }
    public string GetStoryLength() { return storyLength; }
    public string GetGenre() { return genre; }
}

