using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    [Header("Game Rewards")]
    [SerializeField] private int WinnerRewards;
    [SerializeField] private int LoserRewards;

    public GameObject networkRunnerPrefab;
    private NetworkRunner runner;
    private LobbyManager _lobbyManager;
    private GameLauncher _gameLauncher;
    private PlayerProfileModel playerProfile;
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    void Awake() 
    {
        if (_instance == null)
        {
            _instance = this;
            SpawnRunner();
        } 
        if (_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Debug.Log("GAMEMANAGER ONSTART");
    }

    // once runner is disconnected, it can be spawned using this method
    public void SpawnRunner()
    {
        Debug.Log("SPAWNING RUNNER");
        GameObject go = Instantiate(networkRunnerPrefab);
        DontDestroyOnLoad(go);
        go.name = "Session";
        runner = go.GetComponent<NetworkRunner>();
        _gameLauncher = go.GetComponent<GameLauncher>();
        runner.AddCallbacks(_gameLauncher);
        _lobbyManager = GetComponent<LobbyManager>();
    }


    public async Task JoinLobby() 
    {
        Debug.Log("Connecting");
        await _lobbyManager.JoinLobby(runner);
    }

    public void MatchmakeDeathMatch()
    {
        GameLauncher.Instance.MatchmakeDeathMatch();
    }

    public void MatchmakeControlPoint()
    {
        GameLauncher.Instance.MatchmakeControlPoint();
    }

    public void MatchmakeFFA()
    {
        GameLauncher.Instance.MatchmakeFFA();
    }

    public void StartGame()
    {
        GameLauncher.Instance.StartGame();
    }

    public void SetPlayerProfile(PlayerProfileModel ppm)
    {
        playerProfile = ppm;
        GameLauncher.Instance.SetPlayerProfile(ppm);
    }

    public PlayerProfileModel GetPlayerProfile()
    {
        return playerProfile;
    }

    // DOESNT WORK
    // returns the number of players in lobby/session
    public int GetActivePlayers()
    {
        // ONLY AVAILABLE IN SESSION
        IEnumerable<PlayerRef> players = runner.ActivePlayers;
        int count = 0;
        foreach (PlayerRef player in players)
        {
            count++;
        }
        return count;
    }
    
    // assigns players rewards based off position
    public void AssignRewards(int position)
    {
        Shop.Instance.AddCurrency(WinnerRewards - LoserRewards * (position - 1));
        ExperienceBar.Instance.AddExperience(WinnerRewards - LoserRewards * (position - 1));
    }

    // assigns players rewards based off winning team
    public void AssignRewards(Player.Team winningTeam, Player winner)
    {
        if (winningTeam.Equals(winner.team)) // player belongs to winning team
        {
            Shop.Instance.AddCurrency(WinnerRewards);
            ExperienceBar.Instance.AddExperience(WinnerRewards);
        }
        else if (winningTeam.Equals(Player.Team.None)) // game was a draw
        {
            // do nothing 
        } else  // player belongs to losing team
        {
            Shop.Instance.AddCurrency(LoserRewards);
            ExperienceBar.Instance.AddExperience(LoserRewards);
        }
    }

    // handle failed connection/cancel matchmake/quitting game here
    // coroutine here is done so this is done in tandem with NetworkRunner being actively shutdown . 
    public void ReturnToLobby()
    {
        StartCoroutine(EnterLobby());
    }
    IEnumerator EnterLobby()
    {
        // changes scene if necessary
        // case 1: cancelling matchmaking, no need to change scene
        // case 2: exiting game, need to change scene 
        if (SceneManager.GetActiveScene().buildIndex != LevelManager.MENU_SCENE) SceneManager.LoadScene(LevelManager.MENU_SCENE);

        // waits til scene has changed 
        while (SceneManager.GetActiveScene().buildIndex != LevelManager.MENU_SCENE)
        {
            Debug.Log("scene not changed");
            yield return null;
        }

        runner = null;

        // spawn a new runner via GameManager
        SpawnRunner();

        // waits til runner is instantiated
        while (_gameLauncher != GameLauncher.Instance)
        {
            Debug.Log("runner is null");
            yield return null;
        }

        // re-enter lobby
        // JoinLobby();
        MenuUI.Instance.OnJoinLobby();
    }
}
