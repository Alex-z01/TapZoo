using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    private List<Cell> _cells = new List<Cell>();

    public List<Cell> selectedCells = new List<Cell>();

    public int cellWidth, cellHeight; // Dimensions of cells
    public int gridWidth, gridHeight; // Measured in number of cells
    public float realGridWidth, realGridHeight; // Dimensions of grid

    public GameObject world;
    public RectTransform gridCanvas;
    
    public Image image;
    [HideInInspector]
    public List<GameObject> cell_images = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        world.transform.position = new Vector3(world.transform.localScale.x / 2f, -world.transform.localScale.y / 2f, world.transform.localScale.z / 2f);

        realGridWidth = world.transform.localScale.x;
        realGridHeight = world.transform.localScale.z;

        gridWidth = (int)realGridWidth / cellWidth;
        gridHeight = (int)realGridHeight / cellHeight;

        gridCanvas.sizeDelta = new Vector2(realGridWidth, realGridHeight);
        gridCanvas.transform.position = new Vector3(world.transform.position.x, 0.001f, world.transform.position.z);

        CreateGrid();
        CreateCellImages(gridWidth * gridHeight);
    }

    void CreateGrid()
    {
        Debug.Log("Creating grid");
        int count = 0;
        for(int y = 0; y < gridHeight; y+=cellHeight)
        {
            for(int x = 0; x < gridWidth; x+=cellWidth)
            {
                //Cell cell = new Cell(x, y, count);
                count++;
                
                //_cells.Add(cell);
            }
        }
    }

    public void CreateCellImages(int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(image.gameObject, gridCanvas);
            obj.SetActive(false);

            cell_images.Add(obj);
        }
    }

    public void ResetCellImages()
    {
        foreach(GameObject cell in cell_images)
        {
            cell.SetActive(false);
        }
    }

    public Cell GetCell(int index)
    {
        return _cells[index];
    }

    public Cell GetCell(int x, int y)
    {
        int index = x + gridWidth * y;
        return _cells[index];
    }

    public List<Cell> GetAllCells()
    {
        return _cells;
    }

}
