using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    public CanvasGroup mainMenuPanel;
    public CanvasGroup newGamePanel;
    public CanvasGroup settingsPanel;
    public CanvasGroup loadGamePanel;
    public CanvasGroup gamePanel;
    public CanvasGroup leaderboardPanel;
    public CanvasGroup dailyRewardsPanel;
    public Button continueButton;

    public List<CanvasGroup> Panels;
    private story lastStory;

    public float fadeTime = 1f; // Fade duration in seconds

    void Start()
    {
        checkForSave();
        ShowMainMenu();
    }

    void checkForSave()
    {
        lastStory = JsonDataManager.LoadFirstSave<story>();
        if (lastStory != null)
        {
            FindObjectOfType<OpenAI.StreamResponse>().finalString = JsonUtility.ToJson(lastStory);
            continueButton.interactable = true;
        }
        else
            continueButton.interactable = false;
    }

    IEnumerator FadeIn(CanvasGroup panel)
    {
        float startTime = Time.time;
        panel.interactable = true;
        panel.blocksRaycasts = true;

        while (Time.time < startTime + fadeTime && panel.interactable)
        {
            panel.alpha = (Time.time - startTime) / fadeTime;
            yield return null;
        }

        if(panel.interactable)
            panel.alpha = 1f;
    }

    void SetPanelInactive(CanvasGroup panel)
    {
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    void SetAllInactive()
    {
        foreach (var VARIABLE in Panels)
        {
            SetPanelInactive(VARIABLE);
        }
    }

    public void ShowMainMenu()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(mainMenuPanel));
    }
    public void ShowSettingsPanel()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(settingsPanel));
    }


    public void ShowNewGamePanel()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(newGamePanel));
    }

    public void ShowGamePanel()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(gamePanel));
    }
    public void ShowLeaderboardPanel()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(leaderboardPanel));
    }
    public void ShowDailyRewardsPanel()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(dailyRewardsPanel));
    }

    public void ShowLoadGamePanel()
    {
        SetAllInactive();
        StartCoroutine(FadeIn(loadGamePanel));
    }

    public void ContinueGame()
    {
        // You can add the logic of loading the last game here
        // e.g. gameManager.LoadLastGame();

        SetAllInactive();
        StartCoroutine(FadeIn(gamePanel));
    }
}