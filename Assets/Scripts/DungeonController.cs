using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonController : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Sprite sprite;

    [SerializeField] private int rows = 100, cols = 100;
    [SerializeField] private int percentageFill=87;
    [SerializeField] private int activeRequirement=5;
    [SerializeField] private int iterations=10;
    [SerializeField] private int minRegionSize=10;


    private Dungeon dungeon;

    void Start()
    {
        this.dungeon = generateDungeon(rows, cols, percentageFill, activeRequirement, iterations, minRegionSize);
        renderDungeon(dungeon);
    }

    public void renderDungeon(Dungeon dungeon) {
        this.tileMap.ClearAllTiles();
        for (int x = 0; x < dungeon.getSize()[0]; x++)
        {
            for (int y = 0; y < dungeon.getSize()[1]; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);             
                Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                tileMap.SetTileFlags(position, TileFlags.None);
                tile.sprite = sprite;        
                if (dungeon.grid[x, y].type == CELLTYPE.WALL) {
                    tile.color = Color.black;
                } else {
                    tile.color = Color.white;
                }             
                tileMap.SetTile(position, tile);
                
            }
        }
    }

    public void paintEdges(Dungeon dungeon) {
        foreach (Region region in dungeon.emptyRegions)
        {
            Color color = Random.ColorHSV();
            foreach (Cell edge in region.edges)
            {
                Vector3Int position = new Vector3Int(edge.col, edge.row, 0);
                Tile tile = tileMap.GetTile<Tile>(position);
                tile.color = color;
                tileMap.RefreshTile(position);
            }
        }
    }

    public void paintRegions(Dungeon dungeon) {
        foreach (Region region in dungeon.emptyRegions)
        {
            Color color = Random.ColorHSV();
            foreach (Cell cell in region.cells)
            {
                Vector3Int position = new Vector3Int(cell.col, cell.row, 0);
                Tile tile = tileMap.GetTile<Tile>(position);
                tile.color = color;
                tileMap.RefreshTile(position);
            }
        }
    }

    public void showClosestRegion(Dungeon dungeon)
    {
        foreach (Region region in dungeon.emptyRegions)
        {
            Vector3 pointOne = new Vector3(region.connection.edge.col, region.connection.edge.row, 0);
            Vector3 pointTwo = new Vector3(region.connection.otherEdge.col, region.connection.otherEdge.row, 0);
            Debug.DrawLine(pointOne, pointTwo, Color.white, 9999f);
        }
    }

    public Dungeon generateDungeon(int rows, int cols, int percentageFill, int activeRequirement, int iterations, int minRegionSize) 
    {
        return new Dungeon(rows, cols, percentageFill, activeRequirement, iterations, minRegionSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("/"))
        {         
            this.dungeon = generateDungeon(rows, cols, percentageFill, activeRequirement, iterations, minRegionSize);
            renderDungeon(dungeon);
        }

        if (Input.GetKeyDown("r"))
        {         
            this.paintRegions(dungeon);
        }

        if (Input.GetKeyDown("e"))
        {         
            this.paintEdges(dungeon);
        }

        if (Input.GetKeyDown("v"))
        {         
            this.dungeon.connectRegions();
            this.renderDungeon(this.dungeon);
        }
    }
}
