using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    CharacterInputHandler characterInputHandler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // on other player joined and current player is server 
        if (runner.IsServer) 
        {
            Debug.Log("OnPlayerJoined we are server. Spawning player");
            // function to spawn a player, with random position 
            runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);
        }
        Debug.Log("OnPlayerJoined");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    { 
        if (characterInputHandler == null && NetworkPlayer.Local != null)
        {
            characterInputHandler = NetworkPlayer.Local
                .GetComponent<CharacterInputHandler>();
        }
        if (characterInputHandler != null)
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }
    }

    public void OnPlayerLeft(NetworkRunner runner,  PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
    }

        // handles successful connections to server/host
    public void OnConnectedToServer(NetworkRunner runner)
    {
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
