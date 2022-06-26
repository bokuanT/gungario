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


    void Awake() 
    {
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

    public PlayerProfileModel getPlayerProfile()
    {
        return playerProfile;
    }
}
