using Fusion;
using PlayFab.ClientModels;
using UnityEngine;

/// <summary>
/// Player is a network object that represents a players core data. One instance is spawned
/// for each player when the game session starts and it lives until the session ends.
/// This is not the visual representation of the player.
/// </summary>

public class PlayerInfo : NetworkBehaviour
{
	//[SerializeField] public Character CharacterPrefab;
	[Networked] public NetworkString<_32> Name { get; set; }
	// [Networked] public NetworkBool InputEnabled { get; set; }

	public override void Spawned()
	{
		base.Spawned();
		GameLauncher.Instance.SetPlayerInfo(Object.InputAuthority, this);
		if (Object.HasInputAuthority)
        {
			// if the peer's object spawns on the peer's client, set the name  
			PlayerProfileModel profile = GameManager.Instance.GetPlayerProfile();
			Name = profile.DisplayName;
			RPC_SetName(Name);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetName(NetworkString<_32> name)
	{
		Name = name;
	}

}