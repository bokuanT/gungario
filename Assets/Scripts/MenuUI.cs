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
    private bool menuScene = false;
    private bool APICall = false;
    private bool matchmaking = false;

    void Awake()
    {
        // this results in nullreference
        //LoginUI loginUI = GetComponentInChildren<LoginUI>();
        //LobbyUI lobbyUI = GetComponentInChildren<LobbyUI>();
        //MatchmakingUI matchmakingUI = GetComponentInChildren<MatchmakingUI>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   
        if (matchmaking)
        {
            matchmakingUI.gameObject.SetActive(true);
            loginUI.gameObject.SetActive(false);
            lobbyUI.gameObject.SetActive(false);
        } 
        else if (authenticator.isAuthenticated() && lobbyManager.inLobby()) 
        {
            if (!APICall)
            {
                lobbyUI.GetPlayerName(authenticator.getPlayFabID());
                lobbyUI.gameObject.SetActive(true);
                APICall = true;
                return;
            }
            if (!menuScene)
            { 
                loginUI.gameObject.SetActive(false);
                menuScene = true;
            }
        }
    }

    public void IsMatchmaking(bool val)
    {
        matchmaking = val;
    }
}
