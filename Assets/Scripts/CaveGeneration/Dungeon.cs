using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class Dungeon
{
    public Grid grid;
    private List<Region> wallRegions;
    public List<Region> emptyRegions;

    /*
    private List<Cell> floorCells;
    public Cell exitCell { get; private set; }
    public Cell spawnCell { get; private set; }
    */
    
    public Dungeon(int cols, int rows, int percentageFill, int iterations, int minRegionSize) 
    {
        this.grid = new Grid(cols, rows);
        this.grid.Randomise(percentageFill, 1, 1, grid.cells.GetLength(0) - 1, grid.cells.GetLength(1) - 1);
        CellularAutomata.ApplyRule(this.grid, CellularAutomata.BlockIfSurrounded, iterations);
        CellularAutomata.ApplyRule(this.grid, CellularAutomata.FillGap, 1);
        
        this.wallRegions = GetRegionsOfType(CELLTYPE.WALL);
        this.emptyRegions = GetRegionsOfType(CELLTYPE.EMPTY);

        this.wallRegions = FillRegionsBySize(this.wallRegions, CELLTYPE.EMPTY, minRegionSize);
        this.emptyRegions = FillRegionsBySize(this.emptyRegions, CELLTYPE.WALL, minRegionSize);

        while (this.emptyRegions.Count > 1)
        {
            (Region region, Cell cell, Region otherRegion, Cell otherCell, float distance) closestRegion = this.GetClosestRegions(this.emptyRegions, this.emptyRegions);
            this.ConnectCells(closestRegion.cell, closestRegion.otherCell);
            this.CombineRegions(closestRegion.region, closestRegion.otherRegion);
        }        
    }

    private Region CombineRegions(Region region, Region otherRegion)
    {
        foreach (Cell cell in otherRegion.cells)
        {
            region.cells.Add(cell);
        }

        foreach (Cell edge in otherRegion.edges)
        {
            region.edges.Add(edge);
        }
        this.emptyRegions.Remove(otherRegion);
        return region;
    }

    private List<Region> GetRegionsOfType(CELLTYPE cellType) 
    {
        List<Region> regions = new List<Region>();
        List<Cell> visited = new List<Cell>();

        foreach (Cell cell in this.grid.cells)
        {
            if (visited.Contains(cell) == false && cell.type == cellType)
            {
                Region region = new Region(FloodSearch(cell, cellType));
                regions.Add(region);

                foreach (Cell regionCell in region.cells)
                {
                    visited.Add(regionCell);
                }
            }
        }
        return regions;
    }

    private List<Region> FillRegionsBySize(List<Region> regions, CELLTYPE type, int minRegionSize)
    {
        for (int i = 0; i < regions.Count; i++)
        {
            if (regions[i].cells.Count < minRegionSize)
            {
                foreach (Cell cell in regions[i].cells)
                {
                    this.grid.ChangeCellType(cell, type);
                }
                regions.Remove(regions[i]);
                i--;
            }
        }
        return regions;
    }

    private List<Cell> FloodSearch(Cell startCell, CELLTYPE cellType)
    {
        List<Cell> cells = new List<Cell>();
        List<Cell> visited = new List<Cell>();
        Queue<Cell> queue = new Queue<Cell>();
        queue.Enqueue(startCell);

        while (queue.Count > 0)
        {
            Cell cell = queue.Dequeue();
            cells.Add(cell);

            foreach (Cell neighbour in cell.orthoNeighbours)
            {
                if (neighbour.type == cellType && visited.Contains(neighbour) == false) 
                {
                    queue.Enqueue(neighbour);
                    visited.Add(neighbour);
                }
            }
        }
        return cells;
    } 

    public void ConnectCells(Cell cell, Cell otherCell)
    {
        int xDist = (otherCell.col - cell.col);
        int yDist = (otherCell.row - cell.row);

        float magnitude = (float) Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2));

        float xUnit = (float) xDist / magnitude;
        float yUnit = (float) yDist / magnitude;

        for (int step = 0; step <  Math.Ceiling(magnitude); step++)
        {
            int xStep = cell.col + (int) Math.Floor((step * xUnit));
            int yStep = cell.row + (int) Math.Floor((step * yUnit));

            foreach (Cell orthoNeighbour in this.grid.cells[xStep, yStep].orthoNeighbours)
            {
                this.grid.ChangeCellType(orthoNeighbour, CELLTYPE.EMPTY);
            }

            foreach (Cell diagonalNeighbour in this.grid.cells[xStep, yStep].diagonalNeighbours)
            {
                this.grid.ChangeCellType(diagonalNeighbour, CELLTYPE.EMPTY);
            }
            this.grid.ChangeCellType(this.grid.cells[xStep, yStep], CELLTYPE.EMPTY);
        }
    }

    private (Region, Cell, Region, Cell, float) GetClosestRegions(List<Region> regions, List<Region> otherRegions)
    {        
        (Region region, Cell cell, Region otherRegion, Cell otherCell, float distance) bestPair = (regions[0], regions[0].edges[0], otherRegions[0], otherRegions[0].edges[0], float.MaxValue);
        foreach (Region region in regions)
        {
            foreach (Region otherRegion in regions)
            {
                if (region.cells.Count != otherRegion.cells.Count) 
                {
                    (Cell cell, Cell otherCell, float distance) currentPair = GetClosestCells(region.edges, otherRegion.edges);
                    if (currentPair.distance < bestPair.distance)
                    {
                        bestPair.region = region;
                        bestPair.cell = currentPair.cell;
                        bestPair.otherRegion = otherRegion;
                        bestPair.otherCell = currentPair.otherCell;
                        bestPair.distance = currentPair.distance;
                    }
                } 
            }
        }
        return bestPair;
    }

    private (Cell, Cell, float) GetClosestCells(List<Cell> cells, List<Cell> otherCells)
    {
        (Cell cell, Cell otherCell, float distance) bestPair = (cells[0], otherCells[0], float.MaxValue);
        float distance = float.MaxValue;

        foreach (Cell cell in cells)
        {
            foreach (Cell otherCell in otherCells)
            {
                if (cell != otherCell)
                {
                    distance = this.grid.GetDistanceBetweenCells(cell, otherCell);
                    if (distance < bestPair.distance)
                    {
                        bestPair.distance = distance;
                        bestPair.cell = cell;
                        bestPair.otherCell = otherCell;
                    }
                }
            }
        }
        return bestPair;
    }


    /*
    private List<Cell> getFloorTiles(int minHeight)
    {
        List<Cell> eligibleCells = new List<Cell>();
        foreach (Region region in this.emptyRegions)
        {
            foreach (Cell cell in region.edges)
            {
                for (int space = 0; space <= minHeight; space++) 
                {
                    if (this.grid.cells[cell.col, cell.row + space].type == CELLTYPE.WALL)
                    {
                        break;
                    } 
                    else if (space == minHeight) 
                    {
                        eligibleCells.Add(cell);
                    }
                }
            }
        }
        return eligibleCells;
    }

    /*
    private void addSpecials()
    {
        System.Random random = new System.Random();        
        this.exitCell = floorCells[random.Next(0, floorCells.Count)];
        this.spawnCell = floorCells[random.Next(0, floorCells.Count)];
        this.exitCell.type = CELLTYPE.SPECIAL;
        this.spawnCell.type = CELLTYPE.SPECIAL;
    }
    */
}