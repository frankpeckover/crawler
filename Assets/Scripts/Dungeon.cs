using System.Collections.Generic;
using System.Collections;
using System;

public class Region {
    public int id;
    public Cell[] cells;
    public List<Cell> edges;
    public (Cell edge, Cell otherEdge) connection;

    public Region(int id) 
    {
        this.id = id;
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

    public Dungeon(int cols, int rows, int percentageFill, int activeRequirement, int iterations, int minRegionSize) 
    {
        this.rows = rows;
        this.cols = cols;
        this.percentageFill = percentageFill;
        this.activeRequirement = activeRequirement;
        this.iterations = iterations;
        this.minRegionSize = minRegionSize;

        this.grid = new Cell[cols, rows];
        this.initialiseGrid();
        
        for (int i = 0; i < this.iterations; i++)
        {
            this.grid = this.smoothGrid();
        }

        this.wallRegions = getRegions(CELLTYPE.WALL);
        this.emptyRegions = getRegions(CELLTYPE.EMPTY);

        this.wallRegions = processRegions(this.wallRegions, CELLTYPE.EMPTY);
        this.emptyRegions = processRegions(this.emptyRegions, CELLTYPE.WALL);
        this.getRegionEdges();
        this.getClosestRegionEdges();

        while (this.emptyRegions.Count > 1)
        {
            this.emptyRegions = getRegions(CELLTYPE.EMPTY);
            this.getRegionEdges();
            this.getClosestRegionEdges();
            this.connectRegions();
        }
    }

    public int[] getSize()
    {
        int[] size = {this.cols, this.rows};
        return size;
    }

    private void initialiseGrid() 
    {
        System.Random random = new System.Random();
        for (int col = 0; col < cols; col++) 
        {
            for (int row = 0; row < rows; row++) 
            {
                if (row == 0 || col == 0 || row == rows - 1 || col == cols - 1) 
                {
                    this.grid[col, row] = new Cell(col, row, CELLTYPE.WALL);
                }
                else 
                {
                    if (random.Next(0, 101) < this.percentageFill) 
                    {
                        this.grid[col, row] = new Cell(col, row, CELLTYPE.WALL);
                    } 
                    else 
                    {
                        this.grid[col, row] = new Cell(col, row, CELLTYPE.EMPTY);
                    }
                }
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

    private List<Region> getRegionEdges()
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
        return this.emptyRegions;
    }

    public List<Region> connectRegions()
    {
        System.Random random = new System.Random();
        foreach (Region region in emptyRegions)
        {
            int xDist = region.connection.otherEdge.col - region.connection.edge.col + 4;
            int yDist = region.connection.otherEdge.row - region.connection.edge.row + 4;

            this.convertNeighboursToType(region.connection.otherEdge, CELLTYPE.EMPTY);

            for (int total = System.Math.Abs(xDist) + System.Math.Abs(yDist); total >= 0; total--)
            {
                if (System.Math.Abs(xDist) > 0 && System.Math.Abs(yDist) > 0)
                {
                    if (random.Next(0, 101) < 50) 
                    {
                        xDist = xDist > 0 ? --xDist : ++xDist;
                    } else 
                    {
                        yDist = yDist > 0 ? --yDist : ++yDist;
                    }
                }
                else if (System.Math.Abs(xDist) > 0)
                {
                    xDist = xDist > 0 ? --xDist : ++xDist;
                }
                else 
                {
                    yDist = yDist > 0 ? --yDist : ++yDist;
                }
                Cell cellToChange = this.grid[region.connection.edge.col + xDist, region.connection.edge.row + yDist];
                this.convertNeighboursToType(cellToChange, CELLTYPE.EMPTY);

            }
        }
        return this.emptyRegions;
    }

    private void convertNeighboursToType(Cell cell, CELLTYPE type)
    {
        cell.type = type;
        Cell[] neighbours = this.getNeighbours(cell, 1, true);
        foreach (Cell neighbour in neighbours)
        {
            neighbour.type = type;
        }
    }

    private List<Region> getClosestRegionEdges()
    {        
        foreach (Region region in this.emptyRegions)
        {
            float closest = 999f;
            Cell closestEdge = region.cells[0];
            Cell closestOtherEdge = region.cells[0];
            foreach (Region otherRegion in this.emptyRegions)
            {
                if (region.id == otherRegion.id) 
                {
                    break;
                } 
                else 
                {
                    foreach (Cell edge in region.edges)
                    {
                        if (edge.col % 4 == 0) { continue; }
                        foreach (Cell otherEdge in otherRegion.edges)
                        {
                            if (otherEdge.row % 4 == 0) { continue; }
                            int xDist = otherEdge.col - edge.col;
                            int yDist = otherEdge.row - edge.row;
                            float distance = (float)(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));

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
        return this.emptyRegions;
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
                        if (x == 0 && y == 0) { continue; } 
                        if (x == 0 || y == 0) 
                        { 
                            neighbours.Add(this.grid[cell.col + x, cell.row + y]); 
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
        queue.Enqueue(startCell);

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
                Region region = new Region(cell.col * cell.row);
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



