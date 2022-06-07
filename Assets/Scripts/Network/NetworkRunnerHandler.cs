using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using System.Linq;
using Fusion.Photon.Realtime;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;

    private AuthenticationValues authValues;

    // Start is called before the first frame update
    void Start()
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner";

        // AutoHostOrClient: First person starting the game is the host
        // To be changed at a later date
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log("Networkrunner started");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        //checks if theres anything already in the scene
        var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneObjectProvider>().FirstOrDefault();

        if (sceneObjectProvider == null)
        {
            // Handle networked objects that already exit the scene
            sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs 
        {
            GameMode = gameMode,
            Address = address,  
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneObjectProvider = sceneObjectProvider  
        });
    }

    public void setAuthValues(AuthenticationValues values) {
        this.authValues = values;
    }
}
