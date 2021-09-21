using UnityEngine;

public class Machine : MonoBehaviour
{

    // FOLLOWING ATTRIBUTE SHOULD BE FETCHED FROM A ROUND MANAGER CLASS
    private float _roundDuration; // Duration of a round in seconds

    public GameObject product; // What the machine produces, if anything
    public float _productRate; // How many items the machine produces per round
    [SerializeField]
    private float _productProgress; // Current progress (timer) for producing current item
    private float _damageModifier = 1; // How does the machine's damage affect its production rate?
    

    //-----------------HEAT RELATED PARAMETERS----------------------
    [SerializeField]
    private float _currentHeat; // Machine's current heat level
    private int _adjustedHeat; // Rounded heat to an int, used for calculating heat states
    private int _prevAdjustedHeat; // Previous adjusted heat, used to skip unnecessary logic
    [SerializeField]
    private float _heatRate; // Machine's rate of change per round (# of times it will change state in a round).

    // Heat indicator
    private SpriteRenderer _heatIndicator; // Sprite that displays heat information to the player
    private Color _heatColorSafe; // Colour of heat indicator when the machine is in a good state
    private Color _heatColorDanger; // Colour of heat indicator when the machine is no longer stable
    private Color _heatColorCritical; // Colour of heat indicator when the machine is close to exploding

    //-----------------EXPLOSION PARAMETERS-------------------------
    private float _explosionRadius = 4; // Radius of machine's explosion
    private float _fireChance = 0.5f; // Likelyhood of an object catching fire in an explosion
    private float _explosionHeatShift = 1; // How much does an explosion affect the heat of nearby machines?


    //-----------------REPAIR ITEM PARAMETERS------------------------
    [SerializeField]
    private GameObject[] _repairObjectArray; // List of objects the machine might request for repair
    private GameObject _repairObject; // What item is currently needed to repair the machine

    private void Awake()
    {
        // Set initial heat to 0
        _currentHeat = 0;
        // Set explosion radius
        _explosionRadius = 4;

        // ----------HEAT INDICATOR------------
        // Find heat indicator
        _heatIndicator = transform.Find("MachineSprite").transform.Find("HeatIndicator").GetComponent<SpriteRenderer>();
        // Set heat indicator colours
        _heatColorSafe = new Color(0, 1, 0);
        _heatColorDanger = new Color(1, 0.64f, 0);
        _heatColorCritical = new Color(1, 0, 0);
        // Set heat colour to safe as a starting point
        SetHeatIndicator(_heatColorSafe);
    }


    private void Update()
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
        if(_adjustedHeat != _prevAdjustedHeat)
        {
            AdjustMachineState();
        }

        // Increment product progress
        _productProgress += (_productRate * _damageModifier * Time.deltaTime / RoundManager.RoundDuration);

        // Spawn product
        if (_productProgress >= 1)
        {
            ProduceItem();
        }


        // Current time:
            // RoundManager.RoundTimer;

        // Round duration:
            // RoundManager.RoundDuration;

    }

    // Spits out an item
    private void ProduceItem()
    {
        if(product != null)
        {
            Instantiate(product, transform.position + -transform.up, Quaternion.identity);
        }
        _productProgress = 0;
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
                _damageModifier = 0;
                break;
            // Hot state 1
            case 1:
                SetHeatIndicator(_heatColorDanger);
                ToggleRepairRequest(true);
                RequestRepair();
                _damageModifier = 0.75f;
                break;
            // Hot state 2
            case 2:
                SetHeatIndicator(_heatColorCritical);
                _damageModifier = 0.5f;
                break;
            // Hot state 3 (explosion)
            case 3:
                Explode();
                break;
            // Cold state 1 (unimplemented)
            case -1:
                break;
            // Cold state 2 (unimplemented)
            case -2:
                break;
            // Cold state 3 (freeze) (unimplemented)
            case -3:
                break;
        }
    }

    // Changes colour of heat indicator
    private void SetHeatIndicator(Color newColour)
    {
        _heatIndicator.color = newColour;
    }

    // Requests an object for repair
    private void RequestRepair()
    {
        // ------------CHOOSE REPAIR OBJECT----------------
        // Generate random int
        int rand = Random.Range(0, _repairObjectArray.Length);
        // Set random repair object
        _repairObject = _repairObjectArray[rand];

        // ------------DISPLAY SPRITE---------------------
        // THIS PART WILL CHANGE ONCE WE HAVE ACTUAL SPRITES
        // Find sprite to change
        Transform repairRequest = transform.Find("MachineSprite").transform.Find("RepairRequest").transform.Find("RequestSprite");
        // Find reference sprite from randomly chosen repair object
        SpriteRenderer requestSprite = _repairObject.GetComponent<SpriteRenderer>();
        // Change attributes accordingly
        repairRequest.GetComponent<SpriteRenderer>().sprite = requestSprite.sprite;
        repairRequest.GetComponent<SpriteRenderer>().color = requestSprite.color;
        repairRequest.transform.localScale = _repairObject.transform.lossyScale;
    }

    // Enables or disables repair request bubble
    private void ToggleRepairRequest(bool value)
    {
        transform.Find("MachineSprite").transform.Find("RepairRequest").gameObject.SetActive(value);
    }

    // Handles explosion
    private void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        Debug.Log("Hits: " + hitColliders.Length);

        for(int i = 0; i < hitColliders.Length; i++)
        {
            GameObject hitObject = hitColliders[i].gameObject;

            if (hitObject.layer == LayerMask.NameToLayer("Machine"))
            {
                Debug.Log("Hit a machine");
                hitObject.SendMessage("IncrementHeat");
            }
            if (hitObject.layer == LayerMask.NameToLayer("Floor"))
            {
                Debug.Log("Hit a floor tile");

                if(Random.value <= _fireChance)
                hitObject.SendMessage("LightOnFire");
            }

        }

        Destroy(gameObject);
    }

    // Used to increase heat when a nearby explosion occurs
    private void IncrementHeat()
    {
        _currentHeat += _explosionHeatShift;
    }
}
