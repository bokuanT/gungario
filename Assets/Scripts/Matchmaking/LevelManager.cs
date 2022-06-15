using System.Collections;
using System.Collections.Generic;
using Fusion;
using Gungario.Utility;
using UnityEngine;

	public class LevelManager : NetworkSceneManagerBase
	{
        /*
        ============================= SOFTWARE DESIGN =============================
        SINGLETON PATTERN - only 1 level manager is required at any time per client
        ===========================================================================
        */        
		public static LevelManager Instance => Singleton<LevelManager>.Instance;

        public static int MENU_SCENE = 1;
		
		public static void LoadMenu()
		{
            // TO BE IMPLEMENTED?  
			// Instance.Runner.SetActiveScene(LOBBY_SCENE);
		}

		public static void LoadTrack(int sceneIndex)
		{
			Instance.Runner.SetActiveScene(sceneIndex);
		}
		
		protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
		{
			Debug.Log($"Loading scene {newScene}");

			PreLoadScene(newScene);

			List<NetworkObject> sceneObjects = new List<NetworkObject>();

			// if (newScene >= LOBBY_SCENE)
			// {
			// 	yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
			// 	Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
			// 	Debug.Log($"Loaded scene {newScene}: {loadedScene}");
			// 	sceneObjects = FindNetworkObjects(loadedScene, disable: false);
			// }

			finished(sceneObjects);

			// Delay one frame, so we're sure level objects has spawned locally
			yield return null;

			// // Now we can safely spawn karts
			// if (GameManager.CurrentTrack != null && newScene>LOBBY_SCENE)
			// {
			// 	if (Runner.GameMode == GameMode.Host)
			// 	{
			// 		foreach (var player in RoomPlayer.Players)
			// 		{
			// 			player.GameState = RoomPlayer.EGameState.GameCutscene;
			// 			GameManager.CurrentTrack.SpawnPlayer(Runner, player);
			// 		}
			// 	}
			// }

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
