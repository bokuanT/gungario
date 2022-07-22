using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuCanvas;

    public GameObject settingsImage;


    // Start is called before the first frame update
    void Start()
    {
        pauseMenuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        GameIsPaused = false;
        Cursor.visible = false;
        CloseSettings();
    }

    void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        // Since enemy scripts are networked, they will not be paused.
        // freeze time if singleplayer
        GameIsPaused = true;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Resume();
    }

    public void OpenSettings()
    {
        settingsImage.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsImage.SetActive(false);
    }

    public void QuitToMenu()
    {
        Debug.Log("Returning to Menu");
        GameLauncher.Instance.LeaveSession();
    }
}
