using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonDataManager
{
    static readonly string directoryPath = Application.persistentDataPath;

    public static void SaveJson<T>(T data, string fileName)
    {
        string filePath = Path.Combine(directoryPath, fileName + ".json");
        string jsonData = JsonUtility.ToJson(data, true);

        if (File.Exists(filePath))
        {
            string existingData = File.ReadAllText(filePath);
            existingData += jsonData;
            File.WriteAllText(filePath, existingData);
        }
        else
        {
            File.WriteAllText(filePath, jsonData);
        }
    }

    public static T LoadJson<T>(string fileName) where T : new()
    {
        string filePath = Path.Combine(directoryPath, fileName + ".json");

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found.");
            return new T();
        }

        string jsonData = File.ReadAllText(filePath);
        return JsonUtility.FromJson<T>(jsonData);
    }

    public static List<T> LoadAllJsonFiles<T>() where T : new()
    {
        List<T> dataList = new List<T>();
        string[] files = Directory.GetFiles(directoryPath);

        foreach (string file in files)
        {
            if (Path.GetExtension(file) == ".json")
            {
                string jsonData = File.ReadAllText(file);
                T data = JsonUtility.FromJson<T>(jsonData);
                dataList.Add(data);
            }
        }

        return dataList;
    }
}