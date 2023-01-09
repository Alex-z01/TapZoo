using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Runtime.Serialization;

[Serializable]
public class UI_Layout
{
    public string lName;

    public GameObject layout;

    [OptionalField]
    public TMP_Text responseObject;

    [SerializeField]
    private bool state;
    public bool State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
            ActiveState();
        }
    }

    public bool hasPrevious;
    public bool canPause;

    public void ActiveState()
    {
        layout.SetActive(State);
    }
}

public class UIControls : MonoBehaviour
{
    public string previousLayout;

    public string currentLayout;

    [OptionalField]
    public GameObject backButton;

    public List<UI_Layout> UI_Layouts = new List<UI_Layout>();

    private void Start()
    {
        ShowLayout();
    }

    // Method to show a specific UI_Layout
    public void ShowLayout(string lName = "Default")
    {
        previousLayout = currentLayout;
        currentLayout = lName;

        SetResponseMessage("");

        foreach(UI_Layout layout in UI_Layouts)
        {
            if(layout.lName == lName)
            {
                layout.State = true;

                if(layout.hasPrevious) { backButton.SetActive(true); }
                else { backButton.SetActive(false); }
            }
            else
            {
                layout.State = false;
            }
        }
    }

    public void PauseLayout()
    {
        if (PrimaryEvents.isPaused) { ShowLayout("Pause"); }
        else { ShowLayout("Default"); }
    }

    public void PreviousLayout()
    {
        ShowLayout(previousLayout);
    }

    public void SetResponseMessage(string msg)
    {
        foreach(UI_Layout layout in UI_Layouts)
        {
            if(layout.responseObject && layout.lName == currentLayout)
            {
                layout.responseObject.text = msg;
            }
        }
    }
}
