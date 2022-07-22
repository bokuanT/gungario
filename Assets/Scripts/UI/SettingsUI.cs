using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameChangeField;
    [SerializeField] private GameObject nameChangeSuccess;
    [SerializeField] private GameObject nameChangeFail;
    private static SettingsUI _instance;
    public static SettingsUI Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<SettingsUI>();
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        nameChangeSuccess.SetActive(false);
        nameChangeFail.SetActive(false);
    }

    public void OnNameChange(bool result)
    {
        if (result)
        {
            nameChangeSuccess.SetActive(true);
            nameChangeFail.SetActive(false);
        }
        else
        {
            nameChangeSuccess.SetActive(false);
            nameChangeFail.SetActive(true);
        }
    }

    // resets the canvas when user closes the page so the name change messages disappear;
    public void OnClose()
    {
        nameChangeField.text = "";
        nameChangeSuccess.SetActive(false);
        nameChangeFail.SetActive(false);
    }
}
