using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private static float _roundDuration; // How long is a round?
    private static float _roundTimer; // How far have we progressed into the current round?
    public GameObject controlScriptGameObject;

    private bool _isRoundStarted = false;

    public Spawner[] spawners;

    public enum TeamList
    {
        Team1,
        Team2,
        Team3,
        Team4
    }

    public int numberTeams = 4;
    public int totalNumPlayersPerTeam = 4;

    public struct TeamData
    {
        public TeamData(int initialScore, GameObject[] initialPlayerList)
        {
            score = initialScore;
            playerList = initialPlayerList;
            playerIDToSpawn = 0;
        }

        public int score;
        public GameObject[] playerList;
        public int playerIDToSpawn;
    }

    public Dictionary<TeamList, TeamData> Teams;

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
        //_roundDuration = controlScriptGameObject.GetComponent<ControlScript>().roundDuration;
        Debug.Log("Round duration: " + _roundDuration);

        
        InitializeTeams();

        InitializeRound();

        _isRoundStarted = true;
    }

    private void InitializeTeams()
    {
        Teams = new Dictionary<TeamList, TeamData>();

        for(int i = 0; i < numberTeams; i++)
        {
            //Create player gaemobject instantiate them and place them in a gameobject array to be placed on the line under
            //Teams[(TeamList)i] = new TeamData(0, /*List of player gameobjects*/);
        }
    }

    private void InitializeRound()
    {
        //Kill any players existing / disable them (have a reference in TeamData)
        //Instantiate players
        foreach(Spawner currentSpawner in spawners)
        {
            TeamData currentTeam = Teams[currentSpawner.teamSpawnType];

            //Place & Activate player

            currentTeam.playerIDToSpawn = (currentTeam.playerIDToSpawn >= totalNumPlayersPerTeam) ? 0 : currentTeam.playerIDToSpawn++;

            if (currentSpawner.cycleTeams)
            {
                switch (currentSpawner.teamSpawnType)
                {
                    case TeamList.Team1:
                        currentSpawner.teamSpawnType = TeamList.Team2;
                        break;
                    case TeamList.Team2:
                        currentSpawner.teamSpawnType = TeamList.Team3;
                        break;
                    case TeamList.Team3:
                        currentSpawner.teamSpawnType = TeamList.Team4;
                        break;
                    case TeamList.Team4:
                        currentSpawner.teamSpawnType = TeamList.Team1;
                        break;
                }
            }
        }
        //Set _isRoundStarted
        //Cycle team spawner ID
        //Cycle current player to spawn
    }

    private void Update()
    {
        if (_isRoundStarted)
        {
            _roundTimer += Time.deltaTime;
        }
    }

}
