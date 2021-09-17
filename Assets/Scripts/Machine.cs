using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour
{

    // FOLLOWING ATTRIBUTE SHOULD BE FETCHED FROM A ROUND MANAGER CLASS
    private float _roundDuration; // Duration of a round in seconds

    public GameObject product; // What the machine produces, if anything
    public float _productRate; // How many items the machine produces per round

    [SerializeField]
    private float _currentHeat; // Machine's current heat level
    [SerializeField]
    private float _heatRate; // Machine's rate of change per round (# of times it will change state in a round).

    private int _adjustedHeat; // Rounded heat to an int, used for calculating heat states
    private int _prevAdjustedHeat; // Previous adjusted heat, used to skip unnecessary logic

    public GameObject repairItemNeeded; // What item is needed to repair the machine
    private Sprite _repairRequestSprite; // Sprite that appears when the machine is asking for a repair

    private SpriteRenderer _heatIndicator; // Sprite that displays heat information to the player
    private Color _heatColorSafe; // Colour of heat indicator when the machine is in a good state
    private Color _heatColorDanger; // Colour of heat indicator when the machine is no longer stable
    private Color _heatColorCritical; // Colour of heat indicator when the machine is close to exploding

    private GameObject _repairObject;
    [SerializeField]
    private GameObject[] _repairObjectArray;

    private void Awake()
    {
        // -------


        // ----------HEAT INDICATOR------------
        // Find heat indicator
        _heatIndicator = transform.Find("MachineSprite").transform.Find("HeatIndicator").GetComponent<SpriteRenderer>();
        // Set heat indicator colours
        _heatColorSafe = new Color(0, 1, 0);
        _heatColorDanger = new Color(1, 0.64f, 0);
        _heatColorCritical = new Color(1, 0, 0);
        // Set heat colour to safe as a starting point
        SetHeatIndicator(_heatColorSafe);

        // ----------HEAT PARAMETERS------------
        _heatRate = 4;
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
            switch (_adjustedHeat)
            {
                // Safe state
                case 0:
                    SetHeatIndicator(_heatColorSafe);
                    break;
                // Hot state 1
                case 1:
                    SetHeatIndicator(_heatColorDanger);
                    RequestRepair();
                    break;
                // Hot state 2
                case 2:
                    SetHeatIndicator(_heatColorCritical);
                    break;
                // Cold state 1 (unimplemented)
                case -1:
                    break;
                // Cold state 2 (unimplemented)
                case -2:
                    break;
            }
        }

        

        // Current time:
            // RoundManager.RoundTimer;

        // Round duration:
            // RoundManager.RoundDuration;

    }

    // Changes colour of heat indicator
    private void SetHeatIndicator(Color newColour)
    {
        _heatIndicator.color = newColour;
    }

    private void RequestRepair()
    {
        // Generate random int
        int rand = Random.Range(0, _repairObjectArray.Length);
        // Set random repair object
        _repairObject = _repairObjectArray[rand];
        // Display sprite for request
        Sprite requestSprite = _repairObject.GetComponent<SpriteRenderer>().sprite;
        transform.Find("RepairRequest").GetComponent<SpriteRenderer>().sprite = requestSprite;
    }


    // heat += (heatRate * time.deltaTime)/ roundDuration

    // product spawns every (roundDuration / productRate) seconds
    // Fetch from a global game timer (probably use modulo)
    // ...maybe? But what if we want the product rate to change based on heat state?

}
