using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExperienceUI : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    public int experience;
    public int currency;
    [SerializeField] private TMP_Text currencyText;
    public static ExperienceUI _instance;
    public static ExperienceUI Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<ExperienceUI>();
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if (_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        canvas.SetActive(false);

    }

    public void ShowExperience()
    {
        canvas.SetActive(true);
    }

    public void HideExperience()
    {
        canvas.SetActive(false);
    }

    // sets the displayed amount of gold
    public void SetGold(int amount)
    {
        currency = amount;
        currencyText.SetText(amount.ToString());
    }
}
