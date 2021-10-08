using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    //----------------ITEM AND MACHINE INTERACTION-----------------
    [SerializeField] private float _interactionRadius;
    private LayerMask _interactableObjectLayerMask;
    private GameObject _interactableObject;
    private GameObject _previousInteractableObject;
    public Item _heldItem;
    [SerializeField] private InputAction interact;

    //----------------FIRE STUFF------------------------------
    private bool _isOnFire;
    private float _minimumFireDuration;
    private float _maximumFireDuration;
    private float _currentFireDuration;
    private float _fireTimer;

    //-----------------POINTERS ------------------
    [SerializeField] private PlayerMovement _movementScript;
    [SerializeField] private ControlScript _controlScript;
    [SerializeField] private SpriteRenderer _fireSprite;

    private void Awake()
    {
        // TODO: set animator

        interact.performed += OnInteractionPerformed;
        _minimumFireDuration = _controlScript.minFireDurationPlayer;
        _maximumFireDuration = _controlScript.maxFireDurationPlayer;
        _fireSprite.enabled = false;
    }

    private void Update()
    {
        FindInteractableObject();

        if (_isOnFire)
        {
            _fireTimer += Time.deltaTime;
            if (_fireTimer >= _currentFireDuration)
            {
                Extinguish();
            }
        }
    }

    private void FindInteractableObject()
    {
        _interactableObject = _previousInteractableObject;

        // Set interactable objects based on whether the player is holding an item
        // If holding item, can only interact with machines
        if (_heldItem != null)
        {
            _interactableObjectLayerMask = 1 << 6;
        }
        // Otherwise, can only interact with items
        else
        {
            _interactableObjectLayerMask = 1 << 8;
        }

        // Check for interactable objects within radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 0.5f * transform.localScale.y), _interactionRadius, _interactableObjectLayerMask);


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

        // Manage glow
        /*
        Debug.Log(_interactableObject);
        if(_interactableObject != _previousInteractableObject)
        {
            //_previousInteractableObject.SendMessage("StopGlowing");

            Debug.Log("different item");

            
            if(_interactableObject != null)
            {
                _interactableObject.SendMessage("StartGlowing");
            }
            
        }
        else
            Debug.Log("same item");
        */

    }

    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {

        if(!_isOnFire) // Can't use items while you're on fire
        {
            if (_interactableObject != null)
            {
                Interactable interactable = _interactableObject.GetComponent<Interactable>();
                interactable.PlayerInteraction(this);
            }
            else if (_heldItem != null)
            {
                DropItem();
            }
        }

    }

    private void DropItem()
    {
        float xPos = Mathf.Floor(_heldItem.transform.position.x) + 0.5f;
        float yPos = Mathf.Floor(_heldItem.transform.position.y) + 0.5f;
        _heldItem.transform.position = new Vector2(xPos, yPos);
        _heldItem.gameObject.transform.parent = null;
        _heldItem = null;
    }

    private void OnDisable()
    {
        interact.Disable();
    }

    private void OnEnable()
    {
        interact.Enable();
    }

    public void LightOnFire()
    {
        _isOnFire = true;
        _fireTimer = 0;
        _fireSprite.enabled = true;

        Debug.Log("Set on fire");

        _currentFireDuration = Random.Range(_minimumFireDuration, _maximumFireDuration);

        if (_heldItem != null)
        {
            DropItem(); // Danger of dropping it in same location as a machine
        }

        _movementScript.LightOnFire();

    }

    public void Extinguish()
    {
        _isOnFire = false;
        _fireSprite.enabled = false;
        _movementScript.Extinguish();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 0.5f * transform.localScale.y), _interactionRadius);
    }

    private void OnTriggerStay2D (Collider2D collision)
    {
        GameObject TriggerObject = collision.gameObject;

        if (TriggerObject.layer == LayerMask.NameToLayer("FireIce"))
        {
            Floor floorScript = TriggerObject.transform.parent.GetComponent<Floor>();
            if(floorScript.IsOnFire)
            {
                LightOnFire();
            }
            if(floorScript.IsFrozen)
            {
                _movementScript.Freeze();
            }
            

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_isOnFire && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.collider.gameObject.SendMessage("LightOnFire");
        }
    }
}
