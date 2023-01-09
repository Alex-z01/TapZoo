using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "ScriptableObjects/Structure")]
public class Structure : ScriptableObject
{
    public GameObject container;

    public int id;

    public int width;
    public int height;

    public ShopEntrySObj entry;
}