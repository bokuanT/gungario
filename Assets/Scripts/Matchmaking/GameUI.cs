using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    private bool _lobbyIsValid;
    private string lobbyName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getLobby()
    {
        
    }

    public void ValidateLobby()
	{
		_lobbyIsValid = string.IsNullOrEmpty(lobbyName) == false;
	}

	public void TryCreateLobby(GameLauncher launcher)
	{
		if (_lobbyIsValid)
		{
			launcher.JoinOrCreateLobby(lobbyName);
			_lobbyIsValid = false;
		}
	}

}
