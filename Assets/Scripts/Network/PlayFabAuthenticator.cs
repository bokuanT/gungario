using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using Fusion.Photon.Realtime;

public class PlayFabAuthenticator : MonoBehaviour {

    public GameManager gameManager;
    private string _playFabPlayerIdCache;

    // TO BE IMPLEMENTED WHEN NECESSARY 
    // public NetworkRunnerHandler runner;

    private bool authenticate = false;

    /*
    * ===========================================================================================================================================================================================================================
    * WARNING: DO NOT SHIP WITH THIS! 
    * APPID CONTAINS PHOTON CLOUD APPID INFO
    * PURELY FOR TESTING PURPOSES
    * ===========================================================================================================================================================================================================================
    */
    private string appID = "7df052a1-dc1e-480c-bb50-1f4058771c25";

    //Run the entire thing on button press
    public void guestAuthentication() {
        AuthenticateWithCustomID();
    }

    /*
     * Step 1
     * We authenticate current PlayFab user normally.
     * In this case we use LoginWithCustomID API call for simplicity.
     * You can absolutely use any Login method you want.
     * We use PlayFabSettings.DeviceUniqueIdentifier as our custom ID.
     * We pass RequestPhotonToken as a callback to be our next step, if
     * authentication was successful.
     */
    private void AuthenticateWithCustomID(){
        LogMessage("PlayFab authenticating using Custom ID...");

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = PlayFabSettings.DeviceUniqueIdentifier
        }, RequestPhotonToken, OnPlayFabError);
    }

    /*
    * Step 2
    * We request Photon authentication token from PlayFab.
    * This is a crucial step, because Photon uses different authentication tokens
    * than PlayFab. Thus, you cannot directly use PlayFab SessionTicket and
    * you need to explicitly request a token. This API call requires you to
    * pass Photon App ID. App ID may be hard coded, but, in this example,
    * We are accessing it using convenient static field on PhotonNetwork class
    * We pass in AuthenticateWithPhoton as a callback to be our next step, if
    * we have acquired token successfully
    */
    private void RequestPhotonToken(LoginResult obj) {
        LogMessage("PlayFab authenticated. Requesting photon token...");
        //We can player PlayFabId. This will come in handy during next step
        _playFabPlayerIdCache = obj.PlayFabId;
        PlayFabClientAPI.GetPhotonAuthenticationToken(new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = appID
        }, AuthenticateWithPhoton, OnPlayFabError);
    }

    /*
     * Step 3
     * This is the final and the simplest step. We create new AuthenticationValues instance.
     * This class describes how to authenticate a players inside Photon environment.
     */
    private async void AuthenticateWithPhoton(GetPhotonAuthenticationTokenResult obj) {
        LogMessage("Photon token acquired: " + obj.PhotonCustomAuthenticationToken + "  Authentication complete.");

        //We set AuthType to custom, meaning we bring our own, PlayFab authentication procedure.
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
        //We add "username" parameter. Do not let it confuse you: PlayFab is expecting this parameter to contain player PlayFab ID (!) and not username.
        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);    // expected by PlayFab custom auth service

        //We add "token" parameter. PlayFab expects it to contain Photon Authentication Token issued to your during previous step.
        customAuth.AddAuthParameter("token", obj.PhotonCustomAuthenticationToken);

        //We finally tell Photon to use this authentication parameters throughout the entire application.
        // TO BE IMPLEMENTED WHEN NECESSARY
        //runner.setAuthValues(customAuth);
        
        // Set flag to move to next menu
        authenticate = true;

        // enters lobby
        await gameManager.JoinLobby();
    }

    public bool isAuthenticated() {
        return authenticate;
    }

    public string getPlayFabID() {
        return _playFabPlayerIdCache;
    }

    private void OnPlayFabError(PlayFabError obj) {
        LogMessage(obj.GenerateErrorReport());
    }

    public void LogMessage(string message) {
        Debug.Log("PlayFabAuthenticator: " + message);
    }
}