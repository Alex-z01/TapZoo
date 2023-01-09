using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopEntry : MonoBehaviour
{
    public ShopEntrySObj entryData;

    [SerializeField] private TMP_Text _cost;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;

    private UIControls _ui;
    private Shop _shop;

    private void Start()
    {
        _ui = Manager.Instance.uiControls;
        _shop = Manager.Instance.shop;

        _name.text = entryData.entryName;
        _icon.sprite = entryData.icon;
        _cost.text = entryData.cost.ToString();

        gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate { _shop.Purchase(entryData.entryName); });
    }

}
