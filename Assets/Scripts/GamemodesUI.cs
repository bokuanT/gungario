using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class GamemodesUI : MonoBehaviour
{
    public GameObject lobbyUI;
    public TMP_Text AvailableSessions;
    [SerializeField] private AudioEmitter StandardSound;
    [SerializeField] private AudioEmitter CancelSound;

    [Header("SessionCount")]
    [SerializeField] private TMP_Text FFAText;
    [SerializeField] private TMP_Text TDMText;
    [SerializeField] private TMP_Text CPText;

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

    public void PlayStandardSound()
    {
        StandardSound.PlayOneShot();
    }

    public void PlayCancelSound()
    {
        CancelSound.PlayOneShot();
    }

    // Called each time session list is updated in GameLauncher.OnSessionListUpdated()
    // Show available session sizes
    public void UpdateSessions(List<SessionInfo> sessionList)
    {
        int FFA = 0;
        int TDM = 0;
        int CP = 0;

        // Set local variables to session players
        foreach (var session in sessionList)
        {
            // FFA gamemodes (FFA)
            if (session.Name.StartsWith('F') && session.PlayerCount != session.MaxPlayers)
            {
                FFA = session.PlayerCount;
            } 
            // TDM gamemodes (Deathmatch)
            if (session.Name.StartsWith('D') && session.PlayerCount != session.MaxPlayers)
            {
                TDM = session.PlayerCount;
            }
            // CP gamemodes (Controlpoint)
            if (session.Name.StartsWith('C') && session.PlayerCount != session.MaxPlayers)
            {
                CP = session.PlayerCount;
            }
        }

        // update variables if required
        if (FFA.ToString() != FFAText.text)
        {
            FFAText.SetText(FFA.ToString());
        }
        if (TDM.ToString() != TDMText.text)
        {
            TDMText.SetText(TDM.ToString());
        }
        if (CP.ToString() != CPText.text)
        {
            CPText.SetText(CP.ToString());
        }
    }
}
