using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkSceneManagerBase
	{
        /*
        ============================= SOFTWARE DESIGN =============================
        SINGLETON PATTERN - only 1 level manager is required at any time per client
        ===========================================================================
        */        
        public static int MENU_SCENE = 0;
		public static int TESTGAME_SCENE = 1;
		public static int MAP1_SCENE = 2;
		private static LevelManager _instance;
		public static LevelManager Instance
		{
			get
			{
				if (_instance == null) _instance = FindObjectOfType<LevelManager>();
				return _instance;
			}
		}

		public static void LoadMenu()
		{
            // TO BE IMPLEMENTED?  
			// Instance.Runner.SetActiveScene(LOBBY_SCENE);
		}
		
		public static void LoadMap(int sceneIndex)
		{
			// upon creating new networkRunner, sets the new one as Instance.Runner.
			if (Instance.Runner == null) Instance.Initialize(Instance.GetActiveRunner());
			Instance.Runner.SetActiveScene(sceneIndex);
		}
		
		public static void LoadDeathmatch()
		{
			Instance.Runner.SetActiveScene(MAP1_SCENE);
		}

		public NetworkRunner GetActiveRunner()
		{
			return gameObject.GetComponent<NetworkRunner>();
		}

		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
		{
			Debug.Log($"Loading scene {newScene}");

			PreLoadScene(newScene);

			List<NetworkObject> sceneObjects = new List<NetworkObject>();

			if (newScene >= MENU_SCENE)
			{
				yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
				Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
				Debug.Log($"Loaded scene {newScene}: {loadedScene}");
				sceneObjects = FindNetworkObjects(loadedScene, disable: false);
			}

			finished(sceneObjects);

			// Delay one frame, so we're sure level objects has spawned locally
			yield return null;

			// Now we can safely spawn players
			if (newScene>MENU_SCENE)
			{
				if (Runner.GameMode == GameMode.Host || Runner.GameMode == GameMode.Single)
				{
					GameLauncher.Instance.SpawnPlayers();
				}
			}
			
			Invoke("PostLoadScene", 1);
		}

		private void PreLoadScene(int scene)
		{
			// preload things
			// fading in etc
		}
	
		private void PostLoadScene()
		{
			if (SceneManager.GetActiveScene().buildIndex != MENU_SCENE) SetManagers();
		}

		[Rpc(sources: RpcSources.All, targets: RpcTargets.All)]
		private void SetManagers()
		{
			GameObject gameModes = GameObject.Find("Scoreboard_canvas")
				.transform.Find("Gamemodes").gameObject;
			if (gameModes == null)
			{
				Debug.Log("Cannot find gamemode managers");
				return;
			}

			if (GameLauncher.Instance.gamemode == Gamemode.CP || Runner.GameMode == GameMode.Single)
			{
				GameObject ManagerCP = gameModes.transform.Find("ManagerControlPoints").gameObject;
				ManagerCP.SetActive(true);
				GameObject.Find("ControlPoint").SetActive(true);
				//GameObject man = gameModes.transform.Find("ManagerTDM").gameObject;
				//man.SetActive(false);
				//GameObject FFAManager = gameModes.transform.Find("ManagerFreeForAll").gameObject;
				//FFAManager.SetActive(false);
			}
			else if (GameLauncher.Instance.gamemode == Gamemode.FFA)
			{
				GameObject ManagerFFA = gameModes.transform.Find("ManagerFreeForAll").gameObject;
				ManagerFFA.SetActive(true);
				//GameObject ManagerTDM = gameModes.transform.Find("ManagerTDM").gameObject;
				//ManagerTDM.SetActive(false);
				//GameObject CPManager = gameModes.transform.Find("ManagerControlPoints").gameObject;
				//CPManager.SetActive(false);
				//GameObject.Find("ControlPoint").SetActive(false);	
			}
			else if (GameLauncher.Instance.gamemode == Gamemode.TDM)
			{
				GameObject ManagerTDM = gameModes.transform.Find("ManagerTDM").gameObject;
				ManagerTDM.SetActive(true);
				//gameModes.transform.Find("Manager").gameObject.SetActive(true);
				//GameObject CPManager = gameModes.transform.Find("ManagerControlPoints").gameObject;
				//CPManager.SetActive(false);
				//GameObject FFAMan = gameModes.transform.Find("ManagerFreeForAll").gameObject;
				//FFAMan.SetActive(false);
				//GameObject.Find("ControlPoint").SetActive(false);
			}
		}
	}
