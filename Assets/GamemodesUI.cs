using UnityEngine;
using TMPro;
public class GamemodesUI : MonoBehaviour
{
    public GameObject lobbyUI;
    public TMP_Text AvailableSessions;

    public void Back()
    {
        lobbyUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void UpdateSessionData(int players, int sessions)
    {
        // if (players > 1) ActivePlayerCount.SetText($"{players}");
        AvailableSessions.SetText($"{sessions}");

        // In the future, a "active session count" can be implemented by updating when a game starts and ends.
        // To be implemented after game ending and transition is in place
    }
}
