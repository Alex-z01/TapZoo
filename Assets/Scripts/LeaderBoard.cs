using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public struct LeaderBoardEntryData
    {
        public string username;
        public int level;
        public float zooCoins;
    }

    private Networking _networking;

    public GameObject entryContainer;
    public GameObject entryPrefab;

    private void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        _networking = GameObject.Find("PersistantData").GetComponent<Networking>();
    }

    public void SpawnLeaderboard()
    {
        ClearLeaderboard();

        _networking.GetLeaderboard(callback:(List<LeaderBoardEntryData> entries) => {
            Debug.Log(entries.Count);

            int count = 0;

            foreach (LeaderBoardEntryData entry in entries)
            {
                GameObject GO = Instantiate(entryPrefab, entryContainer.transform);
                GO.GetComponent<LeaderboardEntry>().SetValues(count, entry.level, entry.username, entry.zooCoins);
                count++;
            }
        });
    }

    public void ClearLeaderboard()
    {
        foreach(Transform transform in entryContainer.transform)
        {
            Destroy(transform.gameObject);
        }
    }
}
