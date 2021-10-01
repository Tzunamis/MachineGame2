using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{


    // All changes to player movement should go in FixedUpdate


    private void Awake()
    {
        // TODO: set animator

        interact.performed += OnInteractionPerformed;
        interact.canceled += OnInteractionPerformed;
    }


    //----------------ITEM AND MACHINE INTERACTION-----------------
    [SerializeField] private float _interactionRadius;
    private LayerMask _interactableObjectLayerMask;
    private GameObject _interactableObject;
    public Item _heldItem;
    [SerializeField] private InputAction interact;


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

    

    private void FindInteractableObject()
    {
        // Set interactable objects based on whether the player is holding an item
        // If holding item, can only interact with machines
        if (_heldItem != null)
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

    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        if (_interactableObject != null)
        {
            Interactable interactable = _interactableObject.GetComponent<Interactable>();
            interactable.Interact(this);

        }


    }

    

    

    

    private void OnDisable()
    {
        interact.Disable();
    }

    private void OnEnable()
    {
        interact.Enable();
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);
    }
}
