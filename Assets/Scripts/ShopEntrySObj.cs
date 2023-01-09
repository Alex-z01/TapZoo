using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Entry", menuName = "ScriptableObjects/Entries")]
public class ShopEntrySObj : ScriptableObject
{
    public int cost;
    public string entryName;
    public Sprite icon;
}
