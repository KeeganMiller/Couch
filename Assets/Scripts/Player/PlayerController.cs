using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float _GeneralMovementSpeed;              // Speed the player object will move at
    [SerializeField] protected float _SandMovementSpeed;                 // Speed the player moves at when on sand
    [SerializeField] protected float _RotationSpeed;                // Speed at which the character will rotation at
    private Vector3 _PlayerMovementInput;                       // Reference to the current input

    [FormerlySerializedAs("_OnSurface")] [Header("Surface")] 
    public ESurfaceType OnSurface;         // Reference to the surface we are currently on

    [Header("Player Inputs")] 
    [SerializeField] protected PlayerInput _Input;


    [FormerlySerializedAs("_CharController")]
    [Header("Components")] 
    [SerializeField] protected Rigidbody _Rbody;                    // Reference to the Character Controller
    
    protected virtual void Awake()
    {
        // Get reference to the character controller
        if (!_Rbody)
            _Rbody = this.GetComponent<Rigidbody>();

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
        if (!_Rbody)
            return;

        var movementInput = _Input.Player.Move.ReadValue<Vector2>();                    // Get the input value as vector 2
        _PlayerMovementInput = new Vector3(movementInput.x, 0f, movementInput.y);      // Create the movement value

        if (_PlayerMovementInput.normalized.magnitude > 0.1f)
        {
            var finalMovement = _PlayerMovementInput * (GetMovementSpeed() * Time.fixedDeltaTime);          // Calculate final movement direction

            Quaternion toRotation = Quaternion.LookRotation(finalMovement, Vector3.up);             // Get where we want to rotate the character t
            _Rbody.velocity = new Vector3(finalMovement.x, finalMovement.y, finalMovement.z);
            // Apply rotation smoothly
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, (_RotationSpeed * Time.deltaTime));
        }


    }

    /// <summary>
    /// Gets the target movement speed
    /// </summary>
    /// <returns>Movement speed</returns>
    private float GetMovementSpeed()
    {
        if (OnSurface == ESurfaceType.SAND)
            return _SandMovementSpeed;

        if (OnSurface == ESurfaceType.REGULAR || OnSurface == ESurfaceType.ICE)
            return _GeneralMovementSpeed;
    }
}
