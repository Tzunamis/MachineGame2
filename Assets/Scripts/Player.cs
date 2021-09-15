using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /*
     * Used the following tutorials:
     * 
     * UnityEngine.InputSystem
     * https://www.youtube.com/watch?v=3O9KHy58Fps&ab_channel=InfallibleCode
     * 
     * Multiple control schemes
     * https://www.youtube.com/watch?v=oZbIjeWZz60&ab_channel=InfallibleCode
     * 
     */

    // All changes to player movement should go in FixedUpdate

    [SerializeField] private InputAction movement;

    private void Awake()
    {
        // set animator


        movement.performed += OnMovementPerformed;
        movement.canceled += OnMovementPerformed;
    }
    private void FixedUpdate()
    {

        // Player control scripts go here

        // Horizontal = 1: right input
        // Horizontal = -1: left input

        // Vertical = 1: up input
        // Vertical = -1: down input


        if (Horizontal == 1)
        {
            gameObject.transform.position += new Vector3(1, 0, 0);
        }
        else if (Horizontal == -1)
        {
            gameObject.transform.position += new Vector3(-1, 0, 0);
        }
    }


    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();

        Horizontal = direction.x;
        Vertical = direction.y;
    }

    private void OnDisable()
    {
        movement.Disable();
    }

    private void OnEnable()
    {
        movement.Enable();
    }

    private float Vertical { get; set; }

    private float Horizontal { get; set; }
}
