using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Runtime.Serialization;

[Serializable]
public class UI_Layout
{
    public string FatherLayout;
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
    public void ShowLayout(string fName = "", string lName = "Default")
    {
        // Find the current layout
        UI_Layout cLayout = UI_Layouts.Find(layout => layout.lName == currentLayout);
        // If it exists and has no father layout, assign it to be the previous layout
        if(cLayout != null && cLayout.FatherLayout == "")
        {
            previousLayout = currentLayout;
        }
        // Update the current layout
        currentLayout = lName;

        SetResponseMessage("");

        // If the given layout is a father layout all other father layouts should be disabled
        if (fName == "")
        {
            foreach (UI_Layout layout in UI_Layouts)
            {
                if(layout.FatherLayout == "")
                {
                    layout.State = false;
                }
                if(layout.lName == lName)
                {
                    layout.State = true;

                    if (layout.hasPrevious) { backButton.SetActive(true); }
                    else { backButton.SetActive(false); }
                }
            }
        }
        // If the given layout has a father layout, enable the father layout, then the child
        // only disable other layouts with the same father
        else
        {
            foreach (UI_Layout layout in UI_Layouts)
            {
                if(fName == layout.lName)
                {
                    layout.State = true;
                }
                if (lName == layout.lName)
                {
                    layout.State = true;
                }
                if (lName != layout.lName && fName == layout.FatherLayout)
                {
                    layout.State = false;
                }
            }
        }
    }

    public void PauseLayout()
    {
        if (PrimaryEvents.isPaused) { ShowLayout("", "Pause"); }
        else { ShowLayout("", "Default"); }
    }

    public void PreviousLayout()
    {
        ShowLayout("", previousLayout);
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
