using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperZoomScript : MonoBehaviour
{
    public Camera cam;

    public Player _player;

    public LaserSightLine _laser;

    // Update is called once per frame
    void Update()
    {
        if (_player.state == Player.State.Active && cam != null)
        {
            if (Input.GetMouseButton(1))
            {
                cam.orthographicSize = 8.0f;
                _laser.Activate();
            }
            else
            {
                cam.orthographicSize = 5.0f;
            }
        }
    }
}
