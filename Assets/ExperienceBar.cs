using System;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Collections.Generic;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public Player player;
    public TMP_Text levelText;
    public static ExperienceBar _instance;
    public static ExperienceBar Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<ExperienceBar>();
            return _instance;
        }
    }

    public void SetExperience(int amount)
    {
        Debug.Log($"setting {amount} of experience");
        int currentExp = int.Parse(levelText.text) * 1000 + ((int)slider.value);

        // Send updates to playfab only if the user has receieved > 0 exp and a new value is set
        if (amount > currentExp)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(
                new UpdatePlayerStatisticsRequest()
                {
                    Statistics = new List<StatisticUpdate>() { new StatisticUpdate() { StatisticName = "Experience", Value = amount, Version = null } }
                },
                result => Debug.Log("Complete"),
                error => 
                {
                    Debug.Log($"Failed to update experience: {error.GenerateErrorReport()}");
                }
            );
        }

        int level = amount / 1000;
        levelText.SetText(level.ToString());
        AddExperience(amount % 1000);

    }

    public void AddExperience(int amount)
    {
        if (slider.value + amount > 1000)
        {
            slider.value = (slider.value + amount - 1000);
            int level = int.Parse(levelText.text) + 1;
            levelText.SetText(level.ToString());
        } else
        {
            slider.value += amount;
        }
        

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
