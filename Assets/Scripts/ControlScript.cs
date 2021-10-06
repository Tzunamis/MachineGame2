using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{
    [Header("Starting Heat")]
    public float minStartingHeat;
    public float maxStartingHeat;
    [Header("Heat Rate")]
    public float minHeatRate;
    public float maxHeatRate;
    [Header("Fire on Ground")]
    public float minFireDurationFloor;
    public float maxFireDurationFloor;
    [Header("Fire on Player")]
    public float minFireDurationPlayer;
    public float maxFireDurationPlayer;
    [Header("Other")]
    public float roundDuration;
    public float productRate;
    public float explosionRadius;
    public float itemHeatIncrement;


}
