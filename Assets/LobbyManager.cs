using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    private bool lobby = false;
    public GameManager gameManager;
    private NetworkRunner _runner;

    // Start is called before the first frame update
    void Start()
    {
        _runner = gameManager.getRunner();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Utility method to Join the ClientServer Lobby
    public async Task JoinLobby(NetworkRunner runner) {
        var result = await runner.JoinSessionLobby(SessionLobby.Shared);

        if (result.Ok) {
            // all good
            lobby = true;
        } else {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public bool inLobby() {
        return lobby;
    }
}
