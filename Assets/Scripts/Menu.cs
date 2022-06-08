using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string gameScene;
    PlayFabAuthenticator authenticator;

    // Start is called before the first frame update
    void Start()
    {
        authenticator = GetComponentInChildren<PlayFabAuthenticator>();
    }

    public void PlayAsGuest() {
        authenticator.guestAuthentication();
        Debug.Log("Authenticated");
        StartGame();    
    }
    public void StartGame() {
        Debug.Log("StartGame");

        // changed to this temporarily to stop loading game scene together with menu 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        //SceneManager.LoadScene(gameScene);
    }

    public void QuitGame() {
        Application.Quit();
    }


}
