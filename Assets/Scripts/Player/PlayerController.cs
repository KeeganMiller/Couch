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
    [SerializeField] protected float _IceMovementSpeed;                 // Speed the player will move at when on ice
    [SerializeField] protected float _RotationSpeed;                // Speed at which the character will rotation at
    [SerializeField] protected float _StandardDrag = 10f;
    [SerializeField] protected float _IceDrag = 0f;
    [SerializeField] private float _GeneralAcceleration = 75f;
    [SerializeField] private float _IceAcceleration = 20f;
    private Vector3 _PlayerMovementInput;                       // Reference to the current input

    [Header("Surface")]
    protected ESurfaceType _OnSurface;         // Reference to the surface we are currently on
    public ESurfaceType OnSurface => _OnSurface;

    [Header("Player Inputs")] 
    protected InputActionAsset _InputAsset;
    protected InputActionMap _PlayerMap;
    
    


    [FormerlySerializedAs("_CharController")]
    [Header("Components")] 
    [SerializeField] protected Rigidbody _Rbody;                    // Reference to the Character Controller
    
    protected virtual void Awake()
    {
        // Get reference to the character controller
        if (!_Rbody)
            _Rbody = this.GetComponent<Rigidbody>();

        _InputAsset = this.GetComponent<UnityEngine.InputSystem.PlayerInput>().actions;
        _PlayerMap = _InputAsset.FindActionMap("Player");

    }

    protected virtual void Start()
    {
        SetSurfaceType(ESurfaceType.REGULAR);
    }

    protected void OnEnable()
    {
        if (_PlayerMap != null)
            _PlayerMap.Enable();

    }

    protected void OnDisable()
    {
        if(_PlayerMap != null)
            _PlayerMap.Disable();
    }

    protected void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// Controls the character movement
    /// </summary>
    private void HandleMovement()
    {
        // Validate the rigidbody before handling movement
        if (!_Rbody)
            return;

        var movementInput = _PlayerMap.FindAction("Move").ReadValue<Vector2>();
        _PlayerMovementInput = new Vector3(movementInput.x, 0f, movementInput.y);      // Create the movement value

        if (_PlayerMovementInput.normalized.magnitude > 0.1f)
        {
            var finalMovement = _PlayerMovementInput * (GetMovementSpeed() * Time.fixedDeltaTime);          // Calculate final movement direction

            Quaternion toRotation = Quaternion.LookRotation(finalMovement, Vector3.up);             // Get where we want to rotate the character t
            //_Rbody.velocity = new Vector3(finalMovement.x, finalMovement.y, finalMovement.z);
            _Rbody.AddForce(transform.forward * GetMovementSpeed(), ForceMode.Acceleration);
            // Apply rotation smoothly
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, (_RotationSpeed * Time.fixedDeltaTime));
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

        if (OnSurface == ESurfaceType.REGULAR)
            return _GeneralMovementSpeed;

        if (OnSurface == ESurfaceType.ICE)
            return _IceMovementSpeed;

        return 0f;
    }

    /// <summary>
    /// Sets the surface type & updates the drag
    /// </summary>
    /// <param name="type">Surface Type</param>
    public void SetSurfaceType(ESurfaceType type)
    {
        _OnSurface = type;              // Set the surface type

        // Update the drag
        if (_OnSurface == ESurfaceType.ICE)
        {
            _Rbody.drag = _IceDrag;
        }
        else
        {
            _Rbody.drag = _StandardDrag;
        }
    }
}
