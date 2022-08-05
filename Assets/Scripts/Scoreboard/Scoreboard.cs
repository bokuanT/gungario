using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UIElements;
using Fusion.Sockets;
using System;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboard_item_prefab;

   
    private Dictionary<PlayerRef, Scoreboard_item> hashtable = 
        new Dictionary<PlayerRef, Scoreboard_item>();

    private HashSet<int> hashSet = new HashSet<int>();
    private HashSet<int> cosmeticsLoaded = new HashSet<int>();

    [Networked]
    public int redScore { get; set; }

    [Networked]
    public int blueScore { get; set; }

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
            // dont load until name is set
            string name = GameLauncher.Instance.GetPlayerInfo(pr).Name.Value;
            if (!hashSet.Contains(pr.PlayerId) && name != "") 
            {
                // set name in Player script
                NetworkObject obj = Runner.GetPlayerObject(pr);
                Player player = obj.gameObject.GetComponent<Player>();
                player.playerName = name;

                Debug.Log("creating item for " + pr);
                hashSet.Add(pr.PlayerId);
                Scoreboard_item item = CreateScoreboardItem(pr);
                hashtable.Add(pr, item);
            }

            // load cosmetics if received 
            int id = GameLauncher.Instance.GetPlayerInfo(pr).CosmeticHatID;
            if (!cosmeticsLoaded.Contains(pr.PlayerId) && id != 0)
            {
                Debug.Log("Loading hat");
                NetworkObject obj = Runner.GetPlayerObject(pr);
                Player player = obj.gameObject.GetComponent<Player>();
                player.hatSprite.sprite = Inventory.Instance.GetHat(id);
                cosmeticsLoaded.Add(pr.PlayerId);
            }
            
        }

    }

    private void UpdateAllKD()
    {
        int red = 0;
        int blue = 0;

        foreach (Scoreboard_item item in hashtable.Values)
        {
            item.UpdateKD();
            if (item.IsRedTeam())
                red += item.kills;
            if (item.IsBlueTeam())
                blue += item.kills;
        }
        redScore = red;
        blueScore = blue;
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

    public void ResetScore()
    {
        foreach (Scoreboard_item item in hashtable.Values)
        {
            item.Reset();
        }
    }

    public Player[] GetAllScoreboardPlayers()
    {
        int size = hashtable.Values.Count;
        Player[] ret = new Player[size];
        int i = 0;
        foreach (Scoreboard_item item in hashtable.Values)
        {
            ret[i] = item.player;
            i++;
        }
        return ret;
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
