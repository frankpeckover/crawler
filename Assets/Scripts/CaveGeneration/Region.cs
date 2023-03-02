using System.Collections;
using System.Collections.Generic;

public class Region 
{
    //public CELLTYPE type;
    public List<Cell> cells;
    public List<Cell> edges;

    public Region(List<Cell> cells) 
    {
        this.cells = cells;
        this.edges = new List<Cell>();
        this.SetEdges();
    }

    private void SetEdges()
    {
        foreach (Cell cell in this.cells)
        {
            if (cell.CountNeighboursOfType(CELLTYPE.EMPTY, DIRECTION.ORTHO) < 4)
            {
                edges.Add(cell);
            }
            
        }
    }

    public (float, float) FindCenter()
    {
        int xSum = 0;
        int ySum = 0;
        foreach (Cell cell in this.cells)
        {
            xSum += cell.col;
            ySum += cell.row;
        }

        float xCenter = xSum / cells.Count;
        float yCenter = ySum / cells.Count;

        return (xCenter, yCenter);
    }
}