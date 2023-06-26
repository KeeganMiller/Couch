using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("In-Game Players")] 
    [SerializeField] private List<PlayerInput> _Players = new List<PlayerInput>();              // Reference to the inputs of each player
    private const int MAX_PLAYERS = 4;                  // Define how many players can join a match
    private int _CurrentPlayerCount = 0;                // Reference to how many players are currently in the match

    [Header("Spawn Points")] 
    [SerializeField] private List<Transform> _SpawnPoints = new List<Transform>();              // Reference to the spawn points
    [SerializeField] private List<LayerMask> _PlayerLayers = new List<LayerMask>();

    [Header("Inputs")] 
    [SerializeField] private PlayerInputManager _InputManager;
    
    private void Awake()
    {
        if (!_InputManager)
            _InputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        
    }

    public void AddPlayer(PlayerInput player)
    {
        
    }
}
