using UnityEngine;
using Fusion;

public class PlayerDeath : NetworkBehaviour
{
    public Animator animator;

    public Vector3 spawnPoint;

    private NetworkTransform networkTransform;

    [Networked]
	private TickTimer despawnTimer { get; set; }

    public override void Spawned()
    {
        networkTransform = GetComponent<NetworkTransform>();
        if (spawnPoint != null)
        {
            //Debug.Log("Spawned called, at " + spawnPoint);
            networkTransform.TeleportToPosition(spawnPoint);
            despawnTimer = TickTimer.CreateFromSeconds(Runner, 3f);

        }

    }

    public override void FixedUpdateNetwork()
    {
        if (despawnTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        //gameObject.transform.position = spawnPoint;
    }

    public void TriggerDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("explosion");
        }
    }
}
