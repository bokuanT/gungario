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
