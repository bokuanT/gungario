using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkObject playerPrefab;

    private NetworkRunner networkRunner;
    
    // links player to their player object
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public Player Get(PlayerRef playerRef)
    {
        //NetworkObject obj = _spawnedCharacters[playerRef];
        //foreach (PlayerRef p in _spawnedCharacters.Keys)
        //{
        //    Debug.Log(p);
        //}
        //return obj.gameObject.GetComponent<Player>();
        NetworkObject obj = networkRunner.GetPlayerObject(playerRef);
        Player player = obj.gameObject.GetComponent<Player>();
        return player;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    { 
     
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
    {
        networkRunner = runner;
        // on other player joined and current player is server 
        if (runner.IsServer) 
        {
            Debug.Log("We are the server. Spawning player");

        } else {
            Debug.Log("Not the host. Joining");
        }

        // function to spawn a player, with random position 
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, playerRef, InitNetworkState);
        void InitNetworkState(NetworkRunner runner, NetworkObject networkObject)
        {
            Player player = networkObject.gameObject.GetComponent<Player>();
            Debug.Log($"Initializing player {player}");
            player.InitNetworkState(playerRef);
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

        Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft(NetworkRunner runner,  PlayerRef player)
    {
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

        GameObject scoreboardObject = GameObject.Find("Scoreboard_canvas/Scoreboard");
        Scoreboard scoreboard = scoreboardObject.GetComponent<Scoreboard>();
        scoreboard.OnPlayerLeft(player);

        Debug.Log("OnPlayerLeft");
    }

        // handles successful connections to server/host
    public void OnConnectedToServer(NetworkRunner runner)
    {
        networkRunner = runner;
        Debug.Log("OnConnectedToServer");
    }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) 
    { 
        Debug.Log("OnShutdown");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] byteArr) 
    {
        Debug.Log("OnConnectRequest");
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress address, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }

    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }


    // These functions are just here to satisfy the Interface. 
    // To be used in future updates perhaps
   
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> list) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> dict) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> array) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }

}
