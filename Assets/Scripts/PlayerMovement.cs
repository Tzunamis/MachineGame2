using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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

    //----------------MOVEMENT-----------------------
    [SerializeField] private InputAction movement;
    
    [SerializeField] private float speed;


    private void Awake()
    {
        // TODO: set animator
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
            gameObject.transform.position += new Vector3(speed, 0, 0);
        }
        else if (Horizontal == -1)
        {
            gameObject.transform.position += new Vector3(-speed, 0, 0);
        }

        if (Vertical == 1)
        {
            gameObject.transform.position += new Vector3(0, speed, 0);
        }
        else if (Vertical == -1)
        {
            gameObject.transform.position += new Vector3(0, -speed, 0);
        }
    }

    // Movement stuff below
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
