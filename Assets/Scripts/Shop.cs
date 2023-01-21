using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    private ArrayGrid _grid;
    private UIControls _uiControls;
    private ShopData _shopData;

    public event EventHandler<ActiveStructureEventArgs> ActiveStructure;

    public GameObject animalContainer, structContainer, miscContainer;
    public GameObject entryPrefab;

    private void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        _player = Manager.Instance.player;
        _grid = Manager.Instance.grid;
        _shopData = Manager.Instance.GetComponent<ShopData>();
        _uiControls = Manager.Instance.uiControls;
    }

    public void AnimalsTab()
    {
        _uiControls.ShowLayout("Shop", "Shop-Animals");

        _shopData.AnimalShopEntries = _shopData.AnimalShopEntries.OrderBy(pair => pair.Value.cost).ToDictionary(pair => pair.Key, pair => pair.Value);

        if(animalContainer.transform.childCount == 0)
        {
            // Instantiate all the shop entries
            foreach (KeyValuePair<int, StructureShopEntrySObj> pair in _shopData.AnimalShopEntries)
            {
                GameObject entry = Instantiate(entryPrefab, animalContainer.transform);
                entry.GetComponent<ShopEntry>().entryData = pair.Value;
            }
        }
    }

    public void StructureTab()
    {
        _uiControls.ShowLayout("Shop", "Shop-Structures");

        _shopData.StructureShopEntires = _shopData.StructureShopEntires.OrderBy(pair => pair.Value.cost).ToDictionary(pair => pair.Key, pair => pair.Value);

        if (structContainer.transform.childCount == 0)
        {
            // Instantiate all the shop entries
            foreach (KeyValuePair<int, StructureShopEntrySObj> pair in _shopData.StructureShopEntires)
            {
                GameObject entry = Instantiate(entryPrefab, structContainer.transform);
                entry.GetComponent<ShopEntry>().entryData = pair.Value;
            }
        }
    }

    public void MiscTab()
    {
        _uiControls.ShowLayout("Shop", "Shop-Misc");

        _shopData.MiscShopEntires = _shopData.MiscShopEntires.OrderBy(pair => pair.Value.cost).ToDictionary(pair => pair.Key, pair => pair.Value);

        if(miscContainer.transform.childCount == 0)
        {
            // Instantiate all the shop entries
            foreach (KeyValuePair<int, GenericShopEntrySObj> pair in _shopData.MiscShopEntires)
            {
                GameObject entry = Instantiate(entryPrefab, miscContainer.transform);
                entry.GetComponent<ShopEntry>().entryData = pair.Value;
            }
        }
    }

    public void Purchase(GenericShopEntrySObj shopEntry)
    {
        if (_player.zooCoins < shopEntry.cost)
        {
            return;
        }

        if(shopEntry is StructureShopEntrySObj)
        {
            StructureShopEntrySObj structureEntry = (StructureShopEntrySObj)shopEntry;

            _uiControls.ShowLayout("", "Default");

            // Update PlayerState to Structure
            _player.SetPlayerState(Player.PlayerState.Structure);

            // Update Player selection brush
            _player.selectionWidth = structureEntry.width;
            _player.selectionHeight = structureEntry.height;

            // Spawn the structure
            SpawnStructure(structureEntry);
        }


        switch(shopEntry.id)
        {
            case 13:
                ZooExpansion(_grid._lockedSectionIndicies.Min());
                break;
        }
    }

    private void ZooExpansion(int section)
    {
        Debug.Log($"Bought expansion {section}");

        _grid.UnlockSection(section);
        _uiControls.ShowLayout("", "Default");
    }

    public void SpawnStructure(StructureShopEntrySObj structure, int pos_x = 0, int pos_y = 1, int pos_z = 0)
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
