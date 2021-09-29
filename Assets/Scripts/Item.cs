using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, Interactable
{
    
    public void Interact(Player player)
    {
        Debug.Log("Item interaction");
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0.25f, -0.25f, 0);
        player._heldItem = this;
    }

}
