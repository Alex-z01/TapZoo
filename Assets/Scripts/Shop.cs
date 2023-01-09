using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveStructureEventArgs : EventArgs
{
    public BaseStructure structure;

    public ActiveStructureEventArgs(BaseStructure src)
    {
        structure = src;
    }
}

public class Shop : MonoBehaviour
{
    private Player _player;
    private UIControls _uiControls;
    private StructureData _structureData;

    public event EventHandler<ActiveStructureEventArgs> ActiveStructure;

    public GameObject animalContainer;
    public GameObject entryPrefab;

    private void Start()
    {
        InitializeReferences();

        _structureData.allStructures = _structureData.allStructures.OrderBy(pair => pair.Value.entry.cost).ToDictionary(pair => pair.Key, pair => pair.Value);

        // Instantiate all the shop entries
        foreach (KeyValuePair<int, Structure> pair in _structureData.allStructures)
        {
            GameObject entry = Instantiate(entryPrefab, animalContainer.transform);
            entry.GetComponent<ShopEntry>().entryData = pair.Value.entry;
        }
    }

    private void InitializeReferences()
    {
        _player = Manager.Instance.player;
        _structureData = Manager.Instance.GetComponent<StructureData>();
        _uiControls = Manager.Instance.uiControls;
    }

    public void Purchase(string structureName)
    {
        Structure structure = _structureData.allStructures.Values.Where(value => value.entry.entryName == structureName).First();

        if (_player.zooCoins < structure.entry.cost)
        {
            return;
        }

        _uiControls.ShowLayout("Default");

        // Update PlayerState to Structure
        _player.SetPlayerState(Player.PlayerState.Structure);

        // Update Player selection brush
        _player.selectionWidth = structure.width;
        _player.selectionHeight = structure.height;

        // Spawn the structure
        SpawnStructure(structure);
    }

    public void SpawnStructure(Structure structure, int pos_x = 0, int pos_y = 1, int pos_z = 0)
    {
        GameObject GO = Instantiate(structure.container, new Vector3(pos_x, pos_y, pos_z), Quaternion.identity);
        GO.name = structure.container.name;
        GO.GetComponent<BaseStructure>().SetStructureID(structure.id);
        GO.GetComponent<BaseStructure>().SetStructureState(BaseStructure.StructureState.Unlocked);
        GO.GetComponent<BaseStructure>().UpdateAllPositions(new Vector3(structure.width / 2f, 0, structure.height / 2f));
        _player.SetActiveStructure(GO.GetComponent<BaseStructure>());
    }

    protected virtual void OnActiveStructureEvent(BaseStructure src)
    {
        ActiveStructure?.Invoke(this, new ActiveStructureEventArgs(src));
    }
}
