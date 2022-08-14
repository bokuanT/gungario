using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class ProgressBar : NetworkBehaviour
{
    public Slider slider;
    
    public Image fill;

    public const float BASE_CREMENT = 0.2F;

    public SpriteRenderer controlPoint;

    [Networked]
    public float sliderValue { get; set; }

    [Networked(OnChanged = nameof(OnTeamChanged))]
    public Team team { get; set; }

    public enum Team
    {
        None,
        Red,
        Blue
    }

    public override void Spawned()
    {
        team = Team.None;
        sliderValue = 100F;
    }

    public static void OnTeamChanged(Changed<ProgressBar> changed)
    {
        if (changed.Behaviour)
        {
            changed.Behaviour.SetColour();
        }
    }

    public override void FixedUpdateNetwork()
    {
        slider.value = sliderValue;
    }
    private void SetColour()
    {
        Image img = transform.Find("Fill").GetComponent<Image>();

        switch (team)
        {
            case Team.None:
                img.color = Color.white;
                controlPoint.color = Color.white;
                break;
            case Team.Red:
                img.color = Color.red;
                controlPoint.color = Color.red;
                break;
            case Team.Blue:
                img.color = Color.blue;
                controlPoint.color = Color.blue;
                break;
        }
    }
    public void UpdatePercen(int multiplier, Team teamOnPlate)
    {
        if (team == Team.None)
        {
            DecrementSilder(multiplier);

            if (sliderValue <= 0)
            {
                team = teamOnPlate;
            }
        }

        else
        {
            if (teamOnPlate != team)
            {
                DecrementSilder(multiplier);
                if (sliderValue <= 0)
                    team = teamOnPlate;
            }
            else
            {
                IncrementSilder(multiplier);
            }
        }
        
    }

    private void DecrementSilder(int multiplier)
    {
        sliderValue -= multiplier * BASE_CREMENT;

        if (sliderValue < 0)
            sliderValue = 0;
    }

    private void IncrementSilder(int multiplier)
    {
        sliderValue += multiplier * BASE_CREMENT;

        if (sliderValue > 100)
            sliderValue = 100;
    }
    public bool IsNeutral()
    {
        return team == Team.None;
    }

    public void ResetProgress()
    {
        team = Team.None;
        sliderValue = 100F;
        SetColour();
    }

}
