using UnityEngine;
using Fusion;

public class PlayerDeath : NetworkBehaviour
{
    public Animator animator;

    public Vector3 spawnPoint;

    [Networked]
	private TickTimer despawnTimer { get; set; }

    public override void Spawned()
    {
        if (spawnPoint != null)
        {
            //Debug.Log("Spawned called, at " + spawnPoint);

            despawnTimer = TickTimer.CreateFromSeconds(Runner, 3f);

        }
    }

    public override void FixedUpdateNetwork()
    {
        if (despawnTimer.Expired(Runner))
        {
            Destroy(gameObject);
        }
        gameObject.transform.position = spawnPoint;
    }

    public void TriggerDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("explosion");
        }
    }
}
