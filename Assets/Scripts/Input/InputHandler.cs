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
    public Player playerObject;
    private bool _primaryFire;
    private bool _scoreboard;

    [SerializeField] private AudioEmitter _playerMovementSound;

    [Networked]
    private TickTimer SoundCoolDownTimer { get; set; }

    private float DELAY = 1.5f;

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
        player = GetComponent<NetworkPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //cam.enabled = true;
        //cam = GameObject.FindWithTag("PlayerCamera");
        if (cam != null)
        {
            moveInputVector.x = Input.GetAxisRaw("Horizontal");
            moveInputVector.y = Input.GetAxisRaw("Vertical");

            // Gets the mouse position
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseInputVector = new Vector2(mousePos.x, mousePos.y);
        }
        if (Input.GetMouseButton(0) )
			_primaryFire = true;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _scoreboard = !_scoreboard;
        }
    }

    void FixedUpdate()
    {
        if (_scoreboard)
        {
            playerObject.ToggleOnScoreboard();
        }
        else
        {
            playerObject.ToggleOffScoreboard();
        }
    }
    
    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            //Move
            if (playerObject.state == Player.State.Active) //only move if alive
            {
                Vector2 moveDirection = networkInputData.movementInput;
                
                moveDirection.Normalize();
            
                controller.Move(moveDirection);

                if (SoundCoolDownTimer.ExpiredOrNotRunning(Runner))
                {
                    if (moveDirection != Vector2.zero)
                    {
                        _playerMovementSound.PlayOneShot();
                        SoundCoolDownTimer = TickTimer.CreateFromSeconds(Runner, DELAY);
                    }   
                }

                playerObject.setMouse(networkInputData.mouseInput);

                if (networkInputData.IsDown(NetworkInputData.MOUSEBUTTON1))
                {
                    playerObject.Shoot(moveDirection);
                }             

            } 
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        networkInputData.movementInput = moveInputVector;
        networkInputData.mouseInput = mouseInputVector;

        if ( _primaryFire )
        {
            _primaryFire = false;
            networkInputData.Buttons |= NetworkInputData.MOUSEBUTTON1;
        }

        input.Set(networkInputData);
        networkInputData.Buttons = 0;
        
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
