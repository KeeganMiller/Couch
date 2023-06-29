using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float _GeneralMovementSpeed;              // Speed the player object will move at
    [SerializeField] protected float _SandMovementSpeed;                 // Speed the player moves at when on sand
    [SerializeField] protected float _IceMovementSpeed;                 // Speed the player will move at when on ice
    [SerializeField, Tooltip("Speed the character rotates in the movement direction")] 
    protected float _RotationSpeed;                // Speed at which the character will rotation at
    [SerializeField, Tooltip("How much the character drags when on standard material (The lower the more it slides")]
    protected float _StandardDrag = 10f;
    [SerializeField, Tooltip("How much the cahracter drags when on ice (The lower the more it slides)")] 
    protected float _IceDrag = 0f;
    [SerializeField] private float _GeneralAcceleration = 75f;
    [SerializeField] private float _IceAcceleration = 20f;
    private Vector3 _PlayerMovementInput;                       // Reference to the current input

    [Header("Acceleration Properties")] 
    [SerializeField] protected float _MinAcceleration = -5.0f;
    [SerializeField] protected float _MaxAcceleration = 5.0f;
    private float _AccelerationX;
    private float _AccelerationY;
    private bool _UseAcceleration = false;
    private static float _AccelerationXLerpTime = 1.0f;
    private static float _AccelerationYLerpTime;
    [SerializeField] protected float _AccelerationSpeed = 0.2f;
    private float _LerpToPointX = 0.5f;
    private float _LerpToPointY = 0.5f;
    

    [Header("Teleport Amount")] 
    [SerializeField, Tooltip("Amount we move the character forward by")] private float _TeleportAmount;          
    

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

        var movementInput = _PlayerMap.FindAction("Move").ReadValue<Vector2>();                 // Read the player input
        Vector3 lastMovementInput = _PlayerMovementInput;                       // Get a reference to the last input to determine if we have changed direction
        _PlayerMovementInput = new Vector3(movementInput.x, 0f, movementInput.y);      // Create the movement value

        // Update the point we need to lerp to and which direction to lerp in
        bool lerpForwardX = UpdateLerpXPoint();
        bool lerpForwardY = UpdateLerpYPoint();

        UpdateXAcceleration(lerpForwardX);
        UpdateYAcceleration(lerpForwardY);

        if (_PlayerMovementInput.x == 0)
            _AccelerationX = 0f;

        if (_PlayerMovementInput.z == 0)
            _AccelerationY = 0f;


        if (_PlayerMovementInput.normalized.magnitude > 0.1f)
        {
            var MovementDirection = Vector3.zero;
            var finalAcceleration = Vector3.zero;
            MovementDirection = _PlayerMovementInput * (GetMovementSpeed() * Time.fixedDeltaTime);          // Calculate final movement direction
            
            Vector3 forwardTransform = this.transform.forward;              // Get the forward direction

            // If using acceleration than apply the acceleration modifier
            if (_UseAcceleration)
            {
                finalAcceleration = new Vector3(_AccelerationX, 0f, _AccelerationY);                // Add the acceleration into a vector
                // Apply the foward direction to the final direction
                finalAcceleration = new Vector3
                {
                    x = forwardTransform.x + finalAcceleration.x,
                    y = forwardTransform.y,
                    z = forwardTransform.z + finalAcceleration.z
                };
            }
            else
            {
                finalAcceleration = Vector3.zero;
            }
            
            _Rbody.AddForce((transform.forward + finalAcceleration) * GetMovementSpeed(), ForceMode.Acceleration);

            Quaternion toRotation = Quaternion.LookRotation(MovementDirection, Vector3.up);             // Get where we want to rotate the character t
            //_Rbody.velocity = new Vector3(finalMovement.x, finalMovement.y, finalMovement.z);
            _Rbody.AddForce(transform.forward * GetMovementSpeed(), ForceMode.Acceleration);
            // Apply rotation smoothly
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, (_RotationSpeed * Time.fixedDeltaTime));
        }


    }

    private void UpdateXAcceleration(bool lerpForward)
    {
        if (!_UseAcceleration)
            return; 
        // Update the acceleration lerp
        if (_AccelerationXLerpTime != _LerpToPointX)
        {
            _AccelerationX = Mathf.Lerp(_MinAcceleration, _MaxAcceleration, _AccelerationXLerpTime);
            if (lerpForward)
            {
                _AccelerationXLerpTime += _AccelerationSpeed * Time.deltaTime;
                if (_AccelerationXLerpTime > 1.0f)
                    _AccelerationXLerpTime = 1.0f;
            }
            else
            {
                _AccelerationXLerpTime -= _AccelerationSpeed * Time.deltaTime;
                if (_AccelerationXLerpTime < 0.0f)
                    _AccelerationXLerpTime = 0.0f;
            }
        }
    }

    private void UpdateYAcceleration(bool lerpForward)
    {
        if (!_UseAcceleration)
            return;
        
        if (_AccelerationYLerpTime != _LerpToPointY)
        {
            _AccelerationY = Mathf.Lerp(_MinAcceleration, _MaxAcceleration, _AccelerationYLerpTime);
            if (lerpForward)
            {
                _AccelerationYLerpTime += _AccelerationSpeed * Time.deltaTime;
                if (_AccelerationYLerpTime > 1.0f)
                    _AccelerationYLerpTime = 1.0f;
            }
            else
            {
                _AccelerationYLerpTime -= _AccelerationSpeed * Time.deltaTime;
                if (_AccelerationYLerpTime < 0.0f)
                    _AccelerationYLerpTime = 0.0f;
            }
        }
    }

    /// <summary>
    /// Updates the point we need to lerp to
    /// </summary>
    /// <returns>If we lerp forward or not</returns>
    private bool UpdateLerpXPoint()
    {

        if (_PlayerMovementInput.x == 0)
        {
            _LerpToPointX = 0.5f;
            if (_AccelerationXLerpTime > 0.5f)
            {
                return false;
            }

            return true;
        }
        if (_PlayerMovementInput.x > 0.1)
        {
            _LerpToPointX = 1.0f;
            return true;
        }

        if (_PlayerMovementInput.x < 0.1)
        {
            _LerpToPointX = 0.0f;
            return false;
        }

        return true;

    }

    private bool UpdateLerpYPoint()
    {

        if (_PlayerMovementInput.z == 0)
        {
            _LerpToPointY = 0.5f;
            if (_AccelerationXLerpTime > 0.5f)
                return false;

            return true;
        }
        
        if (_PlayerMovementInput.z > 0.1)
        {
            _LerpToPointY = 1.0f;
            return true;
        }

        if (_PlayerMovementInput.z < 0.1)
        {
            _LerpToPointY = 0.0f;
            return false;
        }

        return true;
    }

    public void OnTeleport(InputAction.CallbackContext ctx)
    {
        // Only perform event on button pressed down
        if (!ctx.performed)
            return;

        this.transform.position += this.transform.TransformDirection(Vector3.forward) * _TeleportAmount;
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
            _Rbody.drag = _IceDrag;                 // Apply the ice drag to the rb
            _UseAcceleration = true;                // Flag the acceleration
        }
        else
        {
            // Reset the acceleration
            _AccelerationX = 0f;
            _AccelerationY = 0f;
            // Set the drag to standard
            _Rbody.drag = _StandardDrag;
            // Set acceleration off
            _UseAcceleration = false;
        }
    }
}
