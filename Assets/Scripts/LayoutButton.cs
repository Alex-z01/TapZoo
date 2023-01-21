using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LayoutButton : MonoBehaviour
{
    public UIControls _uiControls;

    public string fatherLayout;
    public string goToLayout;

    private void Awake()
    {
        // Location varies based on scene
        if(SceneManager.GetActiveScene().buildIndex == 0) { _uiControls = GameObject.Find("Scripts").GetComponent<UIControls>(); }
        if(SceneManager.GetActiveScene().buildIndex == 1) { _uiControls = Manager.Instance.uiControls; }
    }

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate { _uiControls.ShowLayout(fatherLayout, goToLayout); });
    }
    
}
