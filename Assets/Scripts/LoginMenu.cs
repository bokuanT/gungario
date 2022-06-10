using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    public PlayFabAuthenticator authenticator;
    private Canvas canvas;

    void Start() {
        canvas = GetComponent<Canvas>();
    }

    void Update() {
        if (authenticator.isAuthenticated() && canvas.enabled) {
            canvas.enabled = false;
        }
    }
    public void PlayAsGuest() {
        authenticator.guestAuthentication();
    }

    public void LoadGameMenu() {
        // changed to this temporarily to stop loading game scene together with menu 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //SceneManager.LoadScene(gameScene);
    }

}
