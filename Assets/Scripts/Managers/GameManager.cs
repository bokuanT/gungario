using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using PlayFab.ClientModels;
using System.Threading.Tasks;

public class GameManager : NetworkBehaviour
{
    public GameObject networkRunnerPrefab;
    private NetworkRunner runner;
    private LobbyManager _lobbyManager;
    private GameLauncher _gameLauncher;
    private PlayerProfileModel playerProfile;
    private int activeScene;
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
        // may return itself or the current instance
        _instance = GameManager.Instance;
        if (_instance == null) _instance = this;
        if (_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        SpawnRunner();
    }

    // once runner is disconnected, it can be spawned using this method
    public void SpawnRunner()
    {
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
        _gameLauncher.MatchmakeDeathMatch();
    }

    public void MatchmakeControlPoint()
    {
        _gameLauncher.MatchmakeControlPoint();
    }

    public void MatchmakeFFA()
    {
        _gameLauncher.MatchmakeFFA();
    }

    public void StartGame()
    {
        _gameLauncher.StartGame();
    }

    public void SetScene(int scene) 
    {
        activeScene = scene;
    }

    public int getScene()
    {
        return activeScene;
    }

    public void SetPlayerProfile(PlayerProfileModel ppm)
    {
        playerProfile = ppm;
        _gameLauncher.SetPlayerProfile(ppm);
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
}