using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * To create a InventoryItem, 
 *  1. Create/Duplicate button in ScrollView UI
 *  2. Add reference under Inventory, and Inventory.Start() array
 *  3. Add Sprite in Inspector
 *  4. Set Button Interactable to false in Inspector
 *  5. ItemId in Inspector "_ _ _"(string of len 3) + " "(counter)
*/
public class InventoryItem : MonoBehaviour
{
    public string ItemID;
    [SerializeField] private Image button;
    [SerializeField] private GameObject selectedText;

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
