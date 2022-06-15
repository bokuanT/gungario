using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    private Action<List<SessionInfo>> _onSessionListUpdated;
    public LobbyManager lobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = new GameObject("Session");
		DontDestroyOnLoad(go);

		_runner = go.AddComponent<NetworkRunner>();
        _runner.AddCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public NetworkRunner getRunner() {
        return _runner;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");

    }

    public async void joinLobby() {
        Debug.Log("Connecting");
        await lobbyManager.JoinLobby(_runner);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
