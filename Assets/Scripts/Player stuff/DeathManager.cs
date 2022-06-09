using Fusion;
using UnityEngine;

public class DeathManager : NetworkBehaviour
{
    [SerializeField] private PlayerDeath _deathPrefab;

    public void OnDeath(NetworkRunner runner, PlayerRef owner, Transform PlayerTransform)
    {
        runner.Spawn(_deathPrefab, PlayerTransform.position, PlayerTransform.rotation,
            owner, InitDeath);
        void InitDeath(NetworkRunner runner, NetworkObject obj)
        {
            PlayerDeath pd = obj.gameObject.GetComponent<PlayerDeath>();
            pd.TriggerDeath();
        }
    }
}
