using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    // Fire variables
    public bool IsOnFire;
    private float _minimumFireDuration;
    private float _maximumFireDuration;

    private float _currentFireDuration;
    private float _fireTimer;

    // Ice variables
    public bool IsFrozen;
    private float _minimumIceDuration;
    private float _maximumIceDuration;

    private float _currentIceDuration;
    private float _iceTimer;

    // Pointers
    [SerializeField] private ControlScript _controlScript;

    [SerializeField] private SpriteRenderer _fireSprite;
    [SerializeField] private Collider2D _triggerZone;
    [SerializeField] private SpriteRenderer _iceSprite;

    private void Awake()
    {
        
        IsOnFire = false;
        _fireSprite.color = GetComponent<SpriteRenderer>().color;
        _triggerZone.enabled = false;

        _minimumFireDuration = _controlScript.minFireDurationFloor;
        _maximumFireDuration = _controlScript.maxFireDurationFloor;

        _iceSprite.enabled = false;

        _minimumIceDuration = _controlScript.minIceDurationFloor;
        _maximumIceDuration = _controlScript.maxIceDurationFloor;
    }

    public void Update()
    {
        if(IsOnFire && IsFrozen)
        {
            ExtinguishFire();
            Thaw();
        }

        if(IsOnFire)
        {
            _fireTimer += Time.deltaTime;
            if(_fireTimer >= _currentFireDuration)
            {
                ExtinguishFire();
            }
        }

        if(IsFrozen)
        {
            _iceTimer += Time.deltaTime;
            if(_iceTimer >= _currentIceDuration)
            {
                Thaw();
            }
        }
    }

    public void LightOnFire()
    {
        IsOnFire = true;
        _fireSprite.color = new Color(1, 0.13f, 0.13f, 0.5f);
        _triggerZone.enabled = true;

        //Debug.Log()

        // Fire duration
        _currentFireDuration = Random.Range(_minimumFireDuration, _maximumFireDuration);
        _fireTimer = 0;
    }

    public void ExtinguishFire()
    {
        IsOnFire = false;
        _fireSprite.color = GetComponent<SpriteRenderer>().color;
        _triggerZone.enabled = false;
    }

    public void Freeze()
    {
        IsFrozen = true;
        _iceSprite.enabled = true;
        _triggerZone.enabled = true;

        // Ice duration
        _currentIceDuration = Random.Range(_minimumIceDuration, _maximumIceDuration);
        _iceTimer = 0;
    }

    public void Thaw()
    {
        IsFrozen = false;
        _iceSprite.enabled = false;
        _triggerZone.enabled = false;
    }
}
