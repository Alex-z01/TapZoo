using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadHandler : MonoBehaviour
{
    private PersistantData _persistantData;

    // Start is called before the first frame update
    void Start()
    {
        _persistantData = GameObject.Find("PersistantData").GetComponent<PersistantData>();
    }

    public void Load(string playerData, string zooData)
    {
        LoadPlayerData(playerData);
        LoadZooData(zooData);
    }

    void LoadPlayerData(string jsonData)
    {
        _persistantData.PlayerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);
    }

    void LoadZooData(string jsonData)
    {
        _persistantData.ZooData = JsonConvert.DeserializeObject<ZooData>(jsonData);
    } 
}
