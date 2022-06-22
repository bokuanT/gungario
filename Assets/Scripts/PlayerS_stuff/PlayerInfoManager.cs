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
            //Debug.Log("networkrunner: " + r.ActivePlayers);
            GameObject gameobj = r.gameObject;
/*            if (gameobj.TryGetComponent(out Spawner spwn))
            {

                return spwn.Get(playerRef);
            }*/
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
