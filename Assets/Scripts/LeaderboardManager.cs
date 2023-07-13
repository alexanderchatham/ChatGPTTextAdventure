using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;
    public string LeaderboardId;
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContainer;

    public TextMeshProUGUI pointsText;
    // The class that stores player data
    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
        public string playerName;
        public int rank;
        public double score;
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public int limit;
        public int total;
        public List<PlayerData> results;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }

    public async void LoadLeaderboard()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
   
        // Parse the JSON string into our defined class structure
        LeaderboardData leaderboardData = JsonConvert.DeserializeObject<LeaderboardData>(JsonConvert.SerializeObject(scoreResponse));

        // Clear current leaderboard
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate a prefab for each player in the leaderboard
        foreach (PlayerData playerData in leaderboardData.results)
        {
            GameObject leaderboardEntryObject = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            LeaderboardEntry leaderboardEntry = leaderboardEntryObject.GetComponent<LeaderboardEntry>();

            if (leaderboardEntry != null)
            {
                leaderboardEntry.Setup(playerData);
            }
        }
    }

    public TMP_InputField InputField;
    public void SetLeaderboardName()
    {
        if(InputField.text.Length<50)
            AuthenticationService.Instance.UpdatePlayerNameAsync(InputField.text.Replace(" ",""));
    }
    public async void AddScore(int i)
    {
        pointsText.text = "Your Points: " + i;
        await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, i);
    }
}
