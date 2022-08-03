using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    private ShopItem selectedHat;

    // Called when player clicks the button in shop, setting the hat
    public void SetHat()
    {
        Debug.Log("Setting Hat");
        GameObject button = GameObject.FindObjectOfType<EventSystem>().currentSelectedGameObject;
        selectedHat = button.GetComponent<ShopItem>();

        // if any previous hats were selected, set those to unselected
        if (selectedHat != null)
        {
            selectedHat.Deselect();
        }

        selectedHat = button.GetComponent<ShopItem>();
        selectedHat.Select();
    }
}
