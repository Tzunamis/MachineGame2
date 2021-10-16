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
    public InputAction movement;
    
    [SerializeField] private float _speed;

    private bool _movingUp = false;
    private bool _movingDown = false;
    private bool _movingLeft = false;
    private bool _movingRight = false;

    //--------------FIRE AND ICE---------------------

    private float _fireMultiplier;
    private float _freezeMultiplier;
    private bool _isOnFire = false;
    private bool _isFrozen = false;

    //--------------POINTERS----------------------------
    [SerializeField] private SpriteRenderer _freezeSprite;
    [SerializeField] private ControlScript _controlScript;

    private float _freezeTimer;
    private float _minimumFreezeDuration;
    private float _maximumFreezeDuration;
    private float _currentFreezeDuration;

    //--------------ANIMATOR----------------------------
    Animator animator; 
    SpriteRenderer spriteRenderer; 

    private void Awake()
    {
        _controlScript = FindObjectOfType<ControlScript>().GetComponent<ControlScript>();

        // TODO: set animator
        movement.performed += OnMovementPerformed;
        movement.canceled += OnMovementPerformed;

        _minimumFreezeDuration = _controlScript.minFreezeDuration;
        _maximumFreezeDuration = _controlScript.maxFreezeDuration;

        _fireMultiplier = _controlScript.fireMultiplier;
        _freezeMultiplier = _controlScript.freezeMultiplier;

        _freezeSprite.enabled = false;

        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(_isFrozen)
        {
            _freezeTimer += Time.deltaTime;

            if(_freezeTimer >= _currentFreezeDuration)
            {
                Thaw();
            }
        }

        ManageAnimator();
    }

    private void FixedUpdate()
    {

        // Fire and ice cancel each other out
        if(_isOnFire && _isFrozen)
        {
            Extinguish();
            Thaw();
            gameObject.GetComponent<Player>().Extinguish();
        }

        if(!_isOnFire)
        {
            NormalMovement();
        }

        // Fire has a different form of movement
        else
        {
            FireMovement();
        }
        
    }

    private void NormalMovement()
    {
        

        float spd = _speed;
        if(_isFrozen)
        {
            spd = _speed * _freezeMultiplier;
        }

        transform.position += transform.up * spd * Vertical;
        transform.position += transform.right * spd * Horizontal;

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
            transform.position += transform.up * _speed * _fireMultiplier;
        }

        if (_movingDown)
        {
            transform.position -= transform.up * _speed * _fireMultiplier;
        }

        if (_movingLeft)
        {
            transform.position -= transform.right * _speed * _fireMultiplier;
        }

        if (_movingRight)
        {
            transform.position += transform.right * _speed * _fireMultiplier;
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

    public void Freeze()
    {
        _isFrozen = true;

        _freezeTimer = 0;
        _freezeSprite.enabled = true;

        _currentFreezeDuration = Random.Range(_minimumFreezeDuration, _maximumFreezeDuration);
    }

    public void Thaw()
    {
        _isFrozen = false;
        _freezeSprite.enabled = false;
    }

    private float Vertical { get; set; }

    private float Horizontal { get; set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isFrozen && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.collider.gameObject.SendMessage("Freeze");
        }
    }


    private void ManageAnimator()
    {
        if (Vertical == 0 && Horizontal == 0)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
            if (Horizontal < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (Horizontal >= 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }



    /*
     * if (Vertical > 0)
     *      move up
     * 
     * 
     * if (Vertical < 0)
     *     move down
     *     
     *     
     * if (Horizontal > 0)
     *     move right
     *     
     *     
     * if (Horizontal < 0)
     *     move left
     */

}
