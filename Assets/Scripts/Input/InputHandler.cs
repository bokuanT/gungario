using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class InputHandler : NetworkBehaviour, INetworkRunnerCallbacks
{
    Vector2 moveInputVector;
    Vector2 mouseInputVector;
    NetworkInputData networkInputData = new NetworkInputData();
    public Camera cam;
    NetworkCharacterControllerPrototypeCustom controller;
    NetworkPlayer player;

    /// <summary>
    /// Hook up to the Fusion callbacks so we can handle the input polling
    /// </summary>
    public override void Spawned()
    {
        // Technically, it does not really matter which InputController fills the input structure, since the actual data will only be sent to the one that does have authority,
        // but in the name of clarity, let's make sure we give input control to the gameobject that also has Input authority.
        if (Object.HasInputAuthority)
        {
            Runner.AddCallbacks(this);
        }

    }
    
    private void Awake() 
    {
        controller = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.enabled = true;
        //cam = GameObject.FindWithTag("PlayerCamera");

        moveInputVector.x = Input.GetAxisRaw("Horizontal");
        moveInputVector.y = Input.GetAxisRaw("Vertical");

        // Gets the mouse position
        Vector3 mousePos =  cam.ScreenToWorldPoint(Input.mousePosition);
		mouseInputVector = new Vector2(mousePos.x,mousePos.y );
    }

    
    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            //Move
            Vector2 moveDirection = networkInputData.movementInput;
            
            moveDirection.Normalize();
        
            controller.Move(moveDirection);

            controller.SetDirections(networkInputData.mouseInput);

        }

            //Rotate character
            // Vector2 mouseDirection = networkInputData.mouseInput;
            // int direction = Utils.getDirection(controller.transform.position, mouseDirection);
            // Debug.Log("Direction is " + direction);
            // setDirections(direction);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        networkInputData.movementInput = moveInputVector;
        networkInputData.mouseInput = mouseInputVector;

        input.Set(networkInputData);
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
