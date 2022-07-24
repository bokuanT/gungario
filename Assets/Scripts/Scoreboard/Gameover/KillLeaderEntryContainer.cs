using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillLeaderEntryContainer : MonoBehaviour
{
    //public 
    public Scoreboard scoreboard;

    public GameObject _entryPrefab;

    public Transform container;

    private MusicManager musicManager;

    [SerializeField] private AudioEmitter VictorySound;

    private void Awake()
    {
        musicManager = GameObject.Find("MusicManager")
            .GetComponent<MusicManager>();
    }

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

    public void StartVictorySound()
    {
        musicManager.StopSound();
        VictorySound.PlayOneShot();
    }

    public void StopVictorySoundInFiveSeconds()
    {
        Invoke("StopSound", 5f);
        
    }

    private void StopSound()
    {
        VictorySound.Stop();
    }

    
}
