using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public GameObject loginText;
    public GameObject AuthUI;
    public PlayFabAuthenticator authenticator;

    public void OnJoinLobby()
    {
        gameObject.SetActive(false);
    }
    
    // Called by button
    public void LoginAsGuest() {
        authenticator.guestAuthentication();
        AuthUI.SetActive(false);
        loginText.SetActive(true);
    }

}
