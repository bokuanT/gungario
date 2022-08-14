using Fusion;
using UnityEngine;

public class DeathManager : NetworkBehaviour
{
    [SerializeField] private PlayerDeath _deathPrefab;

    public void OnDeath(NetworkRunner runner, PlayerRef owner)
    {
        Transform playerTransform = gameObject.transform;
        // Debug.Log("player t " + playerTransform.position);
        // runner.Spawn(_deathPrefab, playerTransform.position, playerTransform.rotation,
        //     owner, InitDeath);
        // void InitDeath(NetworkRunner runner, NetworkObject obj)
        // {
        //     PlayerDeath pd = obj.gameObject.GetComponent<PlayerDeath>();
        //     pd.transform.position = playerTransform.position;
        //     pd.TriggerDeath();
        // }

        var key = new NetworkObjectPredictionKey {Byte0 = (byte) owner.RawEncoded, Byte1 = (byte) runner.Simulation.Tick};
        runner.Spawn(_deathPrefab, playerTransform.position, playerTransform.rotation, owner, (runner, obj) =>
        {
            //Debug.Log("player transform is: " + playerTransform.position);
            PlayerDeath pd = obj.GetComponent<PlayerDeath>();
            pd.spawnPoint = playerTransform.position;
            pd.TriggerDeath();
            
        }, key );
    }
}
