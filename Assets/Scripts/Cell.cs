public enum CELLTYPE {
        WALL,
        EMPTY
    }

public class Cell 
{
    
    public CELLTYPE type;
    public int row, col;

    public Cell(int row, int col, CELLTYPE type=CELLTYPE.EMPTY) 
    {
        this.row = row;
        this.col = col;
        this.type = type;
    }
}