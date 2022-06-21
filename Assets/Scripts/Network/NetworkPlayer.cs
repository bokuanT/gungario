using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Camera.main.gameObject.SetActive(false);

            // GameObject camera = gameObject.transform.Find("Camera").gameObject;
            // Debug.Log("BP 4");
            // if (camera != null)
            // {
            //     Camera cam = camera.AddComponent<Camera>() as Camera;
            //     cam.enabled = true;
            // }
        
            Debug.Log("Spawned local player");
        }
        else 
        {
            AudioListener audioListener = GetComponent<AudioListener>();
            audioListener.enabled = false;

            Transform CameraTmp = gameObject.transform.Find("Camera");
            GameObject Camera = null;
            if (CameraTmp != null)
                Camera = CameraTmp.gameObject;
            if (Camera != null)
            {
                Camera localCamera = Camera.GetComponentInChildren<Camera>();
                localCamera.enabled = false;
            }

            Debug.Log("Spawned remote player");
        }
        
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);

    }
}
