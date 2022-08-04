using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class Shop : MonoBehaviour
{
    private ShopItem selectedHat;
    private static Shop _instance;
    
    [SerializeField] private GameObject canvas;

    // improvement with get components in children?
    [Header("Hats")]
    [SerializeField] private ShopItem hat1;
    [SerializeField] private ShopItem hat2;
    [SerializeField] private ShopItem hat3;
    private List<ShopItem> shopItems;
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
        shopItems = new List<ShopItem>() {hat1, hat2, hat3};
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
        LoadPlayerInfo(false);
    }

    // Updates the Gold amount when called
    // Takes time for result to be returned
    // boolean is taken to refresh shop when required, to avoid unnecessary searching 
    public void LoadPlayerInfo(bool shopRefresh)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result => {

                if (shopRefresh)
                {
                    foreach (var item in result.Inventory)
                    {
                        // list bought hats as bought
                        ShopItem shopItem = shopItems.Find(x => x.ItemID == item.ItemId);
                        if (shopItem != null) shopItem.Bought();
                    }
                }
                Inventory.Instance.UpdateInventory(result.Inventory);
                int amount = result.VirtualCurrency.GetValueOrDefault("GD");
                Debug.Log($"player has {amount} gold");
                ExperienceUI.Instance.SetGold(amount);
            }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    // Coroutine is used here, as purchasing item from playfab takes time. 
    IEnumerator BuyCoroutine()
    {
        bool purchaseComplete = false;
        bool purchaseFailed = false;
        GameObject button = GameObject.FindObjectOfType<EventSystem>().currentSelectedGameObject;
        selectedHat = button.GetComponent<ShopItem>();
        NotificationUI.Instance.GeneratePopUp("Purchasing...");

        // check if hat can be afforded
        PlayFabClientAPI.PurchaseItem(
            new PurchaseItemRequest() 
            {
                ItemId = selectedHat.ItemID,
                Price = selectedHat.price,
                VirtualCurrency = "GD"
            }, result =>
            {
                selectedHat = button.GetComponent<ShopItem>();
                selectedHat.Bought();
                purchaseComplete = true;
                
            }, error =>
            {
                // inform user of unsuccessful purchase
                NotificationUI.Instance.GenerateTimedPopUp("Insufficient Funds", 2);
                purchaseFailed = true;
                Debug.LogError(error.GenerateErrorReport());
            }
        );

        while (purchaseComplete == false && purchaseFailed == false)
        { 
            Debug.Log("Pending purchase...");
            yield return null;
        }
        if (purchaseComplete) NotificationUI.Instance.ClosePopUp();
        LoadPlayerInfo(true);
    }

    // Method called to purchase hat from PlayFab catalog
    // Assigned to Shop item buttons
    public void BuyHat()
    {
        Debug.Log("Buying Hat");
        StartCoroutine(BuyCoroutine());
    }

    public void OpenShop()
    {
        canvas.SetActive(true);
    }

    public void CloseShop()
    {
        canvas.SetActive(false);
    }
}
