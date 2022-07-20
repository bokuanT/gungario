using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Script changes the CursorUI sort order so that the player can use the pause menu
public class CursorUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject image; 

    // Update is called once per frame
    void Update()
    {
        if (PauseMenuUI.GameIsPaused)
        {
            // lower the UI canvas
            canvas.sortingOrder = 0;
        } else
        {
            // raise the UI canvas
            canvas.sortingOrder = 3;
            
        }
    }
}
