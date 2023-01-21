using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic shop entry scriptable object class
/// </summary>
[CreateAssetMenu(fileName = "Entry", menuName = "ScriptableObjects/Entries")]
public class GenericShopEntrySObj : ScriptableObject
{
    [Header("Generic")]
    public int id;
    public string entryName;
    public int cost;
}
