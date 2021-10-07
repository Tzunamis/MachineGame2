using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    private bool _isOnFire;
    private float _minimumFireDuration;
    private float _maximumFireDuration;

    private float _currentFireDuration;
    private float _fireTimer;

    [SerializeField] private ControlScript _controlScript;

    [SerializeField] private SpriteRenderer _fireSprite;
    [SerializeField] private Collider2D _fireTriggerZone;

    private void Awake()
    {
        _isOnFire = false;
        _fireSprite.color = GetComponent<SpriteRenderer>().color;
        _fireTriggerZone.enabled = false;

        _minimumFireDuration = _controlScript.minFireDurationFloor;
        _maximumFireDuration = _controlScript.maxFireDurationFloor;
    }

    public void Update()
    {
        if(_isOnFire)
        {
            _fireTimer += Time.deltaTime;
            if(_fireTimer >= _currentFireDuration)
            {
                ExtinguishFire();
            }
        }
    }

    public void LightOnFire()
    {
        _isOnFire = true;
        _fireSprite.color = new Color(1, 0.13f, 0.13f, 0.5f);
        _fireTriggerZone.enabled = true;

        // Fire duration
        _currentFireDuration = Random.Range(_minimumFireDuration, _maximumFireDuration);
        _fireTimer = 0;
    }

    public void ExtinguishFire()
    {
        _isOnFire = false;
        _fireSprite.color = GetComponent<SpriteRenderer>().color;
        _fireTriggerZone.enabled = false;
    }
}
