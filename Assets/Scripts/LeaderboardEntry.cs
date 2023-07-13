using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void Setup(LeaderboardManager.PlayerData playerData)
    {
        rankText.text = ((int)playerData.rank+1).ToString();
        nameText.text = playerData.playerName;
        scoreText.text = playerData.score.ToString("F0");
    }
}