using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Singleton
    public static PlayerController Instance { get; private set; }

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

        //DungeonController.dungeonReady += OnDungeonReady;
       // DungeonController.dungeonCleared += OnDungeonCleared;
    }
    #endregion

    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;

    public void OnDungeonReady(Dungeon dungeon) 
    {
        spawnPlayer(dungeon);
    }

    public void OnDungeonCleared(Dungeon dungeon)
    {
        RemovePlayer(dungeon);
    }

    private void spawnPlayer(Dungeon dungeon) 
    { 
        //this.playerInstance = Instantiate(this.playerPrefab, new Vector3((float)dungeon.spawnCell.col + 0.5f, (float)dungeon.spawnCell.row + 1.5f, 0f), Quaternion.identity);
    }   

    private void RemovePlayer(Dungeon dungeon)
    {
        GameObject.Destroy(this.playerInstance);
    }
}
