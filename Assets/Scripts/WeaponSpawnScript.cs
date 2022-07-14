using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponSpawnScript : NetworkBehaviour
{
    [SerializeField] private GameObject[] _networkWeapons;

    [Networked]
    public int activePowerupIndex { get; set; }

    [Networked]
    public TickTimer respawnDelay { get; set; }
    
    private const float RESPAWN = 7f;


    public GameObject AvailableIcon;

    public override void Spawned()
    {
        base.Spawned();
        respawnDelay = TickTimer.None;
    }
    public override void FixedUpdateNetwork()
    {
        if (respawnDelay.ExpiredOrNotRunning(Runner))
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

    public NetworkWeapon Pickup()
    {
        if (respawnDelay.ExpiredOrNotRunning(Runner))
        {
            NetworkWeapon ret = _networkWeapons[activePowerupIndex].GetComponentInChildren<NetworkWeapon>();
            SetNextWeaponIndex();
            return ret;
        }
        return null;
    }


    private void SetNextWeaponIndex()
    {
        respawnDelay = TickTimer.CreateFromSeconds(Runner, RESPAWN);
        int i = Random.Range(0, _networkWeapons.Length);
        activePowerupIndex = i;

    }
}
