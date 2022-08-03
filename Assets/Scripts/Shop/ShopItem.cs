using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour
{
    public string ItemID;
    public int price;
    public GameObject Shop;
    
    [SerializeField] private Image button;
    [SerializeField] private GameObject selectedText;

    // this script will be used to load item prices from the shop

    // Display which skin is in use
    public void Select()
    {
        selectedText.SetActive(true);
        button.color = Color.gray;
    }

    public void Deselect()
    {
        selectedText.SetActive(false);
        button.color = Color.white;
    }
}

