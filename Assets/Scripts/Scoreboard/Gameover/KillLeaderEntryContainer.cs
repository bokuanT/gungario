using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillLeaderEntryContainer : MonoBehaviour
{
    //public 
    public Scoreboard scoreboard;

    public GameObject _entryPrefab;

    public Transform container;
    public void SpawnEntry(Player player, int position)
    {
        GameObject item = Instantiate(_entryPrefab, container);
        KillLeaderEntryTemplate entry = item
            .GetComponentInChildren<KillLeaderEntryTemplate>();
        entry.Initialize(player, position);
    }

    public void ResetEntries()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "KillLeaderEntryTemplate(Clone)")
                Destroy(child.gameObject);
        }
    }
}
