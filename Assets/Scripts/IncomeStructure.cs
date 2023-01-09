using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomeStructure : BaseStructure, IIncomeStructure
{
    #region Properties
    [Space(5)]
    [Header("Income Info")]
    [SerializeField]
    private float income_per_tick;
    [SerializeField]
    private float income_to_collect;
    [SerializeField]
    private float max_income_stored;
    [SerializeField]
    private float _current_income;

    [Space(5)]
    [SerializeField]
    private Canvas _incomeCanvas;
    #endregion

    public void Income()
    {
        if (_current_income < max_income_stored)
        {
            _current_income += income_per_tick;
        }
    }

    public void CollectIncome()
    {
        if(_current_income >= income_to_collect && !PrimaryEvents.isPaused)
        {
            GetPlayer().UpdateBalance(_current_income);
            _current_income -= _current_income;
        }
    }

    public float GetCurrentIncome()
    {
        return _current_income;
    }

    public void SetCurrentIncome(float val)
    {
        _current_income = val;
    }

    public void ToggleIncomeUI(bool value)
    {
        _incomeCanvas.gameObject.SetActive(value);
    }

    public override void OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if(GetStructureState() == StructureState.Locked)
        {
            Income();

            if (_current_income >= income_to_collect) { ToggleIncomeUI(true); }
            else { ToggleIncomeUI(false); }
        }
    }

    public override void OnMouseDown()
    {
        //Debug.Log($"Clicked on base structure {name}");
        bool val = baseCanvas.gameObject.activeSelf ? true : false;
        ToggleBaseUI(val);

        CollectIncome();
    }
}
