using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchmakingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text sessionCount;
    [SerializeField] private GameObject cancelMatchmaking;
    [SerializeField] private AudioEmitter StandardSound;
    [SerializeField] private AudioEmitter CancelSound;
    private int capacity;
    
    public void OnMatchmake()
    {
        cancelMatchmaking.SetActive(true);
        // change UI to returning to lobby...
        text.SetText("Matchmaking...");
        if (GameLauncher.Instance.gamemode == Gamemode.FFA)
        {
            capacity = 2;
        }
        if (GameLauncher.Instance.gamemode == Gamemode.CP || GameLauncher.Instance.gamemode == Gamemode.TDM)
        {
            capacity = 4;
        }
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

    void Update()
    {
        sessionCount.SetText($"{GameLauncher.Instance.gameObject.transform.childCount} / {capacity}");
    }
}
