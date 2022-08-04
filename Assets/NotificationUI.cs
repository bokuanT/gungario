using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text PopUpText;
    [SerializeField] private Image PopUpScreen;
    private static NotificationUI _instance;
    public static NotificationUI Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<NotificationUI>();
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
        ClosePopUp();
    }

    // Generates a popup
    public void GeneratePopUp(string notification)
    {
        PopUpScreen.enabled = true;
        PopUpText.SetText(notification);
        PopUpText.enabled = true;
    }

    // Closes a popup
    public void ClosePopUp()
    {
        PopUpScreen.enabled = false;
        PopUpText.enabled = false;
    }

    // creates a timed popup
    public void GenerateTimedPopUp(string notification, int seconds)
    {
        PopUpScreen.enabled = true;
        PopUpText.SetText(notification);
        PopUpText.enabled = true;

        Invoke("ClosePopUp", seconds);
    }
}
