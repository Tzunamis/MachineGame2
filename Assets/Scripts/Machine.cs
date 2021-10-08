using UnityEngine;

public class Machine : MonoBehaviour, Interactable
{

    public GameObject controlScriptGameObject;


    // FOLLOWING ATTRIBUTE SHOULD BE FETCHED FROM A ROUND MANAGER CLASS
    private float _roundDuration; // Duration of a round in seconds

    //-----------------PRODUCT PARAMETERS----------------------
    public GameObject product; // What the machine produces, if anything
    public float _productRate; // How many items the machine produces per round
    [SerializeField]
    private float _productProgress; // Current progress (timer) for producing current item
    

    //-----------------HEAT RELATED PARAMETERS----------------------
    [SerializeField]
    private float _currentHeat; // Machine's current heat level
    private int _adjustedHeat; // Rounded heat to an int, used for calculating heat states
    private int _prevAdjustedHeat; // Previous adjusted heat, used to skip unnecessary logic
    [SerializeField]
    private float _heatRate; // Machine's rate of change per round (# of times it will change state in a round).
    [SerializeField]
    private float _itemHeatIncrement; // How much items change machine heat

    // Heat indicator
    private SpriteRenderer _heatIndicator; // Sprite that displays heat information to the player
    private Color _heatColorSafe; // Colour of heat indicator when the machine is in a good state
    private Color _heatColorHot; // Colour of heat indicator when the machine is starting to heat up
    private Color _heatColorVeryHot; // Colour of heat indicator when the machine is close to exploding
    private Color _heatColorCold; // Colour of heat indicator when the machine is starting to cool down
    private Color _heatColorVeryCold; // Colour of heat indicator when the machine is close to cold implosion

    //-----------------EXPLOSION PARAMETERS-------------------------
    private float _explosionRadius; // Radius of machine's explosion
    private float _fireChance; // Likelyhood of an object catching fire in an explosion
    private float _explosionHeatShift = 1; // How much does an explosion affect the heat of nearby machines?
    private bool _isBroken;


    //-----------------REPAIR ITEM PARAMETERS------------------------
    [SerializeField]
    private GameObject[] _repairObjectArray; // List of objects the machine might request for repair

    private void Awake()
    {
        _isBroken = false;

        ControlScript controlScript = controlScriptGameObject.GetComponent<ControlScript>();

        // Set initial heat
        _currentHeat = Random.Range(controlScript.minStartingHeat, controlScript.maxStartingHeat);
        // Set heat rate
        _heatRate = Random.Range(controlScript.minHeatRate, controlScript.maxHeatRate);
        // Chance for negative heat rate
        if(Random.Range(0f, 1f) <= 0.5f)
        {
            _heatRate = -_heatRate;
        }
        // Set product rate
        _productRate = controlScript.productRate;
        // Set explosion radius
        _explosionRadius = controlScript.explosionRadius;
        // Set item heat increment
        _itemHeatIncrement = controlScript.itemHeatIncrement;
        // Set fire probability
        _fireChance = controlScript.fireProbability;

        // ----------HEAT INDICATOR------------
        // Find heat indicator
        _heatIndicator = transform.Find("MachineSprite").transform.Find("HeatIndicator").GetComponent<SpriteRenderer>();
        // Set heat indicator colours
        _heatColorSafe = new Color(0, 1, 0);
        _heatColorHot = new Color(1, 0.64f, 0);
        _heatColorVeryHot = new Color(1, 0, 0);
        _heatColorCold = new Color(0.2f, 0.9f, 1);
        _heatColorVeryCold = new Color(0.75f, 1, 1);
        // Set heat colour to safe as a starting point
        SetHeatIndicator(_heatColorSafe);
    }


    private void Update()
    {
        // If the machine's broken, it won't do these things
        if(!_isBroken)
        {
            ManageHeat();
            ManageProduction(); 
        }
    }

    // Increments heat and checks for a change in state
    private void ManageHeat()
    {
        // Increment current heat
        _currentHeat += (_heatRate * Time.deltaTime) / RoundManager.RoundDuration;

        // Set previous adjusted heat
        _prevAdjustedHeat = _adjustedHeat;

        // Update adjusted heat
        if (_currentHeat >= 0)
            _adjustedHeat = (int)Mathf.Floor(_currentHeat);
        else
            _adjustedHeat = (int)Mathf.Ceil(_currentHeat);

        // Update states according to adjusted heat
        if (_adjustedHeat != _prevAdjustedHeat)
        {
            AdjustMachineState();
        }
    }

    

    //Changes machine's state according to its heat
    private void AdjustMachineState()
    {
        switch (_adjustedHeat)
        {
            // Safe state
            case 0:
                SetHeatIndicator(_heatColorSafe);
                ToggleRepairRequest(false);
                break;
            // Hot state 1
            case 1:
                SetHeatIndicator(_heatColorHot);
                ToggleRepairRequest(true);
                RequestRepair(_repairObjectArray[0]);
                break;
            // Hot state 2
            case 2:
                SetHeatIndicator(_heatColorVeryHot);
                break;
            // Hot state 3 (explosion)
            case 3:
                RequestRepair(_repairObjectArray[2]);
                Explode();
                break;
            // Cold state 1
            case -1:
                SetHeatIndicator(_heatColorCold);
                ToggleRepairRequest(true);
                RequestRepair(_repairObjectArray[1]);
                break;
            // Cold state 2
            case -2:
                SetHeatIndicator(_heatColorVeryCold);
                break;
            // Cold state 3 (freeze) (unimplemented)
                // lower nearby machine temperatures
                // slow down players
            case -3:
                RequestRepair(_repairObjectArray[2]);
                Implode();
                break;
        }
    }

    // Changes colour of heat indicator
    private void SetHeatIndicator(Color newColour)
    {
        _heatIndicator.color = newColour;
    }

    // Manages production of items
    private void ManageProduction()
    {
        // Increment product progress
        if (transform.GetComponentInChildren<Item>() == null)
        {
            _productProgress += (_productRate * Time.deltaTime / RoundManager.RoundDuration);
        }

        // Spawn product
        if (_productProgress >= 1)
        {
            ProduceItem();
        }
    }

    // Spits out an item
    private void ProduceItem()
    {
        if (product != null)
        {
            Instantiate(product, transform.position + -transform.up * transform.localScale.x, Quaternion.identity, transform);
        }
        _productProgress = 0;
    }

    // Requests an object for repair
    private void RequestRepair(GameObject repairObject)
    {
        // ------------DISPLAY SPRITE---------------------
        // Find sprite to change
        Transform repairRequest = transform.Find("MachineSprite").transform.Find("RepairRequest").transform.Find("RequestSprite");
        // Find reference sprite from randomly chosen repair object
        SpriteRenderer requestSprite = repairObject.GetComponent<SpriteRenderer>();
        // Change attributes accordingly
        repairRequest.GetComponent<SpriteRenderer>().sprite = requestSprite.sprite;
        repairRequest.GetComponent<SpriteRenderer>().color = requestSprite.color;
        repairRequest.transform.localScale = repairObject.transform.lossyScale;
    }

    // Enables or disables repair request bubble
    private void ToggleRepairRequest(bool value)
    {
        transform.Find("MachineSprite").transform.Find("RepairRequest").gameObject.SetActive(value);
    }

    // Handles explosion
    private void Explode()
    {
        // Check for colliders within radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        if(hitColliders != null)
        {
            // Iterate through each collider
            foreach(Collider2D col in hitColliders)
            {
                GameObject hitObject = col.gameObject;

                // If it's a machine, increase the heat
                if (hitObject.layer == LayerMask.NameToLayer("Machine"))
                {
                    hitObject.SendMessage("IncrementHeat");
                }

                // If it's a floor tile, random chance to set on fire
                if (hitObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    if (Random.value <= _fireChance)
                        hitObject.SendMessage("LightOnFire");
                }

                // If it's a player, light them on fire
                if(hitObject.layer == LayerMask.NameToLayer("Player"))
                {
                    hitObject.SendMessage("LightOnFire");
                }

            }
        }

        // Remove the heat indicator to show the machine is broken
        _heatIndicator.enabled = false;
        // Is broken
        _isBroken = true;
    }

    // Handles implosion (cold explosion)
    private void Implode()
    {
        // Check for colliders within radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        if (hitColliders != null)
        {
            // Iterate through each collider
            foreach (Collider2D col in hitColliders)
            {
                GameObject hitObject = col.gameObject;

                // If it's a machine, decrease the heat
                if (hitObject.layer == LayerMask.NameToLayer("Machine"))
                {
                    hitObject.SendMessage("DecrementHeat");
                }

                // If it's a floor tile on fire, put out the fire
                if (hitObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    hitObject.SendMessage("ExtinguishFire");
                }

                // If it's a player, freeze them
                if (hitObject.layer == LayerMask.NameToLayer("Player"))
                {
                    hitObject.SendMessage("Freeze");
                }
            }
        }
        // Remove the heat indicator to show it's broken
        _heatIndicator.enabled = false;
        // Is broken
        _isBroken = true;
    }

    // Used to increase heat when a nearby explosion occurs
    private void IncrementHeat()
    {
        _currentHeat += _explosionHeatShift;
    }

    // Used to decrease heat when a nearby implosion occurs
    private void DecrementHeat()
    {
        _currentHeat -= _explosionHeatShift;
    }

    public void PlayerInteraction(Player player)
    {
        // Effect on machine based on identity of held item
        switch (player._heldItem.gameObject.name)
        {
            case "Gear(Clone)":
                if(_isBroken)
                {
                    _currentHeat = 0;
                    _heatIndicator.enabled = true;
                    _isBroken = false;
                }
                break;
            case "Lightbulb(Clone)":
                _currentHeat += _itemHeatIncrement;
                break;
            case "Pipe(Clone)":
                _currentHeat -= _itemHeatIncrement;
                break;
        }

        Destroy(player._heldItem.gameObject);
    }

}
