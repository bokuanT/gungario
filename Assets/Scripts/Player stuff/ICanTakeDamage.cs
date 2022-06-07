using Fusion;
using UnityEngine;


/// <summary>
/// Interface implemented by any gameobject that can be damaged.
/// </summary>
public interface ICanTakeDamage
{
	void ApplyDamage(Vector3 impulse, byte damage, PlayerRef source);
}

