using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    private bool _isOnFire;
    private SpriteRenderer _fireSprite;

    private void Awake()
    {
        _isOnFire = false;
        _fireSprite = transform.Find("FireSprite").GetComponent<SpriteRenderer>();
        _fireSprite.color = GetComponent<SpriteRenderer>().color;
    }

    public void LightOnFire()
    {
        _isOnFire = true;
        _fireSprite.color = new Color(1, 0.13f, 0.13f, 0.5f);
    }
}
