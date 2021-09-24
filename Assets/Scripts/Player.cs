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

    //----------------MOVEMENT-----------------------
    [SerializeField] private InputAction movement;
    [SerializeField] private float speed;

    //----------------ITEM AND MACHINE INTERACTION-----------------
    [SerializeField] private float _interactionRadius;
    private bool _holdingItem;
    private LayerMask _interactableObjectLayerMask;
    private GameObject _interactableObject;

    private void Awake()
    {
        // TODO: set animator
        movement.performed += OnMovementPerformed;
        movement.canceled += OnMovementPerformed;

        _holdingItem = false;
    }

    private void Update()
    {
        FindInteractableObject();
        if(_interactableObject != null)
        {
            Debug.Log("interactableObject set");
        }
        else
        {
            Debug.Log("no interactable object");
        }
        
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

    private void FindInteractableObject()
    {
        // Set interactable objects based on whether the player is holding an item
        // If holding item, can only interact with machines
        if (_holdingItem)
        {
            _interactableObjectLayerMask = 1 << 6;
            Debug.Log("Looking for machines");
        }
        // Otherwise, can only interact with items
        else
        {
            _interactableObjectLayerMask = 1 << 8;
            Debug.Log("Looking for items");
        }

        // Check for interactable objects within radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _interactionRadius, _interactableObjectLayerMask);


        if (hitColliders.Length == 0)
        {
            _interactableObject = null;
        }
        else
        {
            
            // Figure out which interactible object is closest
            float minDistance = _interactionRadius;
            foreach (Collider2D col in hitColliders)
            {
                GameObject hitObject = col.gameObject;
                float distanceToObject = (transform.position - hitObject.transform.position).magnitude;
                // If current object is closest, set it as _interactableObject
                if (distanceToObject <= minDistance)
                {
                    _interactableObject = hitObject;
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
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
