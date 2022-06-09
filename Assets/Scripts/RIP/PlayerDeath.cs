using UnityEngine;
using Fusion;

public class PlayerDeath : NetworkBehaviour
{
    public Animator animator;

    public override void Spawned()
    {
        animator = GetComponentInChildren<Animator>();
        TriggerDeath();
    }

    public void TriggerDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("explosion");
        }
    }
}
