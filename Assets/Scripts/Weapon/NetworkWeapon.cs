using UnityEngine;
using Fusion;

public class NetworkWeapon : NetworkRigidbody2D
{
    [SerializeField] private Projectile _projectilePrefab;

    [Networked]
	public TickTimer primaryFireDelay { get; set; }

    private const float DELAY = 0.8f;
    
    public void Fire(NetworkRunner runner, PlayerRef owner, Vector3 ownerVelocity)
    {
        if (primaryFireDelay.ExpiredOrNotRunning(Runner))
        {
            Transform exit = GetExitPoint();
            SpawnNetworkShot(runner, owner, exit, ownerVelocity);
            primaryFireDelay = TickTimer.CreateFromSeconds(Runner, DELAY);

        }
    }

    private void SpawnNetworkShot(NetworkRunner runner, PlayerRef owner, Transform exit, Vector3 ownerVelocity)
    {
        //Debug.Log($"Spawning Shot in tick {Runner.Simulation.Tick} stage={Runner.Simulation.Stage}");
        // Create a key that is unique to this shot on this client so that when we receive the actual NetworkObject
        // Fusion can match it against the predicted local bullet.
        var key = new NetworkObjectPredictionKey {Byte0 = (byte) owner.RawEncoded, Byte1 = (byte) runner.Simulation.Tick};
        runner.Spawn(_projectilePrefab, exit.position, exit.rotation, owner, (runner, obj) =>
        {
            obj.GetComponent<Projectile>().InitNetworkState(ownerVelocity, gameObject.transform);
        }, key );
        //Debug.Log("Getting exit pt " + exit.position);
    }

    private Transform GetExitPoint()
    {
        return gameObject.transform;
    }
}