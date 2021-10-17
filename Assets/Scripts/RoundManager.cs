using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RoundManager : MonoBehaviour
{
    private static float _roundDuration; // How long is a round?
    private static float _roundTimer = 0; // How far have we progressed into the current round?
    private bool _isRoundStarted = false;
    private int _currentRound = 0;
    private float _scoreFrequency; // Poorly named. This is how often points are scored by the active team(s)
    private float _scoreTimer = 0;
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

    private int _numberTeams;
    private int _numPlayersPerTeam;
    private int _numMachinesPerTeam;
    private bool _isSimultaneous;

    public struct TeamData
    {

        public TeamData(bool startsActive, int initialScore, GameObject[] initialPlayerList, Machine[] initialMachineList)
        {
            isActive = startsActive;
            score = initialScore;
            playerList = initialPlayerList;
            machineList = initialMachineList;
            playerIDToSpawn = 0;
        }

        public bool isActive;
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

        _numberTeams = controlScript.numTeams;
        _numPlayersPerTeam = controlScript.numPlayersPerTeam;
        _numMachinesPerTeam = controlScript.numMachinesPerTeam;
        _isSimultaneous = controlScript.isSimultaneous;
        _scoreFrequency = controlScript.scoreFrequency;

        InitializeTeams();

        SpawnPlayers();

        InitializeRound();

        _roundDuration = controlScript.roundDuration;
        _isRoundStarted = true;
    }

    private void InitializeTeams()
    {
        // Assign machines to teams
            // For this to work, all machines need to be children of "machineParent" objects which are linked in the inspector
        
        // Makes one list of machines for each team
        List<Machine>[] machines = new List<Machine>[_numberTeams];

        for(int i = 0; i < _numberTeams; i++)
        {
            machines[i] = new List<Machine>();
        }

        foreach (GameObject machineParent in machineParents)
        {
            // Iterate through machine parent to find machines
            for(int i = 0; i < machineParent.transform.childCount; i++)
            {
                Machine machineToBeStored = machineParent.transform.GetChild(i).GetComponent<Machine>();
                // Add machine to correct list based on machine's teamID
                machines[(int)machineToBeStored.teamID].Add(machineToBeStored);
            }
        }

        for (int i = 0; i < _numberTeams; i++)
        {
            GameObject[] playersOnThisTeam = new GameObject[_numPlayersPerTeam];

            //Create player gameobject and place them in a gameobject array to be placed
            for (int j = 0; j < _numPlayersPerTeam; j++)
            {
                GameObject playerToInstantiate = playerPrefab;
                playersOnThisTeam[j] = playerToInstantiate;
            }

            // Check if teams play simultaneously to determine which teams start the game active
            bool startsActive = false;

            if(_isSimultaneous || i == 0)
            {
                startsActive = true;
            }

            // Create team dictionary
            Teams.Add((TeamList)i, new TeamData(startsActive, 0, playersOnThisTeam, machines[i].ToArray()));
        }

    }

    private void SpawnPlayers()
    {
        //Instantiate players

        foreach (Spawner currentSpawner in spawners)
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
                    playerToSpawn.GetComponent<Player>().interact.ApplyBindingOverride(0, "<Keyboard>/tab");
                    //playerToSpawn.GetComponent<PlayerMovement>().movement.ChangeBindingWithGroup("Key.W");
                    break;
                case 1:
                    playerToSpawn.GetComponent<Player>().interact.ApplyBindingOverride(0, "<Keyboard>/enter");
                    break;
                case 2:
                    playerToSpawn.GetComponent<Player>().interact.ApplyBindingOverride(0, "<Keyboard>/space");
                    break;
                case 3:
                    break;
            }

            // Set player active or inactive
            if (currentSpawner.teamSpawnType == TeamList.Team1 || _isSimultaneous)
            {
                playerToSpawn.SetActive(true);
            }
            else
            {
                playerToSpawn.SetActive(false);
            }

            //Place player and update teamdata reference
            currentTeam.playerList[currentTeam.playerIDToSpawn] = Instantiate(playerToSpawn, currentSpawner.transform.position, Quaternion.Euler(0, 0, currentSpawner.spawnRotation));

            //Swap player to spawn
                //Didn't work: currentTeam.playerIDToSpawn = (currentTeam.playerIDToSpawn >= numPlayersPerTeam) ? currentTeam.playerIDToSpawn++ : 0;
            if (currentTeam.playerIDToSpawn >= _numPlayersPerTeam - 1)
            {
                currentTeam.playerIDToSpawn = 0;
            }
            else
            {
                currentTeam.playerIDToSpawn++;
            }

            // Update teamdata
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

    private void InitializeRound()
    {
        if(!_isSimultaneous && _currentRound != 0)
        {
            //Cycle active players
            for (int i = 0; i < _numberTeams; i++)
            {
                if (Teams[(TeamList)i].isActive)
                {

                    Teams[(TeamList)i] = SetTeamActive(Teams[(TeamList)i], false);

                    // Determine next active team
                    int nextActiveTeamID = 0;

                    if(i < _numberTeams - 1)
                    {
                        nextActiveTeamID = i + 1;
                    }

                    Teams[(TeamList)nextActiveTeamID] = SetTeamActive(Teams[(TeamList)nextActiveTeamID], true);

                    //End for loop
                    i = _numberTeams;

                }
            }
        }

        
        
    }

    private TeamData SetTeamActive(TeamData team, bool isActive)
    {
        // Enable/disable active players
        for (int j = 0; j < _numPlayersPerTeam; j++)
        {
            team.playerList[j].SetActive(isActive);
        }

        // Modify teamdata
        team.isActive = isActive;
        return team;
    }

    private void Update()
    {
        if (_isRoundStarted)
        {
            ManageRoundTimer();
            ManageScoreTimer();
        }
    }

    
    private void ManageRoundTimer()
    {
        _roundTimer += Time.deltaTime;
        if(_roundTimer >= RoundDuration)
        {
            //Reset timer
            _roundTimer = 0;
            //Reset score timer (for consistency between teams)
            _scoreTimer = 0;
            //Increment round counter
            _currentRound++;
            //Initialize next round
            InitializeRound();

            // End game if currentRound reaches max rounds
        }
    }

    private void ManageScoreTimer()
    {
        _scoreTimer += Time.deltaTime;
        if (_scoreTimer >= _scoreFrequency)
        {
            //Reset timer
            _scoreTimer = 0;
            IncrementScore();
        }
    }

    private void IncrementScore()
    {
        for (int i = 0; i < _numberTeams; i++)
        {
            TeamData currentTeam = Teams[(TeamList)i];

            if (currentTeam.isActive)
            {
                //Debug.Log("update score");
                foreach(Machine machine in currentTeam.machineList)
                {
                    //Debug.Log("Adjusted heat: " + machine.adjustedHeat);
                    switch (machine.adjustedHeat)
                    {
                        // Safe state
                        case 0:
                            Debug.Log("+4");
                            currentTeam.score += 4;
                            break;
                        // Hot state 1 / Cold state 1
                        case 1:
                        case -1:
                            Debug.Log("+2");
                            currentTeam.score += 2;
                            break;
                        // Hot state 2 / Cold state 2
                        case 2:
                        case -2:
                            Debug.Log("+1");
                            currentTeam.score += 1;
                            break;
                    }
                    
                }
                
                Debug.Log("New score: " + currentTeam.score);

                // Update teamdata
                Teams[(TeamList)i] = currentTeam;
            }
        }
    }
}
