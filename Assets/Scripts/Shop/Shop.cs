using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    private int[] shopItems = new int[4];
    private ShopItem selectedHat;
    private static Shop _instance;
    [SerializeField] GameObject canvas;

    [Header("Hats")]
    [SerializeField] private ShopItem hat1;
    [SerializeField] private ShopItem hat2;
    [SerializeField] private ShopItem hat3;
    public static Shop Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Shop>();
            return _instance;
        }
    }

    public void Start()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if (_instance != this) Destroy(gameObject); 
    }

    // Called when player clicks the button in shop, setting the hat
    public void SetHat()
    {
        Debug.Log("Setting Hat");
        GameObject button = GameObject.FindObjectOfType<EventSystem>().currentSelectedGameObject;

        // if any previous hats were selected, set those to unselected
        if (selectedHat != null)
        {
            selectedHat.Deselect();
        }

        selectedHat = button.GetComponent<ShopItem>();
        selectedHat.Select();
    }

    // Called after playerInfo is loaded, to get the corresponding sprite.
    public Sprite GetHat(int id)
    {
        Debug.Log("Getting Hat");

        // Grabs hat from resources folder, and typecasts as Sprite
        return Resources.Load($"Skins/hat{id}", typeof(Sprite)) as Sprite;
    }

    public void OpenShop()
    {
        canvas.SetActive(true);
    }

    public void CloseShop()
    {
        canvas.SetActive(false);
    }

    public int GetSelectedCosmetics()
    {
        return selectedHat.ItemID;
    }
}
