using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {

        if (Object.HasInputAuthority)
        {
            Local = this;
        
            Debug.Log("Spawned local player");
        }
        else 
        {
            AudioListener audioListener = GetComponent<AudioListener>();
            audioListener.enabled = false;

            Debug.Log("Spawned remote player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);

    }
}
