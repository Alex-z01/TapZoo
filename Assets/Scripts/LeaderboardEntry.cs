using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text indexText;
    public TMP_Text levelText;
    public TMP_Text usernameText;
    public TMP_Text zooCoinsText;
    public TMP_Text animalsText;

    public void SetValues(int index, int level, string username, float zooCoins)
    {
        indexText.text = index.ToString();
        levelText.text = level.ToString();
        usernameText.text = username;
        zooCoinsText.text = zooCoins.ToString();
    }
}
