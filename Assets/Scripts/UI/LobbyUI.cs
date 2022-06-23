using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    public TMP_Text ChangeName; 
    public TMP_Text PlayerName; 
    public PlayFabAuthenticator authenticator;
    public GameManager gameManager;
    public MenuUI menu;
    private bool nameChanged = false;

    public void StartGame() {
        Debug.Log("StartGame");

        if (nameChanged) GetPlayerName(authenticator.getPlayFabID());

        // changed to this temporarily to stop loading game scene together with menu 
        // SceneManager.LoadScene(LevelManager.TESTGAME_SCENE);
        gameManager.SetScene(LevelManager.TESTGAME_SCENE);
        gameManager.StartGame();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void MatchmakeDeathMatch() {
        if (nameChanged) GetPlayerName(authenticator.getPlayFabID());
        gameManager.SetScene(LevelManager.MAP1_SCENE);
        gameManager.MatchmakeDeathMatch();
        menu.IsMatchmaking(true);
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
            if (result.PlayerProfile.DisplayName == null || result.PlayerProfile.DisplayName == "") result.PlayerProfile.DisplayName = "null";
            Debug.Log("Retrieved DisplayName. The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
            gameManager.SetPlayerProfile(result.PlayerProfile);
            PlayerName.SetText("Welcome back, " + result.PlayerProfile.DisplayName + "!");
        },
        error => Debug.LogError(error.GenerateErrorReport()));
    }
}
