using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] protected float _MovementSpeed;              // Speed the player object will move at

    [Header("Components")] 
    [SerializeField] protected Rigidbody _RBody; 
    
    protected virtual void Awake()
    {
        if (!_RBody)
            _RBody = this.GetComponent<Rigidbody>();
    }

    protected void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        
    }
}
