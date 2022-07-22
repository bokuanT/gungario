using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private bool lobby = false;
    public GameManager gameManager;
    private NetworkRunner _runner;
    // Joins the Shared mode lobby 
    private SessionLobby lobbyMode = SessionLobby.Shared;

    // Utility method to Join the ClientServer Lobby
    public async Task JoinLobby(NetworkRunner runner) {

        var result = await runner.JoinSessionLobby(lobbyMode);

        if (result.Ok) {
            // all good
            lobby = true;

            //MenuUI.Instance.OnJoinLobby();

            Debug.Log($"Joined Lobby: {lobbyMode}");
        } else {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public bool inLobby() {
        return lobby;
    }
}
