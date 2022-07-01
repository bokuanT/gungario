using UnityEngine;
using Fusion;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private NetworkWeapon[] _weapons;
    [SerializeField] private Player _player;
    
    [Networked]
    public NetworkWeapon activeWeapon { get; set; }

    public void InitNetworkState()
    {
        
        SetActiveWeapon(_weapons[0], true);
        activeWeapon = _weapons[0];
        
    }

    //public void Awake()
    //{
    //    SetActiveWeapon(_weapons[0], true);
    //    activeWeapon = _weapons[0];
    //}

    public void ShowCorrectWeapon()
    {
        if (activeWeapon != null)
        {
            //check active weps
            int count = 0;
            foreach (NetworkWeapon w in _weapons)
            {
                if (w.gameObject.active)
                    count += 1;
            }
            if (count > 1)
            {
                foreach (NetworkWeapon w in _weapons)
                {
                    w.gameObject.transform.parent.gameObject.SetActive(false);
                }
            }
            activeWeapon.gameObject.transform.parent.gameObject.SetActive(true);
            _player.SetGunTransforms(activeWeapon);
        }
    }


    public void SetActiveWeapon(NetworkWeapon weapon, bool init)
    {

        foreach (NetworkWeapon w in _weapons)
        {
            w.gameObject.transform.parent.gameObject.SetActive(false);
        }

        foreach (NetworkWeapon w in _weapons)
        {
            if (w.Equals(weapon))
            {
                if (init || !w.Equals(activeWeapon))
                {
                    w.gameObject.transform.parent.gameObject.SetActive(true);
                    activeWeapon = w;
                    _player.SetGunTransforms(w);
                    break;
                } else //always gets different weapon
                {
                    if (activeWeapon.index + 1 < _weapons.Length)
                    {
                        NetworkWeapon weaponTMP = _weapons[activeWeapon.index + 1];
                        weaponTMP.gameObject.transform.parent.gameObject.SetActive(true);
                        activeWeapon = weaponTMP;
                        _player.SetGunTransforms(weaponTMP);
                        break;
                        
                    } else
                    {
                        NetworkWeapon weaponTMP = _weapons[activeWeapon.index - 1];
                        weaponTMP.gameObject.transform.parent.gameObject.SetActive(true);
                        activeWeapon = weaponTMP;
                        _player.SetGunTransforms(weaponTMP);
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
