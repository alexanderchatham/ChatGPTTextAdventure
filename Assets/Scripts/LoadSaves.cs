using System.Collections.Generic;
using UnityEngine;

public class LoadSaves : MonoBehaviour
{
    public GameObject savePrefab;

    void OnEnable()
    {
        LoadAllSaves();
    }

    void LoadAllSaves()
    {
        // Clear out any existing children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Load all saves
        List<story> allPlayerData = JsonDataManager.LoadAllJsonFiles<story>();
        foreach (story playerData in allPlayerData)
        {
            // Instantiate a new save prefab for each save
            GameObject newSavePrefab = Instantiate(savePrefab, transform);
            SavePrefab savePrefabScript = newSavePrefab.GetComponent<SavePrefab>();
            savePrefabScript.UpdateSaveData(playerData);
        }
    }
}
