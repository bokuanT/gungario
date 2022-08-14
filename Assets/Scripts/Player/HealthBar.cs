using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class HealthBar : NetworkBehaviour
{
    public Slider slider; 
    public Gradient gradient;
    public Image fill;
    public Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    //TODO use better implementation of OnStateChange
    public override void FixedUpdateNetwork()
    {
        int hp = (int) player.life;
        SetHealth(hp);
    }
}
