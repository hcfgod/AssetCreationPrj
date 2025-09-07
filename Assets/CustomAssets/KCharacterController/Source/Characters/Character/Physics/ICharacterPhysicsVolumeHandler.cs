using UnityEngine;

namespace KCharacterControler
{
	/// <summary>
	/// Encapsulates PhysicsVolume tracking and updates for a Character.
	/// </summary>
	public interface ICharacterPhysicsVolumeHandler
	{
		void Initialize(Character owner);
		void Add(Collider other);
		void Remove(Collider other);
		void UpdateVolumes();
	}
}