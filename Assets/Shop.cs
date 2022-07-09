using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    private int[] shopItems = new int[4];
    private int selectedHat = 0;
    private static Shop _instance;
    public static Shop Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Shop>();
            return _instance;
        }
    }
    // need a better way of storing and accessing cosmetics
    public GameObject hat1;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Called when player clicks the button in shop, setting the hat
    public void SetHat()
    {
        Debug.Log("Setting Hat");
        GameObject button = GameObject.FindObjectOfType<EventSystem>().currentSelectedGameObject;
        Debug.Log(button.name);
        selectedHat = button.GetComponent<ShopItem>().ItemID; 
    }
    // Called after playerInfo is loaded, to get the corresponding sprite.
    public Sprite GetHat(int id)
    {
        Debug.Log("Getting Hat");

        // Grabs hat from resources folder, and typecasts as Sprite
        return Resources.Load($"Skins/hat{id}", typeof(Sprite)) as Sprite;
    }

    public int GetSelectedCosmetics()
    {
        return selectedHat;
    }
}
