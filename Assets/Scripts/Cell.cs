public enum CELLTYPE {
        WALL,
        EMPTY
    }

public class Cell 
{
    
    public CELLTYPE type;
    public int row, col;

    public Cell(int col, int row, CELLTYPE type=CELLTYPE.EMPTY) 
    {
        this.row = row;
        this.col = col;
        this.type = type;
    }

    public override string ToString()
    {
        return this.col + " : " + this.row;
    }

}