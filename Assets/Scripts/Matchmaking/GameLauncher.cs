using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionStatus
{
	Disconnected,
	InLobby,
	Connecting,
	Failed,
	Connected
}

[RequireComponent(typeof(LevelManager))]
public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
	// [SerializeField] private GameManager _gameManagerPrefab;

	public static ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;
	[SerializeField] private PlayerInfo _playerInfoPrefab;
	[SerializeField] private PlayerInfoManager _playerInfoManagerPrefab;
	private PlayerInfoManager _playerInfoManager;
	public NetworkObject playerPrefab;
	private PlayerProfileModel _playerProfile { get; set; }
	private GameMode _gameMode;
	private NetworkRunner _runner;
	private List<SessionInfo> _sessionList;
	public int sessionCount; 
	private int MAX_PLAYERS = 2;
	//private int LocalPlayerRef;
	private static GameLauncher _instance;

	//For control point/dm
	public int gamemode;

	public static GameLauncher Instance
    {
        get
        {
			if (_instance == null) _instance = FindObjectOfType<GameLauncher>();
			return _instance;
		}
    }

	// initial dictionary for spawning at the start of games and maintaining playerinfo
	private Dictionary<PlayerRef, PlayerInfo> _players = new Dictionary<PlayerRef, PlayerInfo>();

    // links player to their player object
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

	void Awake() 
	{ 
		_runner = GetComponent<NetworkRunner>();
	}

	// updates dictionary when playerInfo gameObject is spawned 
	// saves the gameObject reference such that after the RPC_SetName updates it, we can access the update
	public void SetPlayerInfo(PlayerRef playerRef, PlayerInfo playerInfo)
	{
		_players[playerRef] = playerInfo;
		playerInfo.transform.SetParent(_runner.transform);
	}
	
	// retrieves playerInfo from dictionary
	public PlayerInfo GetPlayerInfo(PlayerRef ply)
	{
		if (!_runner)
		{
			Debug.Log("Returning null");
			return null;
		}
		_players.TryGetValue(ply, out PlayerInfo playerInfo);
		return playerInfo;
	}

    public PlayerInfoManager PlayerInfoManager
	{
		get => _playerInfoManager;
		set { _playerInfoManager = value; _playerInfoManager.transform.SetParent(_runner.transform); }
	}

	public Player Get(PlayerRef playerRef)
	{
		//NetworkObject obj = _spawnedCharacters[playerRef];
		//foreach (PlayerRef p in _spawnedCharacters.Keys)
		//{
		//    Debug.Log(p);
		//}
		//return obj.gameObject.GetComponent<Player>();
		NetworkObject obj = _runner.GetPlayerObject(playerRef);
		
		// enemy attacker case
		if (obj == null)
        {
			return null;
        }

		Player player = obj.gameObject.GetComponent<Player>();
		return player;
	}

	public void SetPlayerProfile(PlayerProfileModel ppm)
    {
		_playerProfile = ppm;
    }

	public async void MatchmakeControlPoint()
	{
		Debug.Log("Matchmaking");
		gamemode = 1;
		// check for any existing sessions
		foreach (var session in _sessionList)
		{
			if (session.PlayerCount < session.MaxPlayers)
			{
				SetJoinLobby();
				Debug.Log($"Joining {session.Name}");

				// This call will make Fusion join the first session as a Client
				var result = await _runner.StartGame(new StartGameArgs()
				{
					GameMode = _gameMode, // Client GameMode
					SessionName = session.Name, // Session to Join
					SceneObjectProvider = LevelManager.Instance, // Scene Provider
					DisableClientSessionCreation = true, // Make sure the client will never create a Session
					AuthValues = _runner.AuthenticationValues,
				});

				if (result.Ok)
				{
					// all good
					Debug.Log("Session joined successfully");
					Debug.Log($"Players: {session.PlayerCount}/{session.MaxPlayers}");
				}
				else
				{
					Debug.LogError($"Failed to Start: {result.ShutdownReason}");
				}
				return;
			}
		}
	}

		public async void MatchmakeDeathMatch() 
	{
		Debug.Log("Matchmaking");
		gamemode = 2;
		// check for any existing sessions
		foreach (var session in _sessionList) 
		{
			if (session.PlayerCount < session.MaxPlayers) 
			{
				SetJoinLobby();
				Debug.Log($"Joining {session.Name}");

				// This call will make Fusion join the first session as a Client
				var result = await _runner.StartGame(new StartGameArgs() {
					GameMode = _gameMode, // Client GameMode
					SessionName = session.Name, // Session to Join
					SceneObjectProvider = LevelManager.Instance, // Scene Provider
					DisableClientSessionCreation = true, // Make sure the client will never create a Session
					AuthValues = _runner.AuthenticationValues,
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
		
		// no sessions exist, start own session as host
		
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
			AuthValues = _runner.AuthenticationValues,
		});
		
    }

	public async void StartGame()
    {
        Debug.Log("Starting Practice Map");

		SetSingleLobby();

		await _runner.StartGame(new StartGameArgs 
        {
            GameMode = _gameMode, 
            SessionName = "TestRoom",
            SceneObjectProvider = LevelManager.Instance, 
        });
    }

	public void SetSingleLobby() => _gameMode = GameMode.Single; 
	public void SetCreateLobby() => _gameMode = GameMode.Host;
	public void SetJoinLobby() => _gameMode = GameMode.Client;
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log($"Player {player} Joined!");
		if (runner.IsServer || runner.Topology == SimulationConfig.Topologies.Shared && player == runner.LocalPlayer)
		{
			Debug.Log("Spawning player");
			//LocalPlayerRef = player;
			runner.Spawn(_playerInfoPrefab, Vector3.zero, Quaternion.identity, player);
		}

		CheckSessions();
		SetConnectionStatus(ConnectionStatus.Connected);
	}

	public void CheckSessions()
	{
		// _players is a local dictionary, and only added to when a player joins the session.
		if (_players.Count == MAX_PLAYERS) {
			Debug.Log("LOADING DEATHMATCH");
			LevelManager.LoadMap(LevelManager.MAP1_SCENE);
		} else if (_gameMode == GameMode.Single){
			Debug.Log("LOADING PRACTICEMAP");
			LevelManager.LoadMap(LevelManager.TESTGAME_SCENE);
		}
		
	}

	// called by external script, once map done loading
	public void SpawnPlayers() {
		int i = 0;
		foreach (var player in _players)
		{
			Player.Team tim = gamemode == 0 ? Player.Team.None
				: i % 2 == 0 ? Player.Team.Red
					: Player.Team.Blue;
			SpawnPlayer(_runner, player.Key, tim);
			i++;
		}

	}

	public void SpawnPlayer(NetworkRunner runner, PlayerRef playerRef, Player.Team tim)
	{
		NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, playerRef, InitNetworkState);
		void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
		{
			Player player = networkObject.gameObject.GetComponent<Player>();
			Debug.Log($"Initializing player {player}");
			player.InitNetworkState(playerRef, tim);
		}
		//_spawnedCharacters.Add(playerRef, networkPlayerObject);
        runner.SetPlayerObject(playerRef, networkPlayerObject);
        //Hopefully this syncs the dict across all clients
        IEnumerable<PlayerRef> activePlayers = runner.ActivePlayers;
        foreach (PlayerRef pr in activePlayers)
        {
            NetworkObject player = runner.GetPlayerObject(pr);

            if (!_spawnedCharacters.ContainsKey(pr))
                _spawnedCharacters.Add(pr, player);
        }
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		Debug.Log($"{player.PlayerId} disconnected.");

		// Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            Player playerGameObject = networkObject.gameObject.GetComponent<Player>();
            //Debug.Log("playerg.o. " + playerGameObject);
            if (playerGameObject.scoreboard_item != null)
            {
                //Debug.Log("Destroying: " + playerGameObject.scoreboard_item);
                Destroy(playerGameObject.scoreboard_item.gameObject);
            }
            // takes in a NetworkObject
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            Debug.Log("Player removed successfully");
        }

		// Find and remove the players info gameobject
		if (_players.TryGetValue(player, out PlayerInfo playerInfo))
		{
			_runner.Despawn(playerInfo.Object);
			_players.Remove(player);
			Debug.Log("PlayerInfo removed successfully");
		}

		GameObject scoreboardObject = GameObject.Find("Scoreboard_canvas/Scoreboard");
        Scoreboard scoreboard = scoreboardObject.GetComponent<Scoreboard>();
        scoreboard.OnPlayerLeft(player);

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
        {
			//Debug.Log(_runner.LocalPlayer);
			//PlayerRef player = _gameMode == GameMode.Host ? 0 : LocalPlayerRef;
			//// if host is leaving session, his playerRef is always 0, but _runner.LocalPlayer will return MAX_PLAYERS - 1
			//_runner.Disconnect(player);

			//// Find and remove the players info gameobject
			//if (_players.TryGetValue(player, out PlayerInfo playerInfo))
			//{
			//	_runner.Despawn(playerInfo.Object);
			//	_players.Remove(player);
			//	Debug.Log("PlayerInfo removed successfully");
			//}
			Debug.Log("Back to Lobby from exiting matchmaking is a work-in-progress. To be implemented.");
			_runner.Shutdown();
		}
	}

	// Quits game
	public void LeaveGame()
	{
		if (_runner != null)
		{
			_runner.Shutdown();
			SetConnectionStatus(ConnectionStatus.Disconnected);
		}
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		Debug.Log("Connected to server");
		SetConnectionStatus(ConnectionStatus.Connected);
	}
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		Debug.Log("Disconnected from server");
		LeaveGame();
		SetConnectionStatus(ConnectionStatus.Disconnected);
	}
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
		if (runner.CurrentScene > 0)
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
		LeaveGame();
		SetConnectionStatus(ConnectionStatus.Disconnected);
		(string status, string message) = ConnectFailedReasonToHuman(reason);
		//_disconnectUI.ShowMessage(status,message);
	}

	public void OnInput(NetworkRunner runner, NetworkInput input) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
	{ 
		Debug.Log($"Session List Updated with {sessionList.Count} session(s)");
		_sessionList = sessionList;
		sessionCount = sessionList.Count;
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