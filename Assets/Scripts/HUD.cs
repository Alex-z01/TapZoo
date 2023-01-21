using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public TMP_Text zooCoinsText;
    public TMP_Text zooCoinsAnnouncementText;
    public TMP_Text playerLevelText;

    public void UpdateZooCoins(float coins)
    {
        zooCoinsText.text = coins.ToString();
    }

    public void UpdatePlayerLevel(int level)
    {
        if(playerLevelText) { playerLevelText.text = level.ToString();}
    }

    public void ZooCoinsAnnouncement(float coins, bool isIncrease)
    {
        zooCoinsAnnouncementText.gameObject.SetActive(true);

        if (isIncrease) { 
            zooCoinsAnnouncementText.color = Color.black;
            SetZooCoinsAnnouncementText('+', coins);
        }
        else { 
            zooCoinsAnnouncementText.color = Color.red;
            SetZooCoinsAnnouncementText('-', coins);
        }

        StartCoroutine(ZooCoinsAnnouncementFadeOut(3f));
    }

    private void SetZooCoinsAnnouncementText(char op, float coins)
    {
        zooCoinsAnnouncementText.text = op + coins.ToString();
    }

    IEnumerator ZooCoinsAnnouncementFadeOut(float time)
    {
        yield return new WaitForSeconds(time);
        zooCoinsAnnouncementText.gameObject.SetActive(false);
    }
}
