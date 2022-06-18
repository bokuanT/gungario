using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    //For Player::ApplyDamage
    public static Player Get(NetworkRunner runner, PlayerRef playerRef)
    {
        NetworkRunner[] objs = runner.GetComponents<NetworkRunner>();
        
        foreach (NetworkRunner r in objs)
        {
            GameObject gameobj = r.gameObject;
            if (gameobj.TryGetComponent(out Spawner spwn))
            {
                NetworkObject networkP = r.GetPlayerObject(playerRef);
                return networkP.gameObject.GetComponent<Player>();
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
