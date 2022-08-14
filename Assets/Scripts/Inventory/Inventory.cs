using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PlayFab;
using PlayFab.ClientModels;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private InventoryItem selectedHat;
    private List<InventoryItem> inventoryItems;
    private List<ItemInstance> PlayfabItems;

    // improvement with get components in children?
    [Header("Hats")]
    [SerializeField] private InventoryItem hat1;
    [SerializeField] private InventoryItem hat2;
    [SerializeField] private InventoryItem hat3;

    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Inventory>();
            return _instance;
        }
    }

    public void Start()
    {
        inventoryItems = new List<InventoryItem>() { hat1, hat2, hat3 };
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

        selectedHat = button.GetComponent<InventoryItem>();
        selectedHat.Select();
    }

    // this to set local cosmetic values before the RPC_Call
    public int GetSelectedCosmetics()
    {
        if (selectedHat == null)
        {
            return 0;
        }
        else
        {
            // original itemID: "hat1"
            // removes "hat" 
            // parse "1" to int
            return int.Parse(selectedHat.ItemID.Substring("hat".Length));
        }
    }

    // Called after playerInfo is loaded, to get the corresponding sprite.
    public Sprite GetHat(int id)
    {
        Debug.Log("Getting Hat");

        // Grabs hat from resources folder, and typecasts as Sprite
        return Resources.Load($"Skins/hat{id}", typeof(Sprite)) as Sprite;
    }

    // Called by Shop whenever it reloads the player's cosmetics
    public void UpdateInventory(List<ItemInstance> list)
    {
        PlayfabItems = list;
    }

    public void OpenInventory()
    {
        // register whichever items are present, and makes them selectable in inventory
        foreach (var item in PlayfabItems)
        {
            InventoryItem inventoryItem = inventoryItems.Find(x => x.ItemID == item.ItemId);
            inventoryItem.gameObject.GetComponent<Button>().interactable = true;
        }
        canvas.SetActive(true);
    }

    public void CloseInventory()
    {
        canvas.SetActive(false);
    }
}
