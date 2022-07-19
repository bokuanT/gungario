using Fusion;
using UnityEngine;

public class HealthSpawnScript : NetworkBehaviour
{
    [Networked]
    public TickTimer respawnDelayHealth { get; set; }

    private const float RESPAWN = 7f;

    [SerializeField] private AudioEmitter _collectSound;

    [SerializeField] private AudioEmitter _refreshSound;

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
            {
                AvailableIcon.SetActive(true);
                _refreshSound.PlayOneShot();
            }
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
                _collectSound.PlayOneShot();
                respawnDelayHealth = TickTimer.CreateFromSeconds(Runner, RESPAWN);
            }
        }
    }
}
