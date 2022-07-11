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
        this.player_id = playerRef.PlayerId;
        player = PlayerInfoManager.Get(runner, playerRef);
        player.scoreboard_item = this;
        string name = player.playerName;
        Debug.Log($"name is {name}");
        usernameText.SetText(name, true);

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

    public void Reset()
    {
        this.player.kills = 0;
        this.player.deaths = 0;
        player.ForceMakeHealthySetSpawn(Utils.GetRandomSpawnPoint());
    }

}
