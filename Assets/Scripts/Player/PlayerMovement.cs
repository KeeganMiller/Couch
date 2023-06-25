using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float _MovementSpeed;              // Speed the player object will move at
    [SerializeField] protected float _RotationSpeed;                // Speed at which the character will rotation at

    [Header("Player Inputs")] 
    [SerializeField] protected PlayerInput _Input;


    [Header("Components")] 
    [SerializeField] protected CharacterController _CharController;                    // Reference to the Character Controller
    
    protected virtual void Awake()
    {
        // Get reference to the character controller
        if (!_CharController)
            _CharController = this.GetComponent<CharacterController>();

        _Input = new PlayerInput();
    }

    protected void OnEnable()
    {
        _Input.Enable();
    }

    protected void OnDisable()
    {
        _Input.Disable();
    }

    protected void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Validate the rigidbody before handling movement
        if (!_CharController)
            return;

        var movementInput = _Input.Player.Move.ReadValue<Vector2>();                    // Get the input value as vector 2
        var movementValue = new Vector3(movementInput.x, 0f, movementInput.y);      // Create the movement value

        if (movementValue.normalized.magnitude > 0.1f)
        {
            var finalMovement = movementValue * (_MovementSpeed * Time.deltaTime);          // Calculate final movement direction
            _CharController.Move(finalMovement);                // Apply movement to the character controller
            
            Quaternion toRotation = Quaternion.LookRotation(finalMovement, Vector3.up);             // Get where we want to rotate the character to
            // Apply rotation smoothly
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, (_RotationSpeed * Time.deltaTime));
        }


    }
}
