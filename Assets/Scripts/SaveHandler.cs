using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    private Networking _networking;
    private PersistantData _persistantData;
    private Player _player;
    private ArrayGrid _grid;

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            _persistantData = GameObject.Find("PersistantData").GetComponent<PersistantData>();
            _networking = GameObject.Find("PersistantData").GetComponent<Networking>();

            _player = Manager.Instance.player;
            _grid = Manager.Instance.grid;

            InvokeRepeating("Save", 60 * 5, 60 * 5);
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
    }

    public void Save()
    {
        SavePlayerData();
        SaveZooData();
    }

    void SavePlayerData()
    {
        // TODO save player level and zoo coins
        _player.SavePlayerData();
        _networking.UpdatePlayer(_persistantData.PlayerData._id, _player.GetPlayer());
    }


    void SaveZooData()
    {
        _grid.SaveZooData();
        _networking.UpdateZoo(_persistantData.PlayerData._id, _grid.GetZoo());
    }
}
