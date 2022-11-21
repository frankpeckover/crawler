using System.Collections.Generic;
using System.Collections;
using System;

public class Region {
    public Cell[] cells;
    public List<Cell> edges;
    public (Cell edge, Cell otherEdge) connection;

    public Region() 
    {
        this.edges = new List<Cell>();
    }

    public void createConnection(Cell edge, Cell otherEdge)
    {
        connection = (edge, otherEdge);
    }
}

public class Dungeon
{

    public Cell[,] grid;
    private List<Region> wallRegions;
    public List<Region> emptyRegions;
    private int rows, cols;
    private int percentageFill;
    private int activeRequirement;
    private int iterations;
    private int minRegionSize;

    public Dungeon(int rows, int cols, int percentageFill, int activeRequirement, int iterations, int minRegionSize) 
    {
        this.rows = rows;
        this.cols = cols;
        this.percentageFill = percentageFill;
        this.activeRequirement = activeRequirement;
        this.iterations = iterations;
        this.minRegionSize = minRegionSize;

        this.grid = new Cell[cols, rows];
        this.initialiseGrid();
        this.randomiseGrid();
        for (int i = 0; i < this.iterations; i++)
        {
            this.grid = this.smoothGrid();
        }

        this.wallRegions = getRegions(CELLTYPE.WALL);
        this.emptyRegions = getRegions(CELLTYPE.EMPTY);

        this.wallRegions = processRegions(this.wallRegions, CELLTYPE.EMPTY);
        this.emptyRegions = processRegions(this.emptyRegions, CELLTYPE.WALL);

        this.getEdgeCells();
        
    }

    public int[] getSize()
    {
        int[] size = {this.rows, this.cols};
        return size;
    }

    private List<Region> processRegions(List<Region> regions, CELLTYPE changeTo)
    {
        List<Region> toRemove = new List<Region>();
        foreach (Region region in regions)
        {
            if (region.cells.Length < this.minRegionSize) 
            {
                foreach (Cell cell in region.cells)
                {
                    cell.type = changeTo;
                }
                toRemove.Add(region);
            }
        }
        foreach (Region region in toRemove)
        {
            regions.Remove(region);
        }
        return regions;
    }

    public void getEdgeCells()
    {
        foreach (Region region in this.emptyRegions)
        {
            foreach (Cell cell in region.cells)
            {
                Cell[] neighbours = getNeighbours(cell, 1, false);
                foreach (Cell neighbour in neighbours)
                {
                    if (neighbour.type == CELLTYPE.WALL)
                    {
                        region.edges.Add(neighbour);
                    }
                }
            }
        }
    }

    private void connectRegions()
    {
        foreach (Region region in emptyRegions)
        {
            int x = region.connection.otherEdge.col - region.connection.edge.col;
            float yDist = region.connection.otherEdge.row - region.connection.edge.row;
            float distance = MathF.Pow(x, 2) + MathF.Pow(yDist, 2);
        }
    }

    private void findClosestEdges()
    {
        float closest = 999f;
        
        foreach (Region region in this.emptyRegions)
        {
            Cell closestOtherEdge = region.cells[0];
            Cell closestEdge = region.cells[0];
            foreach (Region otherRegion in this.emptyRegions)
            {
                if (region.cells != otherRegion.cells) {
                    foreach (Cell edge in region.edges)
                    {
                        foreach (Cell otherEdge in otherRegion.edges)
                        {
                            float xDist = otherEdge.col - edge.col;
                            float yDist = otherEdge.row - edge.row;
                            float distance = MathF.Pow(xDist, 2) + MathF.Pow(yDist, 2);

                            if (distance < closest)
                            {
                                closest = distance;
                                closestEdge = edge;
                                closestOtherEdge = otherEdge;
                            }
                        }
                    }
                }
            }
            region.createConnection(closestEdge, closestOtherEdge);
        }
    }

    private void initialiseGrid() 
    {
        for (int col = 0; col < cols; col++) 
        {
            for (int row = 0; row < cols; row++) 
            {
                if (row == 0 || col == 0 || row == cols - 1 || col == rows - 1) 
                {
                    this.grid[col, row] = new Cell(col, row, CELLTYPE.WALL);
                }
                else 
                {
                    this.grid[col, row] = new Cell(row, col, CELLTYPE.EMPTY);
                }
            }
        }
    }

    private void randomiseGrid() 
    {
        System.Random random = new System.Random();
        foreach (Cell cell in this.grid) 
        {
            if (random.Next(0, 101) < this.percentageFill) 
            {
                cell.type = CELLTYPE.WALL;
            }
        }
    }

    private Cell[,] smoothGrid() 
    {
        Cell[,] newGrid = this.grid;
        foreach (Cell cell in newGrid)
        {
            cell.type = computeNewCellType(cell);
        }
        return newGrid;
    }

    private CELLTYPE computeNewCellType(Cell cell) 
    {  
        Cell[] neighbours = this.getNeighbours(cell, 1, true);
        if (this.countNeighboursOfType(neighbours, CELLTYPE.WALL) >= this.activeRequirement && this.grid[cell.col, cell.row].type == CELLTYPE.WALL) 
        {
            return CELLTYPE.WALL;
        } 
        return CELLTYPE.EMPTY;   
    }

    private int countNeighboursOfType(Cell[] neighbours, CELLTYPE type) 
    {
        int activeNeighbours = 0;
        foreach (Cell cell in neighbours)
        {
            if (cell.type == type) 
            {
                activeNeighbours++;
            }
        }
        return activeNeighbours + (8 - neighbours.Length);
    }

    private Cell[] getNeighbours(Cell cell, int scope, bool addDiagonals) 
    {
        List<Cell> neighbours = new List<Cell>();
        for (int x = -scope; x <= scope; x++) 
        {
            for (int y = -scope; y <= scope; y++) 
            {
                if (cell.col + x >= 0 && cell.col + x < this.cols) 
                {
                    if (cell.row + y >= 0 && cell.row + y < this.rows)
                    {
                        if (x == 0 || y == 0)
                        {
                            if (x == 0 && y == 0)
                            {
                            }
                            else
                            {
                                neighbours.Add(this.grid[cell.col + x, cell.row + y]);
                            }
                        } 
                        else if (addDiagonals)
                        {
                            neighbours.Add(this.grid[cell.col + x, cell.row + y]);
                        }
                    }
                }
            }
        }
        return neighbours.ToArray();
    }

    private Cell[] getRegionCells(Cell startCell, CELLTYPE cellType)
    {
        List<Cell> regionCells = new List<Cell>();
        List<Cell> visited = new List<Cell>();
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(this.grid[startCell.col, startCell.row]);

        while (queue.Count > 0)
        {
            Cell cell = queue.Dequeue();
            regionCells.Add(cell);

            Cell[] neighbours = getNeighbours(cell, 1, false);
            foreach (Cell neighbour in neighbours)
            {
                if (neighbour.type == cellType && visited.Contains(neighbour) == false) 
                {
                    queue.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }
        }
        return regionCells.ToArray();
    } 

    private List<Region> getRegions(CELLTYPE cellType) 
    {
        List<Region> regions = new List<Region>();
        List<Cell> visited = new List<Cell>();


        foreach (Cell cell in this.grid)
        {
            if (visited.Contains(cell) == false && cell.type == cellType)
            {
                Region region = new Region();
                region.cells = getRegionCells(cell, cellType);
                regions.Add(region);

                foreach (Cell regionCell in region.cells)
                {
                    visited.Add(regionCell);
                }
            }
        }
        return regions;
    }
}



