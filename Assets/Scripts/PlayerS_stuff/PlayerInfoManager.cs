using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// PlayerInfoManager gets created when a game session starts and exists in only one instance.
/// It survives scene loading and can be used to control game-logic inside a session, across scenes.
/// </summary>


public class PlayerInfoManager : NetworkBehaviour
{
    //private HashSet<PlayerRef> _finishedLoading = new HashSet<PlayerRef>();

    //[Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    //public void RPC_FinishedLoading(PlayerRef playerRef)
    //{
    //    _finishedLoading.Add(playerRef);
    //}

    public override void Spawned()
    {
        GameLauncher.Instance.PlayerInfoManager = this;
        base.Spawned();
    }


    //For Player::ApplyDamage
    public static Player Get(NetworkRunner runner, PlayerRef playerRef)
    {
        NetworkRunner[] objs = runner.GetComponents<NetworkRunner>();
        
        foreach (NetworkRunner r in objs)
        {
            //Debug.Log("networkrunner: " + r.ActivePlayers);
            GameObject gameobj = r.gameObject;
            if (gameobj.TryGetComponent(out GameLauncher launcher))
            {

                return launcher.Get(playerRef);
            }
        }

        return null;
    }

    //for Scoreboard initialisation
    public static IEnumerable<PlayerRef> GetAllPlayerRefs(NetworkRunner runner)
    {
        return runner.ActivePlayers;
    }
}
