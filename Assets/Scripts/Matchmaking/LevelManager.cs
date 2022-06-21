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
			if (Instance.Runner == null) Debug.Log("This is the problem");
			// Instance.Runner is null for some reason
			Instance.Runner.SetActiveScene(MAP1_SCENE);
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
				if (Runner.GameMode == GameMode.Host)
				{
					gameLauncher.SpawnPlayers();
				}
			}

			PostLoadScene();
		}

		private void PreLoadScene(int scene)
		{
            // preload things
            // fading in etc
		}
	
		private void PostLoadScene()
		{
			// post loading players and all
            // match start sequence
			
		}
	}
