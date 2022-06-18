using UnityEngine;
using Fusion;
using TMPro;

public class Scoreboard_item : NetworkBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public int player_id;
    
    [Networked]
    public int kills { get; set; }

    [Networked]
    public int deaths { get; set; }

    public void Initialize(PlayerRef playerRef)
    {
        string name = "player " + playerRef.PlayerId;
        usernameText.SetText(name, true);
        this.player_id = playerRef.PlayerId;
        this.kills = 0;
        this.deaths = 0;
    }

    public void IncrementKill()
    {
        this.kills += 1;
        
    }

    public void IncrementDeath()
    {
        this.deaths += 1;
        
    }

    public void UpdateKD()
    {
        killsText.SetText(this.kills.ToString(), true);
        deathsText.SetText(this.deaths.ToString(), true);
    }
        
}
