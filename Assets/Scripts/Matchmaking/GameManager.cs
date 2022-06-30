using System.Collections.Generic;
using UnityEngine;
using Fusion;
using PlayFab.ClientModels;
using Fusion.Photon.Realtime;

public class GameManager : NetworkBehaviour
{
    public GameObject networkRunnerPrefab;
    private NetworkRunner runner;
    private LobbyManager _lobbyManager;
    private GameLauncher _gameLauncher;
    private PlayerProfileModel playerProfile;
    private int activeScene;
    public static GameManager _instance;
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
        if (_instance == null) _instance = this;
        if (_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);


		GameObject go = Instantiate(networkRunnerPrefab);
		DontDestroyOnLoad(go);
        go.name = "Session";
		runner = go.GetComponent<NetworkRunner>();
        _gameLauncher = go.GetComponent<GameLauncher>();
        runner.AddCallbacks(_gameLauncher);
        _lobbyManager = GetComponent<LobbyManager>();
    }

    public async void joinLobby() 
    {
        Debug.Log("Connecting");
        await _lobbyManager.JoinLobby(runner);
    }

    public void MatchmakeDeathMatch()
    {
        _gameLauncher.MatchmakeDeathMatch();
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
        runner.AuthenticationValues.UserId = ppm.DisplayName;
    }

    public PlayerProfileModel GetPlayerProfile()
    {
        return playerProfile;
    }
}
