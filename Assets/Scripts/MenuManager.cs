using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    public CanvasGroup mainMenuPanel;
    public CanvasGroup newGamePanel;
    public CanvasGroup loadGamePanel;
    public CanvasGroup gamePanel;
    public Button continueButton;

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

        while (Time.time < startTime + fadeTime)
        {
            panel.alpha = (Time.time - startTime) / fadeTime;
            yield return null;
        }

        panel.alpha = 1f;
    }

    void SetPanelInactive(CanvasGroup panel)
    {
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }

    public void ShowMainMenu()
    {
        SetPanelInactive(newGamePanel);
        SetPanelInactive(loadGamePanel);
        SetPanelInactive(gamePanel);
        StartCoroutine(FadeIn(mainMenuPanel));
    }

    public void ShowNewGamePanel()
    {
        SetPanelInactive(mainMenuPanel);
        SetPanelInactive(loadGamePanel);
        SetPanelInactive(gamePanel);
        StartCoroutine(FadeIn(newGamePanel));
    }

    public void ShowGamePanel()
    {
        SetPanelInactive(mainMenuPanel);
        SetPanelInactive(loadGamePanel);
        SetPanelInactive(newGamePanel);
        StartCoroutine(FadeIn(gamePanel));
    }

    public void ShowLoadGamePanel()
    {
        SetPanelInactive(mainMenuPanel);
        SetPanelInactive(newGamePanel);
        SetPanelInactive(gamePanel);
        StartCoroutine(FadeIn(loadGamePanel));
    }

    public void ContinueGame()
    {
        // You can add the logic of loading the last game here
        // e.g. gameManager.LoadLastGame();

        SetPanelInactive(mainMenuPanel);
        SetPanelInactive(newGamePanel);
        SetPanelInactive(loadGamePanel);
        StartCoroutine(FadeIn(gamePanel));
    }
}