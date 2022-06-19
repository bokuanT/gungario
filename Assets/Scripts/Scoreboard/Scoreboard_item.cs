using UnityEngine;
using Fusion;
using TMPro;

public class Scoreboard_item : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public int player_id;

    public int kills;

    public int deaths;

    public Player player;

    public void Initialize(NetworkRunner runner, PlayerRef playerRef)
    {
        string name = "player " + playerRef.PlayerId;
        usernameText.SetText(name, true);
        this.player_id = playerRef.PlayerId;
        player = PlayerInfoManager.Get(runner, playerRef);
        player.scoreboard_item = this;
        
    }

    void Update()
    {
        if (player != null)
        {
            kills = player.kills;
            deaths = player.deaths;
        }
    }

    public void UpdateKD()
    {
        killsText.SetText(this.kills.ToString(), true);
        deathsText.SetText(this.deaths.ToString(), true);
    }
        
}
