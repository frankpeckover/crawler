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
                if (dungeon.grid[x, y].type == CELLTYPE.WALL) {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                    tile.sprite = sprite;
                    tileMap.SetTile(position, tile);
                }
            }
        }
        this.tileMap.gameObject.GetComponent<TilemapCollider2D>().enabled = false;
        this.tileMap.gameObject.GetComponent<TilemapCollider2D>().enabled = true;
    }

    public Dungeon generateDungeon(int rows, int cols, int percentageFill, int activeRequirement, int iterations, int minRegionSize) 
    {
        return new Dungeon(rows, cols, percentageFill, activeRequirement, iterations, minRegionSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {         
            this.dungeon = generateDungeon(rows, cols, percentageFill, activeRequirement, iterations, minRegionSize);
            renderDungeon(dungeon);
        }

        if (Input.GetKeyDown("p")) 
        {
            Debug.Log("pressed p");
        }
    }
}
