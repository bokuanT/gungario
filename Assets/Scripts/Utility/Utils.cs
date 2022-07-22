using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{   
    // function to return a random Vector3 within the spawnable area
    public static Vector3 GetRandomSpawnPoint(Player.Team team)
    {
        // temporary spawning within the centre box of map
        float[,] blueSpawns = new float[,] { { -40, 6 }, { -45, 15 }, { -15, 15 } };
        float[,] redSpawns = new float[,] { { 30, -9 }, { 36, 10 }, { 35, -29.5F } };

        int blueIndex = Random.Range(0, 3);
        int redIndex = Random.Range(0, 3);

        Vector3 blueSpawn = new Vector3(blueSpawns[blueIndex, 0], blueSpawns[blueIndex, 1]);
        Vector3 redSpawn = new Vector3(redSpawns[redIndex, 0], redSpawns[redIndex, 1]);
        Vector3 neutralSpawn = new Vector3(Random.Range(-11,6), Random.Range(-4,5), 0);

        if (team == Player.Team.Red)
            return redSpawn;
        else if (team == Player.Team.Blue)
            return blueSpawn;
        else
            return blueIndex == 0 ? redSpawn
                : blueIndex == 1 ? blueSpawn : neutralSpawn;
                    
    }

    /// <summary>
    /// Gets a cardinal direction UP, RIGHT, DOWN AND LEFT and returns an integer indicating the direction
    /// <param name="origin">The position of the body</param>
    /// <param name="point">The position of the point the body is facing</param>
    /// </summary>
    public static int getDirection(Vector2 origin, Vector2 point) {
        Vector2 lookDir = point - origin;
        float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg;
        //left is 180/-180, right is 0. top is 90, bottom is -90
        //return values: up is 0, right is 1, down is 2, left is 3
        if (angle >= 45f && angle < 135f) {
            return 0;
        } else if (angle < 45f && angle >= -45f) {
            return 1;
        } else if (angle < -45f && angle >= -135f) {
            return 2;
        } else {
            return 3;
        }
    }
}
