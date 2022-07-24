using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeScroller : MonoBehaviour
{
    [SerializeField] private GameObject[] gameModes = new GameObject[3];

    private int index = 0;

    private const int FFA = 0;

    private const int TDM = 1;

    private const int CP = 2;

    // Update is called once per frame
    void Update()
    {
        SetCorrectGamemode();
    }

    private void SetCorrectGamemode()
    {
        if (index == FFA)
        {
            if (!gameModes[FFA].activeInHierarchy)
            {
                SetGamemode(FFA);
            }
        }
        else if (index == TDM)
        {
            if (!gameModes[TDM].activeInHierarchy)
            {
                SetGamemode(TDM);
            }
        }
        else if (index == CP)
        {
            if (!gameModes[CP].activeInHierarchy)
            {
                SetGamemode(CP);
            }
        }
    }

    private void SetGamemode(int ind)
    {
        for (int i = 0; i < gameModes.Length; i++)
        {
            if (i == ind)
            {
                gameModes[i].SetActive(true);
            }
            else 
            {
                gameModes[i].SetActive(false);
            }
        }
    }

    public void IndexUp()
    {
        index += 1;
        if (index >= gameModes.Length)
        {
            index = 0;
        }
    }

    public void IndexDown()
    {
        index -= 1;
        if (index < 0)
        {
            index = gameModes.Length - 1;
        }
    }
}
