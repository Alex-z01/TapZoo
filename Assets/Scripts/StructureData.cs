using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class StructureData : MonoBehaviour
{
    public Dictionary<int, Structure> allStructures = new Dictionary<int, Structure>();

    string[] fileNames = Directory.GetFiles("Assets/ScriptableObjects/Animals", "*.asset");

    private void Awake()
    {
        foreach(string filePath in fileNames)
        {
            Structure structure = AssetDatabase.LoadAssetAtPath<Structure>(filePath);

            if(structure != null)
            {
                allStructures.Add(structure.id, structure);
            }
        }
    }

}
