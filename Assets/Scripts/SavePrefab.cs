using UnityEngine;
using TMPro;

public class SavePrefab : MonoBehaviour
{
    public TextMeshProUGUI saveNameText;
    private story storySave;

    public void UpdateSaveData(story playerData)
    {
        storySave = playerData;
        saveNameText.text = playerData.character +"\n"+playerData.summary;
        // Add more fields as needed
    }
    public void LoadThisSave()
    {
        FindObjectOfType<OpenAI.StreamResponse>().finalString = JsonUtility.ToJson(storySave);
        FindObjectOfType<MenuManager>().ShowGamePanel();
        FindObjectOfType<ContentHandler>().contentLinker();
    }
}
