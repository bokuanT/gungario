using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class RateOfFireBar : NetworkBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    public Image fill;

    public WeaponManager _weaponManager;

    public Player _player;

    public GameObject bar;

    [Networked]
    public float sliderValue { get; set; }

    public override void Spawned()
    {
        sliderValue = 100F;
    }

    public override void FixedUpdateNetwork()
    {
        if (_player.state == Player.State.Active)
        {
            if (!bar.activeInHierarchy)
                bar.SetActive(true);
            SetSliderVal();
            slider.value = sliderValue;
        }
        else
        {
            if (bar.activeInHierarchy)
                bar.SetActive(false);
        }
    }

    private void SetSliderVal()
    {
        NetworkWeapon wep = _weaponManager.activeWeapon;
        TickTimer timer = wep.primaryFireDelay;

        if (!timer.ExpiredOrNotRunning(Runner))
        {
            float numerator = (float)timer.RemainingTime(Runner);
            float denominator = wep.DELAY;
            sliderValue = 100 - numerator / denominator * 100;
        }
        else
        {
            sliderValue = 100;
        }
    }
}
