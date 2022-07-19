using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour
{
    public int ItemID;

    public GameObject Shop;

    public Image buttonImage;

    public GameObject selectedText;

    // this script will be used to load item prices from the shop

    // Display which skin is in use
    public void Select()
    {
        selectedText.SetActive(true);
        buttonImage.color = Color.gray;
    }

    public void Deselect()
    {
        selectedText.SetActive(false);
        buttonImage.color = Color.white;
    }
}

