using Fusion;
using PlayFab.ClientModels;

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
		if (Object.InputAuthority)
        {
			PlayerProfileModel profile = GameManager.Instance.GetPlayerProfile();
			Name = profile.DisplayName;
			GameLauncher.Instance.SetPlayer(Object.InputAuthority, this);
			// RPC_SetName(profile.DisplayName);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetName(NetworkString<_32> name)
	{
		Name = name;
	}

}