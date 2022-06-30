using UnityEngine;
using Fusion;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private NetworkWeapon[] _weapons;
    [SerializeField] private Player _player;
    
    [Networked]
    private NetworkWeapon activeWeapon { get; set; }

    public override void Spawned()
    {
        SetActiveWeapon(_weapons[0]);
        activeWeapon = _weapons[0];
    }

    public void SetActiveWeapon(NetworkWeapon weapon)
    {
        foreach (NetworkWeapon w in _weapons)
        {
            w.gameObject.transform.parent.gameObject.SetActive(false);

            if (w.Equals(weapon))
            {
                if (!w.Equals(activeWeapon))
                {
                    w.gameObject.transform.parent.gameObject.SetActive(true);
                    activeWeapon = w;
                    _player.SetGunTransforms(w);
                } else //always gets different weapon
                {
                    if (activeWeapon.index + 1 < _weapons.Length)
                    {
                        SetActiveWeapon(_weapons[activeWeapon.index + 1]);
                        break;
                    } else
                    {
                        SetActiveWeapon(_weapons[activeWeapon.index - 1]);
                        break;
                    }
                }
            }
        }
    }

    public void Fire(NetworkRunner runner, PlayerRef owner, Vector3 ownerVelocity)
    {
        activeWeapon.Fire(runner, owner, ownerVelocity);
    }

    public NetworkWeapon GetActiveWeapon()
    {
        return activeWeapon;
    }

}
