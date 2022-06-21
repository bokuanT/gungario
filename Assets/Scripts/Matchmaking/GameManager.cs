using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    private NetworkRunner runner;
    private List<SessionInfo> _sessionList;
    private LobbyManager _lobbyManager;
    private GameLauncher _gameLauncher;

    void Awake() 
    {
        DontDestroyOnLoad(gameObject);

		GameObject go = new GameObject("Session");
		DontDestroyOnLoad(go);

		runner = go.AddComponent<NetworkRunner>();
        _gameLauncher = GetComponent<GameLauncher>();
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

}
