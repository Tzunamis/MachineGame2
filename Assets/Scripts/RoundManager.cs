using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public GameObject playerPrefab;
    public GameObject[] machineParents;

    [SerializeField] private InputActionReference interact = null;

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
        Teams = new Dictionary<TeamList, TeamData>();

        InitializeTeams();

        InitializeRound();

        _roundDuration = controlScript.roundDuration;
        _isRoundStarted = true;
    }

    private void InitializeTeams()
    {
        // Assign machines to teams
            // For this to work, all machines need to be children of "machineParent" objects which are linked in the inspector
        
        // Makes one list of machines for each team
        List<Machine>[] machines = new List<Machine>[numberTeams];

        for(int i = 0; i < numberTeams; i++)
        {
            machines[i] = new List<Machine>();
        }

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

        for (int i = 0; i < numberTeams; i++)
        {
            GameObject[] playersOnThisTeam = new GameObject[numPlayersPerTeam];

            //Create player gameobject instantiate them and place them in a gameobject array to be placed on the line under
            for (int j = 0; j < numPlayersPerTeam; j++)
            {
                GameObject playerToInstantiate = playerPrefab;
                // Anything else we need to do to players can happen here
                //players[i, j] = playerToInstantiate;
                playersOnThisTeam[j] = playerToInstantiate;
            }

            // Create team dictionary
            Teams.Add((TeamList)i, new TeamData(0, playersOnThisTeam, machines[i].ToArray()));
        }

    }

    private void InitializeRound()
    {
        //Kill any players existing / disable them (have a reference in TeamData)
        
        //Instantiate players

        foreach(Spawner currentSpawner in spawners)
        {
            TeamData currentTeam = Teams[currentSpawner.teamSpawnType];

            GameObject playerToSpawn = currentTeam.playerList[currentTeam.playerIDToSpawn];
            // Set player scale according to controlscript
            float playerScale = controlScript.playerScale;
            playerToSpawn.transform.localScale = new Vector3(playerScale, playerScale, 1);
            

            // Set controls
            //InputAction[] currentPlayerControls = SetPlayerControls(currentTeam.playerIDToSpawn);

            switch (currentTeam.playerIDToSpawn)
            {
                case 0:
                    Debug.Log("Setting p1 controls");
                    playerToSpawn.GetComponent<Player>().interact.ApplyBindingOverride(0, "<Keyboard>/tab");
                    //playerToSpawn.GetComponent<PlayerMovement>().movement.ChangeBindingWithGroup("Key.W");
                    break;
                case 1:
                    Debug.Log("Setting p2 controls");
                    playerToSpawn.GetComponent<Player>().interact.ApplyBindingOverride(0, "<Keyboard>/enter");
                    break;
                case 2:
                    Debug.Log("Setting p3 controls");
                    playerToSpawn.GetComponent<Player>().interact.ApplyBindingOverride(0, "<Keyboard>/space");
                    break;
                case 3:
                    Debug.Log("Setting p4 controls");
                    break;
            }

            //Place & Activate player
            Instantiate(playerToSpawn, currentSpawner.transform.position, Quaternion.Euler(0, 0, currentSpawner.spawnRotation));

            // Set active players
            // Maybe according to spawner?

            //Swap player to spawn
                
                //Didn't work: currentTeam.playerIDToSpawn = (currentTeam.playerIDToSpawn >= numPlayersPerTeam) ? currentTeam.playerIDToSpawn++ : 0;

            if (currentTeam.playerIDToSpawn >= numPlayersPerTeam - 1)
            {
                currentTeam.playerIDToSpawn = 0;
            }
            else
            {
                currentTeam.playerIDToSpawn++;
            }


            Teams[currentSpawner.teamSpawnType] = currentTeam;

            // Cycle spawners if needed
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
    }

    private void Update()
    {
        if (_isRoundStarted)
        {
            ManageRoundTimer();
        }
    }

    private void ManageRoundTimer()
    {
        _roundTimer += Time.deltaTime;
        if(_roundTimer >= RoundDuration)
        {
            //End of round stuff
            _roundTimer = 0;
        }
    }

}
