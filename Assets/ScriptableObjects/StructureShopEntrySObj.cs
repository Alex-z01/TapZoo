using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StructureEntry", menuName = "ScriptableObjects/StructureEntries")]
public class StructureShopEntrySObj : GenericShopEntrySObj
{
    [Space(5)]
    [Header("Structure")]
    public GameObject container;
    public int height, width;
}
