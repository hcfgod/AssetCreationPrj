using System.Collections.Generic;
using UnityEngine;

namespace KCharacterControler
{
	/// <summary>
	/// Default implementation that mirrors Character's internal volume list and selection logic.
	/// </summary>
	public sealed class DefaultCharacterPhysicsVolumeHandler : ICharacterPhysicsVolumeHandler
	{
		private Character _owner;
		private readonly List<PhysicsVolume> _volumes = new List<PhysicsVolume>();

		public void Initialize(Character owner)
		{
			_owner = owner;
		}

		public void Add(Collider other)
		{
			if (other.TryGetComponent(out PhysicsVolume volume) && !_volumes.Contains(volume))
				_volumes.Insert(0, volume);
		}

		public void Remove(Collider other)
		{
			if (other.TryGetComponent(out PhysicsVolume volume) && _volumes.Contains(volume))
				_volumes.Remove(volume);
		}

		public void UpdateVolumes()
		{
			PhysicsVolume best = null;
			int maxPriority = int.MinValue;
			for (int i = 0, c = _volumes.Count; i < c; i++)
			{
				PhysicsVolume vol = _volumes[i];
				if (vol.priority <= maxPriority)
					continue;

				maxPriority = vol.priority;
				best = vol;
			}

			UpdatePhysicsVolume(best);
		}

		private void UpdatePhysicsVolume(PhysicsVolume newPhysicsVolume)
		{
			if (_owner == null)
				return;

			Vector3 characterCenter = _owner.characterMovement.worldCenter;
			if (newPhysicsVolume && newPhysicsVolume.boxCollider.ClosestPoint(characterCenter) == characterCenter)
			{
				_owner.__SetPhysicsVolume_Internal(newPhysicsVolume);
			}
			else
			{
				_owner.__SetPhysicsVolume_Internal(null);
			}
		}
	}
}