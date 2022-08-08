using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MenuUI : MonoBehaviour
{
    public LoginUI loginUI;
    public LobbyUI lobbyUI;
    public GamemodesUI gamemodeUI;
    public MatchmakingUI matchmakingUI;
    public LobbyManager lobbyManager;
    public PlayFabAuthenticator authenticator;
    public GameObject shopCanvas;
    public GameObject inventoryCanvas;
    public GameObject settingsCanvas;
    public GameObject gameModesUI;
    public GameObject weaponsHelp;

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
        Cursor.visible = true;
        loginUI.gameObject.SetActive(true);
        matchmakingUI.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(false);
        settingsCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        gameModesUI.SetActive(false);
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
            gamemodeUI.UpdateSessionData(players, sessions);
        } 

        if (gamemodeUI.gameObject.activeInHierarchy)
        {
            int players = GameManager.Instance.GetActivePlayers();
            int sessions = GameLauncher.Instance.sessionCount;
            gamemodeUI.UpdateSessionData(players, sessions);
        }
    }

    public void OnMatchmake()
    {
        matchmakingUI.gameObject.SetActive(true);
        matchmakingUI.OnMatchmake();
        loginUI.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(false);
        gameModesUI.SetActive(false);

        // hides experience bar
        ExperienceUI.Instance.HideExperience();
    }
    
    public void OnJoinLobby()
    {
        lobbyUI.GetPlayerName(authenticator.getPlayFabID());
        matchmakingUI.gameObject.SetActive(false);
        loginUI.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(true);

        // displays experience bar
        ExperienceUI.Instance.ShowExperience();

        // updates experience bar everytime we enter lobby
        ExperienceBar.Instance.SetExperience(ExperienceUI.Instance.experience);
    }

    // Activates Shop canvas
    public void OpenShop()
    {
        Shop.Instance.OpenShop();
    }

    // Activiates Inventory canvas
    public void OpenInventory()
    {
        Inventory.Instance.OpenInventory();
    }

    // Activates Settings canvas
    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
    }

    // Deactivates Settings canvas
    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
    }

    public void OpenWeaponsHelp()
    {
        weaponsHelp.SetActive(true);
    }

    public void CloseWeaponsHelp()
    {
        weaponsHelp.SetActive(false);
    }

    public void UpdateSessions(List<SessionInfo> sessionList)
    {
        gamemodeUI.UpdateSessions(sessionList);
    }
}
