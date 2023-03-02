using System.Collections.Generic;
using System.Collections;
using System;

public static class CellularAutomata
{
    public static Grid ApplyRule(Grid grid, Func<Grid, int, int, CELLTYPE> function, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            CELLTYPE[,] newTypes = new CELLTYPE[grid.cells.GetLength(0), grid.cells.GetLength(1)];
            for (int col = 1; col < grid.cells.GetLength(0) - 1; col++) 
            {
                for (int row = 1; row < grid.cells.GetLength(1) - 1; row++) 
                {
                    newTypes[col, row] = function(grid, col, row);
                }
            }

            for (int col = 1; col < grid.cells.GetLength(0) - 1; col++) 
            {
                for (int row = 1; row < grid.cells.GetLength(1) - 1; row++) 
                {
                    grid.cells[col, row].type = newTypes[col, row];
                }
            }
            
        }
        return grid;
    }

    public static CELLTYPE BlockIfSurrounded(Grid grid, int col, int row)
    {  
        int numWalls = grid.cells[col, row].CountNeighboursOfType(CELLTYPE.WALL, DIRECTION.BOTH);

        if (numWalls >= 4 && grid.cells[col, row].type == CELLTYPE.WALL) 
        {
            return CELLTYPE.WALL;
        } 
        return CELLTYPE.EMPTY; 
    }

    public static CELLTYPE FillGap(Grid grid, int col, int row)
    {
        if (grid.cells[col, row].type == CELLTYPE.EMPTY)
        {
            if ((grid.cells[col - 1, row].type == CELLTYPE.WALL && grid.cells[col + 1, row].type == CELLTYPE.WALL) || 
                (grid.cells[col, row - 1].type == CELLTYPE.WALL && grid.cells[col, row + 1].type == CELLTYPE.WALL))
            {
                return CELLTYPE.WALL;
            }
            else 
            { 
                return CELLTYPE.EMPTY; 
            }
        } 
        else
        {
            return CELLTYPE.WALL;
        }
    }
}