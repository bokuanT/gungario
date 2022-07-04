using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchmakingUI : MonoBehaviour
{

    public void QuitMatchmake()
    {
        Debug.Log("Exit Matchmaking");
        MenuUI.Instance.OnJoinLobby();
        GameLauncher.Instance.LeaveSession();
    }

}
