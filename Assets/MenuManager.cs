using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject newGamePanel;
    public GameObject loadGamePanel;
    public GameObject gamePanel;

    // You can also reference your GameManager here
    // public GameManager gameManager;

    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void ShowNewGamePanel()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);
        loadGamePanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    public void ShowLoadGamePanel()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    public void ContinueGame()
    {
        // You can add the logic of loading the last game here
        // e.g. gameManager.LoadLastGame();

        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);
        gamePanel.SetActive(true);
    }
}