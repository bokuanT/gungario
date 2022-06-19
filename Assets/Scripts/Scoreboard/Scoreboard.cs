using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboard_item_prefab;

   
    private Dictionary<PlayerRef, Scoreboard_item> hashtable = 
        new Dictionary<PlayerRef, Scoreboard_item>();

   
    private HashSet<int> hashSet = new HashSet<int>();

    public override void FixedUpdateNetwork()
    {
        UpdateScoreboard();
        UpdateAllKD(); 
    }

    public void OnPlayerLeft(PlayerRef playerRef)
    {
        hashSet.Remove(playerRef.PlayerId);
        hashtable.Remove(playerRef);
    }

    public void UpdateScoreboard()
    {
        IEnumerable<PlayerRef> playerList = PlayerInfoManager.
            GetAllPlayerRefs(Runner);
        
        foreach (PlayerRef pr in playerList)
        {
            if (!hashSet.Contains(pr.PlayerId)) 
            {
                Debug.Log("creating item for " + pr);
                hashSet.Add(pr.PlayerId);
                Scoreboard_item item = CreateScoreboardItem(pr);
                hashtable.Add(pr, item);
            }
            
        }

    }

    private void UpdateAllKD()
    {
        foreach (Scoreboard_item item in hashtable.Values)
        {
            item.UpdateKD();
        }
    }

    private Scoreboard_item CreateScoreboardItem(PlayerRef playerRef)
    {
        //var key = new NetworkObjectPredictionKey {Byte0 = (byte) playerRef.RawEncoded, Byte1 = (byte) Runner.Simulation.Tick};
        //Debug.Log("runner: " + Runner);
        /*
        NetworkObject item = Runner.Spawn(scoreboard_item_prefab, container.position, Quaternion.identity, playerRef, (runner, obj) =>
        {
            Debug.Log("item spawned init " + obj);
            obj.GetComponent<Scoreboard_item>().Initialize(Runner, playerRef);
            obj.gameObject.transform.SetParent(container);
            obj.gameObject.transform.localScale = new Vector3(1, 1, 1);
        });
        */
        //item.Initialize(playerRef);
        //Debug.Log("item " + item);
        GameObject item = Instantiate(scoreboard_item_prefab, container);
        Scoreboard_item s_item = item.GetComponent<Scoreboard_item>();
        s_item.Initialize(Runner, playerRef);
        return s_item;
    }

    // public void OnPlayerJoined (NetworkRunner runner, PlayerRef player)
 	// {
         
    // }
 
    // public void OnPlayerLeft (NetworkRunner runner, PlayerRef player)
 	// {}
 
    // public void OnInput (NetworkRunner runner, NetworkInput input)
    // {}    
    
    // public void OnInputMissing (NetworkRunner runner, PlayerRef player, NetworkInput input)
    // {}
    // public void OnShutdown (NetworkRunner runner, ShutdownReason shutdownReason)
    // {}   
    
    // public void OnConnectedToServer (NetworkRunner runner)
    // {
    //     Debug.Log("connected to server");
        
    // }    
    
    // public void OnDisconnectedFromServer (NetworkRunner runner)
    // {}    
    
    // public void OnConnectRequest (NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    // {}    
    
    // public void OnConnectFailed (NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    // {}    
    
    // public void OnUserSimulationMessage (NetworkRunner runner, SimulationMessagePtr message)
    // {}    
    
    // public void OnSessionListUpdated (NetworkRunner runner, List< SessionInfo > sessionList)
    // {}
    
    // public void OnCustomAuthenticationResponse (NetworkRunner runner, Dictionary< string, object > data)
    // {}    
    
    // public void OnHostMigration (NetworkRunner runner, HostMigrationToken hostMigrationToken)
    // {}    
    // public void OnReliableDataReceived (NetworkRunner runner, PlayerRef player, ArraySegment< byte > data)
    // {}
    // public void OnSceneLoadDone (NetworkRunner runner)
    // {}
    // public void OnSceneLoadStart (NetworkRunner runner)
    // {}

}
