using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private static float _roundDuration; // How long is a round?
    private static float _roundTimer; // How far have we progressed into the current round?
    public GameObject controlScriptGameObject;

    public static float RoundDuration
    {
        get
        {
            return _roundDuration;
        }
            
    }

    public static float RoundTimer
    {
        get
        {
            return _roundTimer;
        }

    }

    // Singleton pattern
    private static RoundManager _instance;

    public static RoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                RoundManager singleton = GameObject.FindObjectOfType<RoundManager>();
                if (singleton == null)
                {
                    GameObject go = new GameObject();
                    _instance = go.AddComponent<RoundManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _roundDuration = controlScriptGameObject.GetComponent<ControlScript>().roundDuration;
        Debug.Log("Round duration: " + _roundDuration);
    }

    private void Update()
    {
        _roundTimer += Time.deltaTime;
    }

}
