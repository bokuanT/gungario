using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    public override void Spawned()
    {
        Debug.Log("BP 1");
        if (Object.HasInputAuthority)
        {
            Debug.Log("BP 2");
            Local = this;

            Camera.main.gameObject.SetActive(false);
            Debug.Log("BP 3");

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

            GameObject Camera = gameObject.transform.Find("Camera").gameObject;
            Camera localCamera = Camera.GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            Debug.Log("Spawned remote player");
        }
        
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);

    }
}
