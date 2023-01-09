using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIncomeStructure
{
    void Income();
    void CollectIncome();
    void ToggleIncomeUI(bool value);
}
