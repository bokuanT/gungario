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
    private PlayerProfileModel _playerProfile;


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
	public NetworkRunner getRunner()
    {
        return runner;
    }
    public async void joinLobby() 
    {
        Debug.Log("Connecting");
        await _lobbyManager.JoinLobby(runner);
    }

    public void matchmakeDeathMatch()
    {
        _gameLauncher.matchmakeDeathMatch();
    }

    public void setPlayerProfile(PlayerProfileModel ppm)
    {
        _playerProfile = ppm;
        _gameLauncher.setPlayerProfile(ppm);
    }
}
