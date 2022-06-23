using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public PlayFabAuthenticator authenticator;

    public void OnJoinLobby()
    {
        this.gameObject.SetActive(false);
    }
    
    // Called by button
    public void LoginAsGuest() {
        authenticator.guestAuthentication();
    }

}
