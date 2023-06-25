using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESurfaceType
{
    REGULAR = 0,
    SAND = 1,
    ICE = 2
}

public class SurfaceController : MonoBehaviour
{
    [SerializeField] private ESurfaceType _SurfaceType;

    
    private void OnTriggerEnter(Collider other)
    {
        // Check that a player has entered the trigger
        if (other.CompareTag("Player"))
        {
            // Get reference to the player controller
            PlayerController player = other.GetComponent<PlayerController>();
            // Validate the player controller and set the On sourface property
            if (player)
                player.SetSurfaceType(_SurfaceType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player)
            {
                switch (_SurfaceType)
                {
                    case ESurfaceType.REGULAR:
                        break;
                    case ESurfaceType.ICE:
                        if (player.OnSurface == ESurfaceType.ICE)
                            player.SetSurfaceType(ESurfaceType.REGULAR);
                        break;
                    case ESurfaceType.SAND:
                        if(player.OnSurface == ESurfaceType.SAND)
                            player.SetSurfaceType(ESurfaceType.REGULAR);
                        break;
                    default:
                        Debug.LogWarning("Failed to update surface");
                        break;
                }
            }
        }
    }
}
