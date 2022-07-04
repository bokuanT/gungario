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

    private static MenuUI _instance;
    public static MenuUI Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<MenuUI>();
            return _instance;
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
}
