using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class ShopData : MonoBehaviour
{
    // All shop entires
    public Dictionary<int, StructureShopEntrySObj> AnimalShopEntries = new Dictionary<int, StructureShopEntrySObj>();
    public Dictionary<int, StructureShopEntrySObj> StructureShopEntires = new Dictionary<int, StructureShopEntrySObj>();
    public Dictionary<int, GenericShopEntrySObj> MiscShopEntires = new Dictionary<int, GenericShopEntrySObj>();

    string[] animalFiles = Directory.GetFiles("Assets/ScriptableObjects/ShopEntries/Animals", "*.asset");
    string[] structFiles = Directory.GetFiles("Assets/ScriptableObjects/ShopEntries/Structures", "*.asset");
    string[] miscFiles = Directory.GetFiles("Assets/ScriptableObjects/ShopEntries/Misc", "*.asset");

    private void Awake()
    {
        foreach(string filePath in animalFiles)
        {
            StructureShopEntrySObj entry = AssetDatabase.LoadAssetAtPath<StructureShopEntrySObj>(filePath);

            if(entry != null)
            {
                AnimalShopEntries.Add(entry.id, entry);
            }
        }

        foreach(string filePath in structFiles)
        {
            StructureShopEntrySObj entry = AssetDatabase.LoadAssetAtPath<StructureShopEntrySObj>(filePath);

            if (entry != null)
            {
                StructureShopEntires.Add(entry.id, entry);
            }
        }

        foreach (string filePath in miscFiles)
        {
            GenericShopEntrySObj entry = AssetDatabase.LoadAssetAtPath<GenericShopEntrySObj>(filePath);

            if (entry != null)
            {
                MiscShopEntires.Add(entry.id, entry);
            }
        }
    }

}
