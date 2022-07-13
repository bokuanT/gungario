using Fusion;
using UnityEngine;

public class HealthSpawnScript : NetworkBehaviour
{
    [Networked]
    public TickTimer respawnDelayHealth { get; set; }

    private const float RESPAWN = 7f;


    public GameObject AvailableIcon;

    public override void Spawned()
    {
        base.Spawned();
        respawnDelayHealth = TickTimer.None;
    }
    public override void FixedUpdateNetwork()
    {
        if (respawnDelayHealth.ExpiredOrNotRunning(Runner))
        {
            if (AvailableIcon.activeInHierarchy == false)
                AvailableIcon.SetActive(true);
        }
        else
        {
            if (AvailableIcon.activeInHierarchy == true)
                AvailableIcon.SetActive(false);
        }
    }

    public void OnPickUp(Player player)
    {
        if (respawnDelayHealth.ExpiredOrNotRunning(Runner))
        {
            if (player.state == Player.State.Active)
            {
                player.life += 30;
                if (player.life > 100)
                    player.life = 100;

                respawnDelayHealth = TickTimer.CreateFromSeconds(Runner, RESPAWN);
            }
        }
    }
}
