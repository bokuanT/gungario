using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class CharacterInputHandler : NetworkBehaviour, INetworkRunnerCallbacks
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 mouseInputVector = Vector2.zero;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
<<<<<<< HEAD
        Camera cam = gameObject.GetComponentInChildren<Camera>();
        cam.transform.position += new Vector3(0,0,-10);
=======
        cam = gameObject.GetComponentInChildren<Camera>();
        // cam.transform.position += new Vector3(0,0,-10);
>>>>>>> 7337714e0eb8c3dbfe5057efca0b5d042fb8b485
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxisRaw("Horizontal");
        moveInputVector.y = Input.GetAxisRaw("Vertical");

        // Gets the mouse position
        // somehow redundant
        mouseInputVector = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.movementInput = moveInputVector;
        networkInputData.mouseInput = mouseInputVector;

        return networkInputData;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
            input.Set(GetNetworkInput());
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    
}
