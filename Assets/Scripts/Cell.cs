using System;

[Serializable]
public class Cell
{
    public enum CellState { VALID, INVALID, LOCKED };

    private CellState _cellState;

    private int _section;

    public int cell_x, cell_y;

    public Cell(int x, int y)
    {
        cell_x = x;
        cell_y= y;
    }

    public CellState GetState()
    {
        return _cellState;
    }
    public void SetState(CellState state)
    {
        _cellState = state;
    }
    
    public int GetSection() { return _section; }
    public void SetSection(int section) { _section = section; }

}
