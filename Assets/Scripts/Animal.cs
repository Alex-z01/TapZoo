using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    private string animalName;
    private int animalLevel;

    // To be implemented later
    private int upkeepCost; 

    private void LevelUp()
    {
        animalLevel++;
    }
}
