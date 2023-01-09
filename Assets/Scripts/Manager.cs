using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;

    public ArrayGrid grid;
    public HUD hud;
    public Player player;
    public Shop shop;
    public UIControls uiControls;

    public static Manager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<Manager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
