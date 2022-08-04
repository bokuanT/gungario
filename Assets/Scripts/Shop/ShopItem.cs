using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// Items sold in Shop

/*
 * To create a ShopItem, 
 *  1. Create/Duplicate button in ScrollView UI
 *  2. Add reference under Shop, and Shop.Start() array
 *  3. Add Sprite in Inspector
 *  4. Fill in Price in Inspector
 *  5. ItemId in Inspector "_ _ _"(string of len 3) + " "(counter)
*/
public class ShopItem : MonoBehaviour
{
    public string ItemID;
    public int price;
    
    [SerializeField] private Image button;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private GameObject purchasedText;

    // this script will be used to load item prices from the shop

    void Start()
    {
        priceText.SetText(price.ToString() + "G");
    }

    public void Bought()
    {
        purchasedText.SetActive(true);
        button.color = Color.gray;
        gameObject.GetComponent<Button>().interactable = false;
    }
}

