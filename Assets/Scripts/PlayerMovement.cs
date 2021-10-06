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
    [SerializeField] private InputAction _movement;
    
    [SerializeField] private float _speed;
    [SerializeField] private float _fireMultiplier;

    private bool _movingUp = false;
    private bool _movingDown = false;
    private bool _movingLeft = false;
    private bool _movingRight = false;

    private bool _isOnFire = false;
    

    private void Awake()
    {
        // TODO: set animator
        _movement.performed += OnMovementPerformed;
        _movement.canceled += OnMovementPerformed;

    }


    private void FixedUpdate()
    {

        // Player control scripts go here

        if(!_isOnFire)
        {
            NormalMovement();
        }

        else
        {
            FireMovement();
        }
        
    }

    private void NormalMovement()
    {
        // Need to make this more smooth

        if (Vertical == 1)
        {
            transform.position += new Vector3(0, _speed, 0);
        }
        else if (Vertical == -1)
        {
            transform.position += new Vector3(0, -_speed, 0);
        }

        if (Horizontal == 1)
        {
            transform.position += new Vector3(_speed, 0, 0);
        }
        else if (Horizontal == -1)
        {
            transform.position += new Vector3(-_speed, 0, 0);
        }
    }

    private void FireMovement()
    {
        // It's not pretty but it works

        if (Vertical == 1)
        {
            _movingUp = true;
            _movingDown = false;
            _movingLeft = false;
            _movingRight = false;
        }
        else if (Vertical == -1)
        {
            _movingDown = true;
            _movingUp = false;
            _movingLeft = false;
            _movingRight = false;
        }

        if (Horizontal == 1)
        {
            _movingRight = true;
            _movingLeft = false;
            _movingUp = false;
            _movingDown = false;
        }
        else if (Horizontal == -1)
        {
            _movingLeft = true;
            _movingRight = false;
            _movingUp = false;
            _movingDown = false;
        }


        if (_movingUp)
        {
            transform.position += new Vector3(0, _speed * _fireMultiplier, 0);
        }

        if (_movingDown)
        {
            transform.position += new Vector3(0, -_speed * _fireMultiplier, 0);
        }

        if (_movingLeft)
        {
            transform.position += new Vector3(-_speed * _fireMultiplier, 0, 0);
        }

        if (_movingRight)
        {
            transform.position += new Vector3(_speed * _fireMultiplier, 0, 0);
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
        _movement.Disable();
    }

    private void OnEnable()
    {
        _movement.Enable();
    }

    // Fire stuff

    public void LightOnFire()
    {
        _isOnFire = true;
        // Set movement direction based on current movement
    }

    public void Extinguish()
    {
        _isOnFire = false;
    }

    private float Vertical { get; set; }

    private float Horizontal { get; set; }
}
