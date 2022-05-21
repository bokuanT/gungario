using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{   
    // function to return a random Vector3 within the spawnable area
    public static Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-8,8), Random.Range(-8,8), 0);
    }
}