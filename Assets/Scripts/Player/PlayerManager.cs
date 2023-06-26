using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;                       // Instance reference
    [Header("In-Game Players")] 
    [SerializeField] private List<GameObject> _Players = new List<GameObject>();              // Reference to the inputs of each player
    private const int MAX_PLAYERS = 4;                  // Define how many players can join a match
    private int _CurrentPlayerCount = 0;                // Reference to how many players are currently in the match

    [Header("Spawn Points")] 
    [SerializeField] private List<Transform> _SpawnPoints = new List<Transform>();              // Reference to the spawn points

    [Header("Prefabs")] 
    [SerializeField] private GameObject _PlayerObject;                  // Reference to the player prefab

    [Header("Debug Spawning")] 
    [SerializeField, Range(1, 4)] private int _SpawnPlayers;
    
    private void Awake()
    {
        // Create the instance
        if (Instance == null)
            Instance = this;
        else 
            Destroy(this.gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < _SpawnPlayers; ++i)
        {
            AddPlayer();
        }
    }

    public void AddPlayer()
    {
        // Check that there is room for the player to join
        if (_CurrentPlayerCount >= MAX_PLAYERS)
        {
            Debug.Log("#AddPlayer::PlayerManager - Lobby is full");
            return;
        }

        _CurrentPlayerCount += 1;                   // Increment the player count

        // Spawn the player in the world
        GameObject spawnedPlayer = GameObject.Instantiate(_PlayerObject);
        // If player has spawned then set the position
        if (spawnedPlayer)
        {
            spawnedPlayer.transform.position = _SpawnPoints[_CurrentPlayerCount - 1].position;
        }
    }
}
