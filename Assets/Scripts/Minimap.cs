using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private Transform player;

    public void SetPlayer(Transform ply)
    {
        player = ply;
    }
    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = player.position.z - 10;
            transform.position = newPosition;
        }
    }
}
