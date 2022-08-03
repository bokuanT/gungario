using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class Shop : MonoBehaviour
{
    private int[] shopItems = new int[4];
    private ShopItem selectedHat;
    private static Shop _instance;
    
    [SerializeField] private GameObject canvas;

    [Header("Currency")]
    [SerializeField] private int currency;
    [SerializeField] private TMP_Text currencyText;

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

    public void AddCurrency(int amount)
    {
        PlayFabClientAPI.AddUserVirtualCurrency(
            new AddUserVirtualCurrencyRequest()
            {
                Amount = amount,
                VirtualCurrency = "GD"
            }, result => 
            {
                Debug.Log($"Success, {amount} added");
            }, error => Debug.LogError(error.GenerateErrorReport()));
        LoadPlayerInfo();
    }

    public void LoadPlayerInfo()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => {
                currency = result.VirtualCurrency.GetValueOrDefault("GD");
                Debug.Log($"player has {currency} gold");
                currencyText.SetText(currency.ToString());
            }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    // Called when player clicks the button in shop, setting the hat
    public void SetHat()
    {
        Debug.Log("Setting Hat");
        GameObject button = GameObject.FindObjectOfType<EventSystem>().currentSelectedGameObject;
        selectedHat = button.GetComponent<ShopItem>();

        // check if hat can be afforded
        PlayFabClientAPI.PurchaseItem(
            new PurchaseItemRequest() 
            {
                ItemId = selectedHat.ItemID,
                Price = selectedHat.price,
                VirtualCurrency = "GD"
            }, result =>
            {
                // if any previous hats were selected, set those to unselected
                if (selectedHat != null)
                {
                    selectedHat.Deselect();
                }

                selectedHat = button.GetComponent<ShopItem>();
                selectedHat.Select();
                
            }, error =>
            {
                // inform user of unsuccessful purchase

                Debug.LogError(error.GenerateErrorReport());

            }
        );

        LoadPlayerInfo();
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
        if (selectedHat == null)
        {
            return 0;
        } else
        {
            // original itemID: "hat1"
            // removes "hat" 
            // parse "1" to int
            return int.Parse(selectedHat.ItemID.Substring("hat".Length));
        }
    }
}
