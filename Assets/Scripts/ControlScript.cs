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
    public float fireProbability;
    [Header("Fire on Player")]
    public float minFireDurationPlayer;
    public float maxFireDurationPlayer;
    [Header("Fire on Ground")]
    public float minIceDurationFloor;
    public float maxIceDurationFloor;
    public float iceProbability;
    [Header("Freeze on Player")]
    public float minFreezeDuration;
    public float maxFreezeDuration;
    [Header("Movement Speed")]
    public float fireMultiplier;
    public float freezeMultiplier;
    [Header("Round Manager")]
    public float roundDuration;
    public float scoreFrequency;
    public int numOfRoundsPerTeam;
    public int numTeams;
    public int numPlayersPerTeam;
    public int numMachinesPerTeam;
    public bool isSimultaneous;
    public bool rotateCamera;
    [Header("Other")]
    public float productRate;
    public float explosionRadius;
    public float itemHeatIncrement;
    public float playerScale;

}
