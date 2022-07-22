using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchmakingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameObject cancelMatchmaking;
    [SerializeField] private AudioEmitter StandardSound;
    [SerializeField] private AudioEmitter CancelSound;
    public void OnMatchmake()
    {
        cancelMatchmaking.SetActive(true);
        // change UI to returning to lobby...
        text.SetText("Matchmaking...");
    }

    public void QuitMatchmake()
    {
        Debug.Log("Exit Matchmaking");
        cancelMatchmaking.SetActive(false);
        text.SetText("Returning to Lobby...");
        GameLauncher.Instance.LeaveSession();
    }
    public void PlayStandardSound()
    {
        StandardSound.PlayOneShot();
    }

    public void PlayCancelSound()
    {
        CancelSound.PlayOneShot();
    }
}
