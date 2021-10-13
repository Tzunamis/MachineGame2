using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private static float _roundDuration; // How long is a round?
    private static float _roundTimer; // How far have we progressed into the current round?
    private bool _isRoundStarted = false;
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

    public ControlScript controlScript;
    public Spawner[] spawners;
    public GameObject[] machineParents;

    public enum TeamList
    {
        Team1,
        Team2,
        Team3,
        Team4
    }

    public int numberTeams = 4;
    public int numPlayersPerTeam = 4;
    public int numMachinesPerTeam = 20;

    public struct TeamData
    {
        public TeamData(int initialScore, GameObject[] initialPlayerList, Machine[] initialMachineList)
        {
            score = initialScore;
            playerList = initialPlayerList;
            machineList = initialMachineList;
            playerIDToSpawn = 0;
        }

        public int score;
        public GameObject[] playerList;
        public Machine[] machineList;
        public int playerIDToSpawn;
    }

    public Dictionary<TeamList, TeamData> Teams;


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
        
        InitializeTeams();

        InitializeRound();

        _roundDuration = controlScript.roundDuration;
        _isRoundStarted = true;
    }

    private void InitializeTeams()
    {
        Teams = new Dictionary<TeamList, TeamData>();

        // Assign machines to teams
            // For this to work, all machines need to be children of "machineParent" objects which are linked in the inspector
        
        // Makes one list of machines for each team
        List<Machine>[] machines = new List<Machine>[numberTeams];

        foreach (GameObject machineParent in machineParents)
        {
            // Iterate through machine parent to find machines
            for(int i = 0; i < transform.childCount; i++)
            {
                Machine machineToBeStored = machineParent.transform.GetChild(i).GetComponent<Machine>();
                // Add machine to correct list based on machine's teamID
                machines[(int)machineToBeStored.teamID].Add(machineToBeStored);
            }
        }

        //for(int i = 0; i < machines.Length; i++)
        //{
        //    Teams[(TeamList)i].machineList
        //    machineList.ToArray;
        //}

         



        for (int i = 0; i < numberTeams; i++)
        {
            //Create player gaemobject instantiate them and place them in a gameobject array to be placed on the line under
            for(int j = 0; j < numPlayersPerTeam; j++)
            {

            }

            //Teams[(TeamList)i] = new TeamData(0, /*List of player gameobjects*/, /*List of machines*/);
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

            currentTeam.playerIDToSpawn = (currentTeam.playerIDToSpawn >= numPlayersPerTeam) ? 0 : currentTeam.playerIDToSpawn++;

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
