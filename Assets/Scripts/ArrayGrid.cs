using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ArrayGrid : MonoBehaviour
{
    #region Properties
    [SerializeField] private Cell[,] _cells = new Cell[0, 0];
    private Image[,] _cellImages;
    private int _cellW = 1; int _cellL = 1;
    private int _gridW, _gridL;
    [SerializeField]private List<BaseStructure> _placedStructures = new List<BaseStructure>();
    private float _realGridW, _realGridL;

    [SerializeField]private ZooData zooData;
    [Space(4)]


    private PersistantData _persistantData;
    private ShopData shopData;
    private Player _player;

    public Image cellImage;
    public RectTransform gridCanvas;
    public Cell[,] selectedCells;
    public GameObject world;

    [Serializable]
    public struct Section
    {
        public Vector2 bottom_left, top_right;
        public bool locked;
    }
    private List<Section> _sections = new List<Section>();
    public List<int> _lockedSectionIndicies = new List<int>();
    #endregion

    private void Start()
    {
        _persistantData = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        shopData = Manager.Instance.GetComponent<ShopData>();
        _player = Manager.Instance.player;

        LoadZoo();

        SubscribeToEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            int unlocked = UnityEngine.Random.Range(1, _lockedSectionIndicies.Count);
            print($"Unlocked section {unlocked}!");

            UpdateSectionLockState(unlocked, false);
        }
    }

    private void LoadZoo()
    {
        // Copy the ZooData recieved from the server
        zooData = _persistantData.ZooData;

        if (zooData.lockedSections.Count == 0) { print($"No locked sections loaded"); }
        _lockedSectionIndicies = zooData.lockedSections;

        EstablishGrid();
        CreateGrid();

        foreach (ZooData.CellData data in zooData.cells)
        {
            _cells[data.pos_x, data.pos_y].SetState(data.state);
        }

        foreach (ZooData.StructureData data in zooData.structures)
        {
            AddBaseStructure(SpawnStructureFromLoad(data));
        }
    }

    private void OnApplicationQuit()
    {
        SaveZooData();
    }

    private BaseStructure SpawnStructureFromLoad(ZooData.StructureData data)
    {
        StructureShopEntrySObj structureEntry = shopData.AnimalShopEntries[data.id];
        GameObject GO = Instantiate(structureEntry.container, new Vector3(data.pos_x, 0, data.pos_z), Quaternion.identity);
        GO.name = structureEntry.container.name;
        GO.GetComponent<BaseStructure>().SetStructureID(structureEntry.id);
        GO.GetComponent<BaseStructure>().SetStructureState(BaseStructure.StructureState.Locked);
        GO.GetComponent<BaseStructure>().UpdateAllPositions(new Vector3(structureEntry.width / 2f, 0, structureEntry.height / 2f));
        if(GO.GetComponent<IncomeStructure>())
        {
            GO.GetComponent<IncomeStructure>().SetCurrentIncome(data.current_income);
        }
        return GO.GetComponent<BaseStructure>();
    }

    private void EstablishGrid()
    {
        world.transform.localScale = new Vector3(zooData.width, 1, zooData.height);

        world.transform.position = new Vector3(world.transform.localScale.x / 2f, -world.transform.localScale.y / 2f, world.transform.localScale.z / 2f);

        _realGridW = world.transform.localScale.x;
        _realGridL = world.transform.localScale.z;

        _gridW = (int)_realGridW / _cellW;
        _gridL = (int)_realGridL / _cellL;

        _cells = new Cell[_gridW, _gridL];
        _cellImages = new Image[_gridW, _gridL];

        gridCanvas.sizeDelta = new Vector2(_realGridW, _realGridL);
        gridCanvas.transform.position = new Vector3(world.transform.position.x, 0.001f, world.transform.position.z);
    }

    private void SubscribeToEvent()
    {
        _player.Hover += OnHover;
    }

    private void OnHover(object sender, OnHoverEventArgs e)
    {
        ResetImageGrid();

        int hit_x = Mathf.FloorToInt(e.point.x);
        int hit_y = Mathf.FloorToInt(e.point.z);

        int selection_area = e.selectionWidth * e.selectionHeight;

        bool selectedCellsValid = false;

        // Get the neighboring cells of hovered cell, including hovered cell
        selectedCells = GetCellNeighbors(hit_x, hit_y, e.selectionWidth, e.selectionHeight);

        foreach (Cell cell in selectedCells)
        {
            selectedCellsValid = true;
            if(cell.IsUnityNull() || cell.GetState() != Cell.CellState.VALID)
            {
                selectedCellsValid = false;
                break;
            }
        }

        HoverUI();

        if (selection_area == CountNonNullElements(selectedCells) &&
            selectedCellsValid)
        {
            _player.SetSelectionState(Player.SelectionState.VALID);
        }
        else
        {
            _player.SetSelectionState(Player.SelectionState.INVALID);
        }

        if (e.playerState == Player.PlayerState.Structure)
        {
            _player.GetActiveStructure().gameObject.transform.position = new Vector3(hit_x, 1, hit_y);
        }
    }

    private void HoverUI()
    {
        foreach (Cell cell in selectedCells)
        {
            if(cell != null)
            {
                Image img = _cellImages[cell.cell_x, cell.cell_y];

                img.gameObject.SetActive(true);
                img.GetComponent<RectTransform>().anchoredPosition = new Vector2(cell.cell_x, cell.cell_y);

                img.GetComponent<Image>().color = _player.GetSelectionSate() == Player.SelectionState.VALID ? new Color(0, 255, 0, 0.75f) : new Color(255, 0, 0, 0.75f);
            }
        }
    }

    private void CreateGrid()
    {
        for(int x = 0; x < _gridW; x++)
        {
            for(int y = 0; y < _gridL; y++)
            {
                _cells[x, y] = new Cell(x, y);
            }
        }

        CreateSectionsByCount(3, 3);
        LockSections();
        CreateImageGrid();
    }

    /// <summary>
    /// Splits the grid up into sections and populates the _sections list
    /// Sets first section to be unlocked by default and populates
    /// _lockedSectionIndicies list
    /// </summary>
    /// <param name="w">Number of sections horizontally</param>
    /// <param name="h">Number of sections vertically</param>
    private void CreateSectionsByCount(int w, int h)
    {
        int sectionW = _gridW / w; 
        int sectionH = _gridL / h;
        int sectionCounter = 0;

        for(int y = 0; y <= _gridL - sectionH; y += sectionH)
        {
            for (int x = 0; x <= _gridW - sectionW; x += sectionW)
            {
                _sections.Add(new Section()
                {
                    bottom_left = new Vector2(x, y),
                    top_right = new Vector2(x + sectionW, y + sectionH),
                    locked = true
                });

                for(int j = y; j < y + sectionH; j++)
                {
                    for (int i = x; i < x + sectionW; i++)
                    {
                        _cells[i, j].SetSection(sectionCounter);
                    }
                }
                sectionCounter++;
            }
        }

        if(_lockedSectionIndicies.Count == 0)
        {
            for (int i = 0; i < sectionCounter; i++)
            {
                _lockedSectionIndicies.Add(i);
            }

            UpdateSectionLockState(0, false);
        }
    }

    //private void CreateSectionsByDimensions()

    private void UpdateSectionLockState(int sectionIndex, bool locked)
    {
        Section section = _sections[sectionIndex];
        section.locked = locked;

        _sections[sectionIndex] = section;

        if (!locked)
        {
            print($"Section {sectionIndex} unlocked.");
            _lockedSectionIndicies.Remove(sectionIndex);
            UnlockSection(sectionIndex);
        }
        if (locked)
        {
            print($"Section {sectionIndex} locked.");
            _lockedSectionIndicies.Add(sectionIndex);
        }
    }

    public void UnlockSection(int index)
    {
        UpdateSectionLockState(index, false);
        for (int y = 0; y < _gridL; y++)
        {
            for (int x = 0; x < _gridW; x++)
            {
                if (_cells[x,y].GetSection() == index)
                {
                    _cells[x, y].SetState(Cell.CellState.VALID);
                }
            }
        }
        ResetImageGrid();
    }

    private void LockSections()
    {
        for (int y = 0; y < _gridL; y++)
        {
            for (int x = 0; x < _gridW; x++)
            {
                if (_lockedSectionIndicies.Contains(_cells[x, y].GetSection()))
                {
                    _cells[x, y].SetState(Cell.CellState.LOCKED);
                }
            }
        }
    }

    private void CreateImageGrid()
    {
        for(int x = 0; x < _gridW; x++)
        {
            for(int y = 0; y < _gridL; y++)
            {
                GameObject obj = Instantiate(cellImage.gameObject, gridCanvas);
                _cellImages[x, y] = obj.GetComponent<Image>();

                if (_cells[x, y].GetState() != Cell.CellState.LOCKED)
                {
                    obj.SetActive(false);
                }
                else
                {
                    _cellImages[x, y].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                    _cellImages[x, y].color = Color.gray;
                    _cellImages[x, y].gameObject.SetActive(true);
                }
            }
        }
    }

    public void AddBaseStructure(BaseStructure structure)
    {
        _placedStructures.Add(structure);
    }

    public void RemoveBaseStructure(BaseStructure structure)
    {
        _placedStructures.Remove(structure);
    }

    public void RemoveBaseStructure(int index)
    {
        _placedStructures.RemoveAt(index);
    }

    public void SaveZooData()
    {
        zooData = new ZooData();

        zooData.width = (int)world.transform.localScale.x;
        zooData.height = (int)world.transform.localScale.z;

        foreach(BaseStructure baseStructure in _placedStructures)
        {
            zooData.structures.Add(new ZooData.StructureData()
            {
                id = baseStructure.structureID,
                pos_x = (int)baseStructure.transform.position.x,
                pos_z = (int)baseStructure.transform.position.z,
                current_income = Mathf.Round(baseStructure.GetComponent<IncomeStructure>().GetCurrentIncome())
            });
        }

        foreach(Cell cell in _cells)
        {
            zooData.cells.Add(new ZooData.CellData()
            {
                pos_x = cell.cell_x,
                pos_y = cell.cell_y,
                section = cell.GetSection(),
                state = cell.GetState()
            });
        }

        zooData.lockedSections = _lockedSectionIndicies;
    }

    public ZooData GetZoo()
    {
        return zooData;
    }

    public Cell[,] GetCellNeighbors(int cell_x, int cell_y, int xDist, int yDist)
    {
        Cell[,] returnCells = new Cell[xDist, yDist];

        int cur_x = cell_x;
        int cur_y = cell_y;
        int max_width = cell_x + xDist;
        int max_height = cell_y + yDist;

        // Horizontally 
        for (int i = cur_x, x = 0; i < max_width; i++, x++)
        {
            // Vertically
            for (int j = cur_y, y = 0; j < max_height; j++, y++)
            {
                if (i < _gridW && j < _gridL)
                {
                    returnCells[x, y] = _cells[i, j];
                }
            }
        }
        return returnCells;
    }

    public int CountNonNullElements(Array array)
    {
        if (array.Rank == 1)
        {
            // Base case: array is one-dimensional
            int count = 0;
            foreach (object element in array)
            {
                if (element != null)
                {
                    count++;
                }
            }
            return count;
        }
        else
        {
            // Recursive case: array has more than one dimension
            int count = 0;
            foreach (object element in array)
            {
                if (element != null)
                {
                    if (element is Array)
                    {
                        // Element is an array, so call the function recursively
                        count += CountNonNullElements((Array)element);
                    }
                    else
                    {
                        // Element is not an array, so it counts as a non-null element
                        count++;
                    }
                }
            }
            return count;
        }
    }
    
    public void ResetImageGrid()
    {
        for(int y = 0; y < zooData.height; y++)
        {
            for(int x = 0; x < zooData.width; x++)
            {
                if (_cells[x, y].GetState() != Cell.CellState.LOCKED)
                {
                    _cellImages[x, y].gameObject.SetActive(false);
                } else
                {
                    _cellImages[x, y].color = Color.gray;
                }
            }
        }
    }

    public Cell[,] GetAllCells()
    {
        return _cells;
    }
    public Cell GetCell(int x, int y)
    {
        return _cells[x, y];
    }
}
