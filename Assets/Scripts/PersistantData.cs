using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string _id;
    public string username;
    public string email;
    public float zooCoins;
    public int level;
}

[Serializable]
public class ZooData
{
    [Serializable]
    public struct StructureData
    {
        public int id;
        public int pos_x, pos_z;
        public float current_income;
    };

    [Serializable]
    public struct CellData
    {
        public int pos_x, pos_y;
        public int section;
        public Cell.CellState state;
    }

    public int width, height;

    public List<CellData> cells = new List<CellData>();
    public List<StructureData> structures = new List<StructureData>();
    public List<int> lockedSections = new List<int>();
}

public class PersistantData : MonoBehaviour
{
    public PlayerData PlayerData;
    public ZooData ZooData;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void Clean()
    {
        PlayerData = new PlayerData();
        ZooData = new ZooData();
    }
}
