using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillLeaderEntryTemplate : MonoBehaviour
{
    public Text username;

    public Text kills;

    public Text position;

    public void Initialize(Player player, int position)
    {
        username.text = player.playerName;
        kills.text = player.kills.ToString();
        this.position.text = position.ToString();
    }
    
}
