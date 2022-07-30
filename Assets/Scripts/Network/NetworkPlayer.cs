using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    [SerializeField] private Camera MinimapCamera;
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Camera.main.gameObject.SetActive(false);
            FindObjectOfType<Minimap>().GetComponent<Minimap>().SetPlayer(gameObject.transform);
        
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
