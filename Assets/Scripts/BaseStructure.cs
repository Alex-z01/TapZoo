using System;
using System.Linq;
using UnityEngine;

public abstract class BaseStructure : MonoBehaviour, IBaseStructure
{
    #region Properties
    public enum StructureState { Unlocked, Locked };

    [Header("Base Info")]
    public int structureID;
    public Canvas baseCanvas;
    public GameObject structureObject;

    [SerializeField]
    [Tooltip("Unlocked: Structure hasn't been placed\nLocked: Structure is placed")]
    private StructureState _structureState;

    private Player _player;
    private Shop _shop;
    #endregion

    public abstract void OnTick(object sender, TickSystem.OnTickEventArgs e);

    private void Start()
    {
        _player = Manager.Instance.player;
        _shop = Manager.Instance.shop;

        SubscribeToEvents();
    }

    private void Update()
    {
        if(_structureState.Equals(StructureState.Unlocked))
        {
            UpdateModelPosition(new Vector3(_player.selectionWidth / 2f, 0, _player.selectionHeight / 2f));   
        }
    }

    public void UpdateAllPositions(Vector3 pos)
    {
        UpdateModelPosition(pos);
        UpdateCanvasPositions(pos);
        UpdateColliderCenter(pos);
    }

    public void UpdateModelPosition(Vector3 pos)
    {
        structureObject.transform.localPosition = new Vector3(pos.x, pos.y, pos.z); 
    }

    public void UpdateColliderCenter(Vector3 pos)
    {
        GetComponent<BoxCollider>().center += pos;
    }

    public void UpdateCanvasPositions(Vector3 pos)
    {
        foreach(Transform transform in this.transform)
        {
            if(transform.GetComponent<Canvas>())
            {
                transform.position += pos;
            }
        }
    }

    public abstract void OnMouseDown();

    public void Rotate()
    {
        structureObject.transform.localPosition = new Vector3(
            structureObject.transform.localPosition.z,
            structureObject.transform.localPosition.y,
            structureObject.transform.localPosition.x
            );

        GetComponent<BoxCollider>().center = new Vector3(
            GetComponent<BoxCollider>().center.z,
            GetComponent<BoxCollider>().center.y,
            GetComponent<BoxCollider>().center.x
            );

        GetComponent<BoxCollider>().size = new Vector3(
            GetComponent<BoxCollider>().size.z,
            GetComponent<BoxCollider>().size.y,
            GetComponent<BoxCollider>().size.x
            );

        structureObject.transform.Rotate(0, 90, 0);
    }

    public void ToggleBaseUI(bool value)
    {
        baseCanvas.gameObject.SetActive(value);
    }

    public void SetStructureID(int id)
    {
        structureID = id;
    }

    public void SubscribeToEvents()
    {
        TickSystem.OnTick += OnTick;
    }

    public void UnsubscribeFromEvents()
    {
        TickSystem.OnTick -= OnTick;
    }

    public StructureState GetStructureState() { return _structureState; }
    public void SetStructureState(StructureState structureState) { _structureState = structureState; }

    public Player GetPlayer() { return _player; }

    public Shop GetShop() { return _shop; }
}
