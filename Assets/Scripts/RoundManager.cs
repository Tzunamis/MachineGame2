using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    private static float _roundDuration; // How long is a round?
    private static float _roundTimer = 0; // How far have we progressed into the current round?
    public bool isRoundStarted = false;
    public bool gameOver = false;
    private int _numRounds;
    private int _currentRound = 1;
    private float _scoreFrequency; // Poorly named. This is how often points are scored by the active team(s)
    private float _scoreTimer = 0;
    public GameObject camera;

    //Hat color try
    private SpriteRenderer _hatColor;
    private Color _hatColorPlayer0;
    private Color _hatColorPlayer1;
    private Color _hatColorPlayer2;
    private Color _hatColorPlayer3;


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

    [SerializeField] private InputActionReference interact;

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
    private bool _spawningPlayers = false;

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

    // UI stuff
    public Text scoreText;
    public Text timerText;
    public Canvas pauseCanvas;
    public Text pauseText;

    // Player sprites
    private Animator animator;

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
        // Create teamdata dictionary
        Teams = new Dictionary<TeamList, TeamData>();

        // Fetch variables from control script
        _numberTeams = controlScript.numTeams;
        _numPlayersPerTeam = controlScript.numPlayersPerTeam;
        _numMachinesPerTeam = controlScript.numMachinesPerTeam;
        _isSimultaneous = controlScript.isSimultaneous;
        _scoreFrequency = controlScript.scoreFrequency;
        _roundDuration = controlScript.roundDuration;

        if(_isSimultaneous)
        {
            _numRounds = controlScript.numOfRoundsPerTeam;
        }
        else
        {
            _numRounds = _numberTeams * controlScript.numOfRoundsPerTeam;
        }

        // Disable "game over" canvas
        pauseCanvas.enabled = false;

        // Initialize teamdata
        InitializeTeams();

        // Display initial UI
        UpdateScoreUI();
        UpdateTimerUI();

        // Set hat colours
        // Setting red
         _hatColorPlayer0 = new Color(0.6078432f, 0.2313726f, 0.1333333f, 1f);
        //Setting yellow
        _hatColorPlayer1 = new Color(0.854902f, 0.6901961f, 0f, 1f);
        //Setting blue
        _hatColorPlayer2 = new Color(0.2039216f, 0.2509804f, 0.4705882f, 1f);
        //Setting green
        _hatColorPlayer3 = new Color(0.4941176f, 0.5333334f, 0.145098f, 1f);


        // Manage players
        if (_spawningPlayers)
        {
            // Not quite working. Input system makes it very hard to rebind controls at runtime
            SpawnPlayers();
        }
        else
        {
            // This is an alternate function that accomplishes the same thing as SpawnPlayers with preexisting players in the scene (they need to be children of the spawners)
            RegisterPlayers();
        }

        // Called at the start of each round
        InitializeRound();

        
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

                // Set all machines active
                machineToBeStored.isActive = true;
                // Set machine inactive if needed
                if(!_isSimultaneous && machineToBeStored.teamID != TeamList.Team1)
                {
                    machineToBeStored.isActive = false;
                }
            }
        }

       
        for (int i = 0; i < _numberTeams; i++)
        {

            // Check if teams play simultaneously to determine which teams start the game active
            bool startsActive = false;

            if (_isSimultaneous || i == 0)
            {
                startsActive = true;
            }

            GameObject[] playersOnThisTeam = new GameObject[_numPlayersPerTeam];

            if(_spawningPlayers)
            {
                //Create player gameobject and place them in a gameobject array to be placed
                for (int j = 0; j < _numPlayersPerTeam; j++)
                {
                    GameObject playerToInstantiate = playerPrefab;
                    playersOnThisTeam[j] = playerToInstantiate;
                }

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

            // Set controls
            //InputAction[] currentPlayerControls = SetPlayerControls(currentTeam.playerIDToSpawn);

            Player playerScript = playerToSpawn.GetComponent<Player>();

            switch (currentTeam.playerIDToSpawn)
            {
                case 0:
                    //    Debug.Log(playerScript.interact.bindings[0].effectivePath);
                    InputBinding inputbinding = playerScript.interact.bindings[0];
                    inputbinding.path = "<Keyboard>/enter";
                    playerScript.interact.ApplyBindingOverride(0, inputbinding);
               //     Debug.Log(playerScript.interact.bindings[0].effectivePath);
                    //playerScript.interact.
                    //playerScript.interact = interact.action;
                    //playerScript.GetComponent<Player>().interact.performed += playerScript.OnInteractionPerformed;
                    //playerToSpawn.GetComponent<PlayerMovement>().movement.ChangeBindingWithGroup("Key.W");
                    break;
                case 1:
                    playerScript.interact.ApplyBindingOverride(0, "<Keyboard>/tab");
                //    Debug.Log(playerScript.interact.bindings[0].effectivePath);
                    break;
                case 2:
                    playerScript.interact.ApplyBindingOverride(0, "<Keyboard>/enter");
              //      Debug.Log(playerScript.interact.bindings[0].effectivePath);
                    break;
                case 3:
                    break;
            }

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

    private void RegisterPlayers()
    {
        foreach (Spawner currentSpawner in spawners)
        {
            TeamData currentTeam = Teams[currentSpawner.teamSpawnType];

            GameObject playerToRegister = currentSpawner.transform.GetChild(0).gameObject;
            // Set player scale according to controlscript
            float playerScale = controlScript.playerScale;
            playerToRegister.transform.localScale = new Vector3(playerScale, playerScale, 1);

            // Set player active or inactive
            if (currentSpawner.teamSpawnType == TeamList.Team1 || _isSimultaneous)
            {
                playerToRegister.SetActive(true);
            }
            else
            {
                playerToRegister.SetActive(false);
            }

            // Set player sprite according to team
            animator = playerToRegister.GetComponent<Animator>();

            switch(currentSpawner.teamSpawnType)
            {
                case TeamList.Team2:
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Player2_Controller");
                    break;
                case TeamList.Team3:
                    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Player3_Controller");
                    break;
            }

            // Set player hat colour
            _hatColor = playerToRegister.transform.GetChild(2).GetComponent<SpriteRenderer>();

            switch (currentTeam.playerIDToSpawn)
            {
                case 0:
                    _hatColor.color = _hatColorPlayer0;
                    break;

                case 1:
                    _hatColor.color = _hatColorPlayer1;
                    break;

                case 2:
                    _hatColor.color = _hatColorPlayer2;
                    break;

                case 3:
                    _hatColor.color = _hatColorPlayer3;
                    break;
            }

            //Place player and update teamdata reference
            currentTeam.playerList[currentTeam.playerIDToSpawn] = playerToRegister;

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
        // Used for canvas message at end of function
        int nextActiveTeamID = -1;

        if(!_isSimultaneous && _currentRound != 1)
        {
            //Cycle active players and machines
            for (int i = 0; i < _numberTeams; i++)
            {
                if (Teams[(TeamList)i].isActive)
                {
                    Teams[(TeamList)i] = SetTeamActive(Teams[(TeamList)i], false);

                    // Determine next active team
                    if(i < _numberTeams - 1)
                    {
                        nextActiveTeamID = i + 1;
                    }
                    else
                    {
                        nextActiveTeamID = 0;
                    }

                    Teams[(TeamList)nextActiveTeamID] = SetTeamActive(Teams[(TeamList)nextActiveTeamID], true);


                    //End for loop
                    i = _numberTeams;
                }
            }

            // Rotate camera to next arena if needed
            if(camera != null)
            {
                camera.transform.RotateAround(Vector3.zero, Vector3.forward, 120);
            }

        }
        else if(!_isSimultaneous)
        {
            nextActiveTeamID = 0;
        }

        // Wait for player input to start round
        isRoundStarted = false;
        pauseCanvas.enabled = true;
        if(nextActiveTeamID == -1)
        {
            pauseText.text = "Players, get ready!\nPress Y to start.";
        }
        else
        {
            pauseText.text = "Round "+ _currentRound + "\nTeam " + (nextActiveTeamID + 1) + ", get ready!\nPress Y to start the round.";
        }
        
    }

    private TeamData SetTeamActive(TeamData team, bool isActive)
    {
        // Enable/disable active machines
        for (int i = 0; i < _numMachinesPerTeam; i++)
        {
            team.machineList[i].isActive = isActive;
        }

        // Enable/disable active players
        for (int i = 0; i < _numPlayersPerTeam; i++)
        {
            team.playerList[i].SetActive(isActive);
        }

        // Modify teamdata
        team.isActive = isActive;
        return team;
    }

    private void Update()
    {
        //if(!controlsBound)
        //{
        //    controlsBound = true;
        //    BindControls();
        //}

        if (isRoundStarted && !gameOver)
        {
            ManageRoundTimer();
            ManageScoreTimer();
        }
        else if (Input.GetKeyDown(KeyCode.Y) && !gameOver)
        {
            isRoundStarted = true;
            pauseCanvas.enabled = false;
        }
    }

    //private void BindControls()
    //{

    //    for(int i = 0; i < _numberTeams; i++)
    //    {

    //        for(int j = 0; j < _numPlayersPerTeam; j++)
    //        {
    //            // Set controls

    //            Player playerScript = Teams[(TeamList)i].playerList[j].GetComponent<Player>();

    //            switch (Teams[(TeamList)i].playerIDToSpawn)
    //            {
    //                case 0:
    //                    InputBinding inputbinding = playerScript.interact.bindings[0];
    //                    inputbinding.overridePath = "<Keyboard>/enter";
    //                    playerScript.interact.ApplyBindingOverride(0, inputbinding);
    //                    Debug.Log(playerScript.interact.bindings[0].effectivePath);
    //                    //playerScript.interact.
    //                    //playerScript.interact = interact.action;
    //                    //playerScript.GetComponent<Player>().interact.performed += playerScript.OnInteractionPerformed;
    //                    //playerToSpawn.GetComponent<PlayerMovement>().movement.ChangeBindingWithGroup("Key.W");
    //                    break;
    //                case 1:
    //                    playerScript.interact.ApplyBindingOverride(0, "<Keyboard>/tab");
    //                    Debug.Log(playerScript.interact.bindings[0].effectivePath);
    //                    break;
    //                case 2:
    //                    playerScript.interact.ApplyBindingOverride(0, "<Keyboard>/space");
    //                    Debug.Log(playerScript.interact.bindings[0].effectivePath);
    //                    break;
    //                case 3:
    //                    break;
    //            }

    //        }

    //    }

        

    //}
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

            if(_currentRound <= _numRounds)
            {
                //Initialize next round
                InitializeRound();
            }
            else
            {
                // End game if currentRound reaches max rounds
                EndGame();
            }


        }
        UpdateTimerUI();
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
        Debug.Log("Incrementing score");
        for (int i = 0; i < _numberTeams; i++)
        {
            TeamData currentTeam = Teams[(TeamList)i];

            if (currentTeam.isActive)
            {
                foreach(Machine machine in currentTeam.machineList)
                {
                    switch (machine.adjustedHeat)
                    {
                        // Safe state
                        case 0:
                            currentTeam.score += 4;
                            break;
                        // Hot state 1 / Cold state 1
                        case 1:
                        case -1:
                            currentTeam.score += 2;
                            break;
                        // Hot state 2 / Cold state 2
                        case 2:
                        case -2:
                            currentTeam.score += 1;
                            break;
                    }
                    
                }

                // Update teamdata
                Teams[(TeamList)i] = currentTeam;

                // Update score UI
                UpdateScoreUI();

            }
        }
    }

    private void UpdateScoreUI()
    {
        string newScoreText = ("Team 1: " + Teams[TeamList.Team1].score + "\nTeam 2: " + Teams[TeamList.Team2].score + "\nTeam 3: " + Teams[TeamList.Team3].score);
        if (_numberTeams == 4)
        {
            newScoreText += "\nTeam 4: " + Teams[TeamList.Team4].score;
        }
        scoreText.text = newScoreText;
    }

    private void UpdateTimerUI()
    {
        timerText.text = ("Time remaining: " + Mathf.Ceil(RoundDuration - RoundTimer));
    }

    private void EndGame()
    {
        isRoundStarted = false;
        gameOver = true;
        for( int i = 0; i < _numberTeams; i++)
        {
            SetTeamActive(Teams[(TeamList)i], false);
        }
        pauseCanvas.enabled = true;
        string gameOverText = "Game over!\n" + "Team 1: " + Teams[TeamList.Team1].score + " points \nTeam 2: " + Teams[TeamList.Team2].score + " points\nTeam 3: " + Teams[TeamList.Team3].score + " points\n";
        if(_numberTeams == 4)
        {
            gameOverText += "Team 4: " + Teams[TeamList.Team4].score + " points\n";
        }
        gameOverText += "Press escape to return to menu.";
        pauseText.text = gameOverText;
    }
}
