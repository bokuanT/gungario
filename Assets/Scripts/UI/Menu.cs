using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Menu : MonoBehaviour
{
    public TMP_Text ChangeName; 
    public TMP_Text PlayerName; 
    public PlayFabAuthenticator authenticator;
    public GameManager gameManager;
    private bool nameChanged = false;
    
    
    // temporary boolean to make the apicall happen once per load
    // if not, Update will keep calling to the api
    private bool apicall = false;

    void Update() {
        if (authenticator.isAuthenticated() && apicall == false) {
            // sets the PlayerName from database
            GetPlayerName(authenticator.getPlayFabID());
            apicall = true;
        }
    }

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

    void GetPlayerName(string playFabId) {
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
