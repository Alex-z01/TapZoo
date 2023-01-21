using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic shop entry class
/// </summary>
public class ShopEntry : MonoBehaviour
{
    public GenericShopEntrySObj entryData;

    [SerializeField] private TMP_Text _cost;
    //[SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;

    private UIControls _ui;
    private Shop _shop;

    private void Start()
    {
        _ui = Manager.Instance.uiControls;
        _shop = Manager.Instance.shop;

        Setup();
        
        // All shop entries call the purchase function

        gameObject.GetComponent<Button>().onClick.AddListener(delegate { _shop.Purchase(entryData); });
        
    }
   
    private void Setup()
    {
        if (_name) { _name.text = entryData.entryName; }
        //if (_icon) { _icon.sprite = entryData.icon; }
        if (_cost) { _cost.text = entryData.cost.ToString("##,##"); }
    }

}
