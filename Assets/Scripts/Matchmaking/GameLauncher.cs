using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
	Disconnected,
	Connecting,
	Failed,
	Connected
}

[RequireComponent(typeof(LevelManager))]
public class GameLauncher : NetworkBehaviour, INetworkRunnerCallbacks
{
	// [SerializeField] private GameManager _gameManagerPrefab;

	public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

	public NetworkObject playerPrefab;
	private GameMode _gameMode;
	private NetworkRunner _runner;
	private GameManager _gameManager;
	private LobbyManager _lobbyManager;
	private List<SessionInfo> _sessionList;
	private String lobbyName;
	private int MAX_PLAYERS = 2;

	// initial list for spawning at the start of games
	public static List<PlayerRef> SessionPlayers = new List<PlayerRef>();

	// links player to their player object
    public static Dictionary<PlayerRef, NetworkObject> Players = new Dictionary<PlayerRef, NetworkObject>();

    //For Player.cs :: ApplyDamage -> maybe can try replace this with better soln
    private static Dictionary<PlayerRef, NetworkObject> _staticSpawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    //For Player::ApplyDamage
    public static Player Get(PlayerRef playerRef)
    {
        NetworkObject value = _staticSpawnedCharacters[playerRef];
        return value.gameObject.GetComponent<Player>();
    }

	void Awake() {
		// Application.runInBackground = true;

		_gameManager = GetComponent<GameManager>();
		_lobbyManager = GetComponent<LobbyManager>();
	}

	public async void matchmakeDeathMatch() 
	{
		_runner = _gameManager.getRunner();
		Debug.Log("Matchmaking");

		if (_sessionList != null && _sessionList.Count > 0) // Other Sessions exist
		{
			foreach (var session in _sessionList) {
				if (session.PlayerCount < session.MaxPlayers) {
					SetJoinLobby();
					Debug.Log($"Joining {session.Name}");

					// This call will make Fusion join the first session as a Client
					var result = await _runner.StartGame(new StartGameArgs() {
						GameMode = _gameMode, // Client GameMode
						SessionName = session.Name, // Session to Join
						SceneObjectProvider = LevelManager.Instance, // Scene Provider
						DisableClientSessionCreation = true, // Make sure the client will never create a Session
					});

					if (result.Ok) {
						// all good
						Debug.Log("Session joined successfully");
						Debug.Log($"Players: {session.PlayerCount}/{session.MaxPlayers}");
					} else {
						Debug.LogError($"Failed to Start: {result.ShutdownReason}");
					}
					return;
				} 
			}
		} 
		else 	// no sessions exist, start own session as host
		{
			Debug.Log("No Session Found");
			Debug.Log("Creating Session");
			SetCreateLobby();
			int sessionNumber = (int) (UnityEngine.Random.value * 100);

			// This call will make Fusion create the first session as a host
			await _runner.StartGame(new StartGameArgs() {
				GameMode = _gameMode, // Host GameMode
				SessionName = "Deathmatch" + sessionNumber, // Session to Join
				SceneObjectProvider = LevelManager.Instance, // Scene Provider
				PlayerCount = MAX_PLAYERS,
			});
		}


    }

	public void SetCreateLobby() => _gameMode = GameMode.Host;
	public void SetJoinLobby() => _gameMode = GameMode.Client;
	private void SetConnectionStatus(ConnectionStatus status)
	{
		Debug.Log($"Setting connection status to {status}");

		ConnectionStatus = status;
		
		if (!Application.isPlaying) 
			return;

		if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
		{
			SceneManager.LoadScene(LevelManager.MENU_SCENE);
		}
	}
	public void LeaveSession()
	{
		if (_runner != null)
			_runner.Shutdown();
		else
			SetConnectionStatus(ConnectionStatus.Disconnected);
	}
	public void OnConnectedToServer(NetworkRunner runner)
	{
		Debug.Log("Connected to server");
		SetConnectionStatus(ConnectionStatus.Connected);
	}
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		Debug.Log("Disconnected from server");
		LeaveSession();
		SetConnectionStatus(ConnectionStatus.Disconnected);
	}
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		if (runner.CurrentScene>0)
		{
			Debug.LogWarning($"Refused connection requested by {request.RemoteAddress}");
			request.Refuse();
		}
		else
			request.Accept();
	}
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		Debug.Log($"Connect failed {reason}");
		LeaveSession();
		SetConnectionStatus(ConnectionStatus.Failed);
		(string status, string message) = ConnectFailedReasonToHuman(reason);
		//_disconnectUI.ShowMessage(status,message);
	}
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log($"Player {player} Joined!");
		SessionPlayers.Add(player);
		CheckSessions();
		SetConnectionStatus(ConnectionStatus.Connected);
	}

	public void CheckSessions()
	{
		if (SessionPlayers.Count == MAX_PLAYERS) {
			Debug.Log("LOADING DEATHMATCH");
			LevelManager.LoadDeathmatch();
		} 
		
	}

	// called by external script, once map done loading
	public void SpawnPlayers() {
		foreach (var player in SessionPlayers)
		{
			SpawnPlayer(_runner, player);
		}
	}

	public void SpawnPlayer(NetworkRunner runner, PlayerRef player)
	{
		if (runner.IsServer)
		{
			// singular game manager
			// if(_gameMode==GameMode.Host)
			// 	runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);

			NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player, InitNetworkState);
			void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
			{
				Player player = networkObject.gameObject.GetComponent<Player>();
				Debug.Log($"Initializing player {player}");
				//player.InitNetworkState();
			}
			Players.Add(player, networkPlayerObject);
			_staticSpawnedCharacters.Add(player, networkPlayerObject);
		}
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log($"{player.PlayerId} disconnected.");
		// attempt to destroy player object
		NetworkObject temp;
		Players.TryGetValue(player, out temp);
		Destroy(temp);
		Players.Remove(player);

		SetConnectionStatus(ConnectionStatus);
	}
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		Debug.Log($"OnShutdown {shutdownReason}");
		SetConnectionStatus(ConnectionStatus.Disconnected);

		(string status, string message) = ShutdownReasonToHuman(shutdownReason);
		// _disconnectUI.ShowMessage( status, message);

		// RoomPlayer.Players.Clear();

		if(_runner)
			Destroy(_runner.gameObject);
		
		// Reset the object pools
		// _pool.ClearPools();
		// _pool = null;

		_runner = null;
	}
	public void OnInput(NetworkRunner runner, NetworkInput input) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
	{ 
		Debug.Log($"Session List Updated with {sessionList.Count} session(s)");
		_sessionList = sessionList;
        foreach (var session in sessionList) 
        {
            Debug.Log($"{session.Name} Players: {session.PlayerCount}/{session.MaxPlayers}");
        }
	}
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
	public void OnSceneLoadDone(NetworkRunner runner) { }
	public void OnSceneLoadStart(NetworkRunner runner) { }

	private static (string, string) ShutdownReasonToHuman(ShutdownReason reason)
	{
		switch (reason)
		{
			case ShutdownReason.Ok:
				return (null, null);
			case ShutdownReason.Error:
				return ("Error", "Shutdown was caused by some internal error");
			case ShutdownReason.IncompatibleConfiguration:
				return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
			case ShutdownReason.ServerInRoom:
				return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
			case ShutdownReason.DisconnectedByPluginLogic:
				return ("Disconnected By Plugin Logic", "You were kicked, the room may have been closed");
			case ShutdownReason.GameClosed:
				return ("Game Closed", "The session cannot be joined, the game is closed");
			case ShutdownReason.GameNotFound:
				return ("Game Not Found", "This room does not exist");
			case ShutdownReason.MaxCcuReached:
				return ("Max Players", "The Max CCU has been reached, please try again later");
			case ShutdownReason.InvalidRegion:
				return ("Invalid Region", "The currently selected region is invalid");
			case ShutdownReason.GameIdAlreadyExists:
				return ("ID already exists", "A room with this name has already been created");
			case ShutdownReason.GameIsFull:
				return ("Game is full", "This lobby is full!");
			case ShutdownReason.InvalidAuthentication:
				return ("Invalid Authentication", "The Authentication values are invalid");
			case ShutdownReason.CustomAuthenticationFailed:
				return ("Authentication Failed", "Custom authentication has failed");
			case ShutdownReason.AuthenticationTicketExpired:
				return ("Authentication Expired", "The authentication ticket has expired");
			case ShutdownReason.PhotonCloudTimeout:
				return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
			default:
				Debug.LogWarning($"Unknown ShutdownReason {reason}");
				return ("Unknown Shutdown Reason", $"{(int)reason}");
		}
	}

	private static (string,string) ConnectFailedReasonToHuman(NetConnectFailedReason reason)
	{
		switch (reason)
		{
			case NetConnectFailedReason.Timeout:
				return ("Timed Out", "");
			case NetConnectFailedReason.ServerRefused:
				return ("Connection Refused", "The lobby may be currently in-game");
			case NetConnectFailedReason.ServerFull:
				return ("Server Full", "");
			default:
				Debug.LogWarning($"Unknown NetConnectFailedReason {reason}");
				return ("Unknown Connection Failure", $"{(int)reason}");
		}
	}
}