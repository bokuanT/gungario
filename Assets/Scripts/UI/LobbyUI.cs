using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    public TMP_Text ChangeName; 
    public TMP_Text PlayerName; 
    public TMP_Text ActivePlayerCount;
    public TMP_Text AvailableSessions;
    public PlayFabAuthenticator authenticator;
    private bool nameChanged = false;

    public void StartGame() {
        Debug.Log("StartGame");

        if (nameChanged) GetPlayerName(authenticator.getPlayFabID());

        // changed to this temporarily to stop loading game scene together with menu 
        // SceneManager.LoadScene(LevelManager.TESTGAME_SCENE);
        GameManager.Instance.SetScene(LevelManager.TESTGAME_SCENE);
        GameManager.Instance.StartGame();
    }

    public void QuitGame() {
        Application.Quit();
        GameLauncher.Instance.LeaveGame();
    }

    public void MatchmakeDeathMatch() {
        //  if (nameChanged) GetPlayerName(authenticator.getPlayFabID());
        GameManager.Instance.SetScene(LevelManager.MAP1_SCENE);
        GameManager.Instance.MatchmakeDeathMatch();
        MenuUI.Instance.OnMatchmake();
    }

    // Temporary method to demonstrate persistent data being stored in player profile
    // Stores this data in Playfab if there exists text in the input
    public void SetPlayerName() {
        if (ChangeName == null || ChangeName.GetParsedText() == "") {
            Debug.Log("Enter name in field");
        } else {
            PlayFabClientAPI.UpdateUserTitleDisplayName( new UpdateUserTitleDisplayNameRequest {
                DisplayName = ChangeName.GetParsedText()
            }, result => {
                Debug.Log("The player's display name is now: " + result.DisplayName);
                PlayerName.SetText("Welcome back, " + result.DisplayName + "!");
                nameChanged = true;
            }, error => Debug.LogError(error.GenerateErrorReport()));
        }
    }

    public void GetPlayerName(string playFabId) {
        PlayFabClientAPI.GetPlayerProfile( new GetPlayerProfileRequest() {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints() {
                ShowDisplayName = true
            }
        },
        result => {
            // So won't be null on login
            if (result.PlayerProfile.DisplayName == null || result.PlayerProfile.DisplayName == "") result.PlayerProfile.DisplayName = "Player";
            Debug.Log("Retrieved DisplayName. The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
            GameManager.Instance.SetPlayerProfile(result.PlayerProfile);
            PlayerName.SetText("Welcome back, " + result.PlayerProfile.DisplayName + "!");
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }

    public void UpdateSessionData(int players, int sessions)
    {
        if (players > 1) ActivePlayerCount.SetText($"{players}");
        if (sessions > 0) AvailableSessions.SetText($"{sessions}");
        
        // In the future, a "active session count" can be implemented by updating when a game starts and ends.
        // To be implemented after game ending and transition is in place
    }

}
