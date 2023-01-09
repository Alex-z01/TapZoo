using System;
using UnityEngine;

public class OnHoverEventArgs : EventArgs
{
    public GameObject inHandObj { get; }
    public Player.PlayerState playerState { get; }
    public Vector3 point { get; }
    public int selectionWidth { get; }
    public int selectionHeight { get; }

    public OnHoverEventArgs(Vector3 p, Player.PlayerState pState, int selW = 0, int selH = 0)
    {
        point = p;
        selectionWidth = selW;
        selectionHeight = selH;
        playerState = pState;
    }
}

public class Player : MonoBehaviour
{
    #region Properties
    public event EventHandler<OnHoverEventArgs> Hover;

    public enum PlayerState { Idle, Structure, Menu };
    public enum SelectionState { VALID, INVALID };

    [HideInInspector] public int selectionWidth;
    [HideInInspector] public int selectionHeight;

    private Shop _shop;
    private ArrayGrid _grid;
    private HUD _hud;
    private UIControls _uiControls;

    [SerializeField]
    private BaseStructure _activeStructure;

    [SerializeField]
    private PlayerState _playerState;
    private SelectionState _selectionState;

    private StructureData _structureData;
    private PersistantData _persistantData;
    private PlayerData _playerData;

    private int _level;
    public int Level
    {
        get { return _level; }
        set
        {
            _level = value;
            _hud.UpdatePlayerLevel(_level);
        }
    }

    private float _zooCoins;
    public float zooCoins
    {
        get
        {
            return _zooCoins;
        }

        set
        {
            value = Mathf.Round(value);

            if(value < _zooCoins) { _hud.ZooCoinsAnnouncement(_zooCoins-value, false); }
            else { _hud.ZooCoinsAnnouncement(value-_zooCoins, true); }

            _zooCoins = value;
         
            _hud.UpdateZooCoins(_zooCoins);
        }
    }
    #endregion

    private void Start()
    {
        InitializeReferences();
        SubscribeToEvents();
        LoadPlayer();
    }

    private void Update()
    {
        if (_playerState == PlayerState.Idle)
        {
            IdleState();
        }
        else if (_playerState == PlayerState.Structure)
        {
            StructureState();
        }
    }

    #region Initilization
    private void InitializeReferences()
    {
        _persistantData = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        _structureData = GameObject.Find("System").GetComponent<StructureData>();
        _shop = Manager.Instance.shop;
        _grid = Manager.Instance.grid;
        _hud = Manager.Instance.hud;
        _uiControls = Manager.Instance.uiControls;
    }

    private void LoadPlayer()
    {
        _playerData = _persistantData.PlayerData;

        zooCoins = _playerData.zooCoins;
        Level = _playerData.level;
    }

    private void SubscribeToEvents()
    {
        TickSystem.OnTick += OnTick;
        PrimaryEvents.Escape += OnEscape;
    }
    #endregion

    #region GETTERS and SETTERS
    public PlayerState GetPlayerState()
    {
        return _playerState;
    }
    public void SetPlayerState(PlayerState state)
    {
        _playerState = state;
    }

    public BaseStructure GetActiveStructure()
    {
        return _activeStructure;
    }
    public void SetActiveStructure(BaseStructure src)
    {
        _activeStructure = src;
    }

    public SelectionState GetSelectionSate()
    {
        return _selectionState;
    }
    public void SetSelectionState(SelectionState state)
    {
        _selectionState = state;
    }

    public PlayerData GetPlayer()
    {
        _playerData.level = Level;
        _playerData.zooCoins = zooCoins;

        return _playerData;
    }
    #endregion

    #region MANIPULATORS
    public void UpdateBalance(float value)
    {
        zooCoins += value;
    }
    #endregion

    #region StateManagers
    void IdleState()
    {

    }

    void StructureState()
    {
        OrthoMouseRay();

        if (Input.GetMouseButtonDown(0))
        {
            PlaceStructure();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateStructure();
        }
    }
    #endregion

    #region Events
    protected virtual void OnHoverEvent(Vector3 point)
    {
        Hover?.Invoke(this, new OnHoverEventArgs(point, _playerState, selectionWidth, selectionHeight));
    }
    #endregion

    #region SubscribedFunctions
    private void OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        // Do tick based actions here
    }

    private void OnEscape(object sender, PrimaryEvents.OnEscapeEventArgs e)
    {
        print($"{e.playerState}");
        if(e.playerState == PlayerState.Idle)
        {
            if(_uiControls.UI_Layouts.Find(layout => layout.lName == _uiControls.currentLayout).canPause)
            {
                PrimaryEvents.isPaused = PrimaryEvents.isPaused ? false : true;
                _uiControls.PauseLayout();
            }

            return;
        }

        if(e.playerState == PlayerState.Structure)
        {
            CancelStructurePurchase();
            return;
        }
    }
    #endregion

    #region Pure Functionality
    void MouseRay()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 50, Color.red);

        if (Physics.Raycast(Camera.main.transform.position, ray.direction, out hit, Mathf.Infinity, 1 << 6))
        {
            if (hit.collider != null)
            {
                OnHoverEvent(hit.point);
            }
        }
    }

    void OrthoMouseRay()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.red);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << 6))
        {
            if (hit.collider != null)
            {
                OnHoverEvent(hit.point);
            }
        }
    }
    #endregion

    #region IdleState Functionality

    #endregion

    #region StructureState Functionality
    void CancelStructurePurchase()
    {
        _playerState = PlayerState.Idle;

        Destroy(_activeStructure.gameObject);

        selectionHeight = 1;
        selectionWidth = 1;

        _grid.ResetImageGrid();
    }

    void PlaceStructure()
    {
        // If hovering position is valid
        if (ValidatePosition())
        {
            // Update PlayerState to Idle
            SetPlayerState(PlayerState.Idle);

            // Take player's money
            zooCoins -= _structureData.allStructures[_activeStructure.structureID].entry.cost;

            // Update selected cells' CellState to INVALID
            foreach (Cell cell in _grid.selectedCells)
            {
                cell.SetState(Cell.CellState.INVALID);
            }

            // Set the structure's in world position and lock it
            _activeStructure.transform.position = new Vector3(_activeStructure.transform.position.x, 0, _activeStructure.transform.position.z);
            _activeStructure.SetStructureState(BaseStructure.StructureState.Locked);

            _grid.AddBaseStructure(_activeStructure);

            // Reset all hover image
            _grid.ResetImageGrid();

            // Remove the active structure from player
            _activeStructure = null;
        }
        else
        {
            Debug.Log("Can't place structure here");
        }
    }

    void RotateStructure()
    {
        int temp = selectionWidth;
        selectionWidth = selectionHeight;
        selectionHeight = temp;

        _activeStructure.Rotate();
    }

    bool ValidatePosition()
    {
        // Base case
        if (selectionHeight * selectionWidth != _grid.selectedCells.Length)
            return false;

        foreach (Cell cell in _grid.selectedCells)
        {
            if (cell == null || cell.GetState() != Cell.CellState.VALID)
            {
                return false;
            }
        }
        Debug.Log("Valid");
        return true;
    }
    #endregion

    #region Data
    public void SavePlayerData()
    {
        _playerData.level = Level;
        _playerData.zooCoins = zooCoins;
    }
    #endregion






    



    

}
