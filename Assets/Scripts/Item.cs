using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Interactable
{

    private SpriteRenderer _glow;

    private void Awake()
    {
        _glow = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _glow.enabled = false;
    }

    public void PlayerInteraction(Player player)
    {
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0.25f, 0.3f, 0);
        player._heldItem = this;
    }

    public void StartGlowing()
    {
        _glow.enabled = true;
    }

    public void StopGlowing()
    {
        _glow.enabled = false;
    }
}
