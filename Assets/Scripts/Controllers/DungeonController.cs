using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DungeonController : MonoBehaviour
{

    #region Singleton
    public static DungeonController Instance { get; private set; }

    private void Awake() 
    {       
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    #endregion

    #region Variables
    private Dungeon dungeon;

    [SerializeField] GameObject exitTriggerPrefab; 

    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Sprite sprite;

    [SerializeField] private int rows, cols;
    [SerializeField] private int percentageFill;
    [SerializeField] private int iterations;
    [SerializeField] private int minRegionSize;

    
    public delegate void DungeonDelegate(Dungeon dungeon);

    public static event DungeonDelegate dungeonReady;
    public static event DungeonDelegate dungeonCleared;
    #endregion

    void Start()
    {
        generateDungeon(this.rows, this.cols, this.percentageFill, this.iterations, this.minRegionSize);

        ExitTrigger.exitTriggerCollided += OnDungeonCleared;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnDungeonCleared();
        }

        if (Input.GetKeyDown("q"))
        {         

        }

        if (Input.GetKeyDown("w"))
        {         
           foreach (Region region in this.dungeon.emptyRegions)
            {
                paintCells(region.cells);
                paintCells(region.edges);
            }
        }

        if (Input.GetKeyDown("e"))
        {         
            
        }

        if (Input.GetKeyDown("r"))
        {         
            
        }
    }

    private void OnDungeonCleared()
    {
        dungeonCleared?.Invoke(this.dungeon);
        generateDungeon(this.rows, this.cols, this.percentageFill, this.iterations, this.minRegionSize);
    }

    private void generateDungeon(int rows, int cols, int percentageFill, int iterations, int minRegionSize)
    {
        this.dungeon = new Dungeon(rows, cols, percentageFill, iterations, minRegionSize);
        //Instantiate(this.exitTriggerPrefab, new Vector3((float)dungeon.exitCell.col + 0.5f, (float)dungeon.exitCell.row + 1.5f, 0f), Quaternion.identity);
        buildTileMap();
        dungeonReady?.Invoke(this.dungeon);
    }

    public void buildTileMap() {
        this.tileMap.ClearAllTiles();
        for (int col = 0; col < this.dungeon.grid.cells.GetLength(0); col++)
        {
            for (int row = 0; row < this.dungeon.grid.cells.GetLength(1); row++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);             
                Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                tileMap.SetTileFlags(position, TileFlags.None);
                tile.sprite = sprite;   
                tile.name = string.Format("Tile ({0}, {1})", col, row);
                if (dungeon.grid.cells[col, row].type != CELLTYPE.EMPTY) {
                    tile.color = Color.black;
                } else {
                    tile.color = Color.white;
                }         
                tileMap.SetTile(position, tile);
            }
        }
    }

    public void paintCells(List<Cell> cells) {
        Color color = UnityEngine.Random.ColorHSV();
        foreach (Cell cell in cells)
        {
            Vector3Int position = new Vector3Int(cell.col, cell.row, 0);
            Tile tile = tileMap.GetTile<Tile>(position);
            tile.color = color;
            tileMap.RefreshTile(position);
        }
    }

    private Vector3Int lastSize; // The size of the Tilemap last frame
    private GUIStyle style; // The GUIStyle to use for the numbers
    [SerializeField] public bool draw = false;

    private void OnDrawGizmos()
    {
        if (!draw)
        {
            return;
        }
        // Create the GUIStyle if it doesn't exist
        if (style == null)
        {
            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 10;
            style.alignment = TextAnchor.MiddleCenter;
        }

        // Only update if the tileMap size has changed
        if (this.tileMap.size != lastSize)
        {
            lastSize = this.tileMap.size;
            SceneView.RepaintAll(); // Force a Scene view repaint
        }

        // Loop through each tile position in the tileMap
        for (int y = this.tileMap.origin.y; y < this.tileMap.origin.y + this.tileMap.size.y; y++)
        {
            for (int x = this.tileMap.origin.x; x < this.tileMap.origin.x + this.tileMap.size.x; x++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);

                // Get the tile's name (assumes it is in the format "Tile (x, y)")
                if (this.tileMap.GetTile(tilePos))
                {
                    string tileName = this.tileMap.GetTile(tilePos).name;
                    string[] nameParts = tileName.Split('(', ')', ',');
                    int tileX = int.Parse(nameParts[1].Trim());
                    int tileY = int.Parse(nameParts[2].Trim());

                    // Display the tile number using Gizmos.Label
                    Vector3 worldPos = this.tileMap.CellToWorld(tilePos) + new Vector3(0.25f, 0.25f, 0);
                    Handles.Label(worldPos, String.Format("{0}, {1}", tileX, tileY), style);
                }

                
            }
        }
    }
}
