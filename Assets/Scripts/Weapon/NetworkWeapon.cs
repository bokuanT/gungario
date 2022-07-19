using UnityEngine;
using Fusion;

public class NetworkWeapon : NetworkRigidbody2D
{
    [SerializeField] private Projectile _projectilePrefab;

    [Networked]
	public TickTimer primaryFireDelay { get; set; }

    [Networked]
    public TickTimer cannotShootDelay { get; set; }

    [SerializeField] public float DELAY = 0.8f;

    [SerializeField] private float CANNOTSHOOTDELAY = 0.6f;

    [SerializeField] private AudioEmitter _audioEmitter;

    [SerializeField] private AudioEmitter _audioEmitterCannotShoot;

    [SerializeField] public int index;
    public void Fire(NetworkRunner runner, PlayerRef owner, Vector3 ownerVelocity)
    {
        if (primaryFireDelay.ExpiredOrNotRunning(Runner))
        {
            Transform exit = GetExitPoint();
            SpawnNetworkShot(runner, owner, exit, ownerVelocity);
            primaryFireDelay = TickTimer.CreateFromSeconds(Runner, DELAY);
            if (_audioEmitter != null)
                _audioEmitter.PlayOneShot();
        }
        else //cannot shoot sound plays if weapon is sniper and its more than 50% through the shoot delay
        {
            if (cannotShootDelay.ExpiredOrNotRunning(Runner) && index == 2)
            {
                float numerator = (float)primaryFireDelay.RemainingTime(Runner);
                float denominator = DELAY;
                if (numerator / denominator < 0.5)
                {
                    _audioEmitterCannotShoot.PlayOneShot();
                    cannotShootDelay = TickTimer.CreateFromSeconds(Runner, CANNOTSHOOTDELAY);
                }
            }
        }
    }

    private void SpawnNetworkShot(NetworkRunner runner, PlayerRef owner, Transform exit, Vector3 ownerVelocity)
    {
        //Debug.Log($"Spawning Shot in tick {Runner.Simulation.Tick} stage={Runner.Simulation.Stage}");
        // Create a key that is unique to this shot on this client so that when we receive the actual NetworkObject
        // Fusion can match it against the predicted local bullet.
        //Debug.Log("runner: " + Runner);
        var key = new NetworkObjectPredictionKey {Byte0 = (byte) owner.RawEncoded, Byte1 = (byte) runner.Simulation.Tick};
        Vector3 spn = new Vector3(exit.position.x, exit.position.y);
        runner.Spawn(_projectilePrefab, spn, exit.rotation, owner, (runner, obj) =>
        {
            obj.GetComponent<Projectile>().InitNetworkState(ownerVelocity, gameObject.transform);
        }, key );
        //Debug.Log("Getting exit pt " + exit.position);
    }

    private Transform GetExitPoint()
    {
        return gameObject.transform;
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }

        NetworkWeapon p = obj as NetworkWeapon;
        if ((System.Object)p == null)
        {
            return false;
        }

        return this.index == p.index;
    }
}