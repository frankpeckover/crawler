using System.Collections.Generic;
using System.Collections;
using System;


public class Grid
{
    public Cell[,] cells;

    public Grid(int cols, int rows) 
    {
        this.cells = new Cell[cols, rows];

        for (int col = 0; col < cols; col++) 
        {
            for (int row = 0; row < rows; row++) 
            {
                this.cells[col, row] = new Cell(col, row);
            }
        }

        for (int col = 0; col < cols; col++) 
        {
            for (int row = 0; row < rows; row++) 
            {
                this.AssignDiagonalNeighbours(col, row, 1);
                this.AssignOrthoNeighbours(col, row, 1);
            }
        }        
    }

    public float GetDistanceBetweenCells(Cell from, Cell to)
    {
        int xDist = to.col - from.col;
        int yDist = to.row - from.row;
        return (float)(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));
    }

    public void ChangeCellType(Cell cell, CELLTYPE type)
    {
        cells[cell.col, cell.row].type = type;
    }

    public Cell[,] Randomise(int percentageFill, int startX, int startY, int endX, int endY)
    {
        System.Random random = new System.Random();
        for (int col = startX; col < endX; col++) 
        {
            for (int row = startY; row < endY; row++) 
            {

                if (random.Next(0, 101) < percentageFill) 
                {
                    this.cells[col, row].type = CELLTYPE.WALL;
                } 
                else 
                {
                    this.cells[col, row].type = CELLTYPE.EMPTY;
                }
            }
        }
        return cells;
    }

    private void AssignDiagonalNeighbours(int cellX, int cellY, int scope) 
    {
        for (int x = -scope; x <= scope; x++) 
        {
            for (int y = -scope; y <= scope; y++) 
            {
                if (cellX + x >= 0 && cellX + x < cells.GetLength(0)) 
                {
                    if (cellY + y >= 0 && cellY + y < cells.GetLength(1))
                    {
                        if (x == 0 && y == 0) { continue; } 
                        if (x != 0 && y != 0) 
                        { 
                            Cell neighbour = cells[cellX + x, cellY + y];
                            cells[cellX, cellY].diagonalNeighbours.Add(neighbour); 
                        }
                    }
                }
            }
        }
    }

    private void AssignOrthoNeighbours(int cellX, int cellY, int scope)
    {
        for (int x = -scope; x <= scope; x++) 
        {
            for (int y = -scope; y <= scope; y++) 
            {
                if (cellX + x >= 0 && cellX + x < cells.GetLength(0)) 
                {
                    if (cellY + y >= 0 && cellY + y < cells.GetLength(1))
                    {
                        if (x == 0 && y == 0) { continue; } 
                        if (x == 0 || y == 0) 
                        { 
                            Cell neighbour = cells[cellX + x, cellY + y];
                            cells[cellX, cellY].orthoNeighbours.Add(neighbour); 
                        }
                    }
                }
            }
        }
    }
}