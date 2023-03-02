using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Cell 
{
    public int col { get; private set;}
    public int row { get; private set;}
    public CELLTYPE type;
    public List<Cell> orthoNeighbours;
    public List<Cell> diagonalNeighbours;

    public Cell(int col, int row)
    {
        this.col = col;
        this.row = row;
        this.type = CELLTYPE.WALL;
        
        this.orthoNeighbours = new List<Cell>();
        this.diagonalNeighbours = new List<Cell>();
    }

    public int CountNeighboursOfType(CELLTYPE type, DIRECTION direction)
    {
        IEnumerable<Cell> neighbours = direction switch
        {
            DIRECTION.ORTHO => this.orthoNeighbours,
            DIRECTION.DIAGONAL => this.diagonalNeighbours,
            DIRECTION.BOTH => this.orthoNeighbours.Concat(this.diagonalNeighbours),
            _ => Enumerable.Empty<Cell>()
        };
        return neighbours.Count(cell => cell.type == type);
    }
}

