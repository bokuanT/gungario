using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHat()
    {
        Debug.Log("Setting Hat");
        GameObject button = GameObject.FindObjectOfType<EventSystem>().currentSelectedGameObject;
        Debug.Log(button.name);
    }
}
