using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("In-Game Players")] 
    [SerializeField] private List<GameObject> _Players = new List<GameObject>();              // Reference to the inputs of each player
    private const int MAX_PLAYERS = 4;                  // Define how many players can join a match
    private int _CurrentPlayerCount = 0;                // Reference to how many players are currently in the match

    [Header("Spawn Points")] 
    [SerializeField] private List<Transform> _SpawnPoints = new List<Transform>();              // Reference to the spawn points
    [SerializeField] private List<LayerMask> _PlayerLayers = new List<LayerMask>();

    [Header("Prefabs")] 
    [SerializeField] private GameObject _PlayerObject;                  // Reference to the player prefab
    
    private void Awake()
    {

    }

    public void AddPlayer(GameObject player)
    {
        // Check that there is room for the player to join
        if (_CurrentPlayerCount >= MAX_PLAYERS)
        {
            Debug.Log("#AddPlayer::PlayerManager - Lobby is full");
            return;
        }

        GameObject spawnedPlayer = GameObject.Instantiate(_PlayerObject);
        Transform spawnTransform = _SpawnPoints[_CurrentPlayerCount - 1];
        if (spawnedPlayer && spawnTransform)
        {
            spawnedPlayer.transform.position = spawnTransform.position;
        }

    }
}
