using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public LoginUI loginUI;
    public LobbyUI lobbyUI;
    public MatchmakingUI matchmakingUI;
    public LobbyManager lobbyManager;
    public PlayFabAuthenticator authenticator;
    public GameObject canvas;

    private static MenuUI _instance;
    public static MenuUI Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<MenuUI>();
            return _instance;
        }
    }
    
    private void Start()
    {
        loginUI.gameObject.SetActive(true);
        matchmakingUI.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(false);
        canvas.SetActive(false);
    }

    private void FixedUpdate()
    {
        // if in lobby
        if (lobbyUI.gameObject.activeInHierarchy)
        {
            // grab number of active players
            int players = GameManager.Instance.GetActivePlayers();
            int sessions = GameLauncher.Instance.sessionCount;
            lobbyUI.UpdateSessionData(players, sessions);
        }
    }

    public void OnMatchmake()
    {
        matchmakingUI.gameObject.SetActive(true);
        loginUI.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(false);
    }
    
    public void OnJoinLobby()
    {
        lobbyUI.GetPlayerName(authenticator.getPlayFabID());
        matchmakingUI.gameObject.SetActive(false);
        loginUI.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(true);
    }

    // Activates Shop canvas
    public void OpenShop()
    {
        canvas.SetActive(true);
    }

    // Deactivates Shop canvas
    public void CloseShop()
    {
        canvas.SetActive(false);
    }
}
