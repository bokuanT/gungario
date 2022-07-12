using System.Collections;
using System.Collections.Generic;
using Fusion;
using Gungario.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

	public class LevelManager : NetworkSceneManagerBase
	{
        /*
        ============================= SOFTWARE DESIGN =============================
        SINGLETON PATTERN - only 1 level manager is required at any time per client
        ===========================================================================
        */        
		public static LevelManager Instance => Singleton<LevelManager>.Instance;
		public GameLauncher gameLauncher;
        public static int MENU_SCENE = 0;
		public static int TESTGAME_SCENE = 1;
		public static int MAP1_SCENE = 2;
		
		public static void LoadMenu()
		{
            // TO BE IMPLEMENTED?  
			// Instance.Runner.SetActiveScene(LOBBY_SCENE);
		}

		public static void LoadMap(int sceneIndex)
		{
			Instance.Runner.SetActiveScene(sceneIndex);
		}
		
		public static void LoadDeathmatch()
		{
			Instance.Runner.SetActiveScene(MAP1_SCENE);
		}

		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
		{
			Debug.Log($"Loading scene {newScene}");

			PreLoadScene(newScene);

			List<NetworkObject> sceneObjects = new List<NetworkObject>();

			if (newScene > MENU_SCENE)
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
			SetManagers();
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
			if (gameLauncher.gamemode == 0)
			{
				GameObject otherMan = gameModes.transform.Find("Manager").gameObject;
				otherMan.SetActive(false);
				GameObject correctManager = gameModes.transform.Find("ManagerControlPoints").gameObject;
				correctManager.SetActive(false);
				GameObject.Find("ControlPoint").SetActive(false);
			}

			if (gameLauncher.gamemode == 1)
			{
				GameObject otherMan = gameModes.transform.Find("Manager").gameObject;
				otherMan.SetActive(false);
				GameObject FFAMan = gameModes.transform.Find("ManagerFreeForAll").gameObject;
				FFAMan.SetActive(false);

			}
			if (gameLauncher.gamemode == 2)
			{
				//gameModes.transform.Find("Manager").gameObject.SetActive(true);
				GameObject correctManager = gameModes.transform.Find("ManagerControlPoints").gameObject;
				correctManager.SetActive(false);
				GameObject.Find("ControlPoint").SetActive(false);
				GameObject FFAMan = gameModes.transform.Find("ManagerFreeForAll").gameObject;
				FFAMan.SetActive(false);
			}
		}
	}
