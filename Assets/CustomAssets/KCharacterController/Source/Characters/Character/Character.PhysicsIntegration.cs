using System;
using UnityEngine;

namespace KCharacterControler
{
	public partial class Character : MonoBehaviour
	{
		private ICharacterAutoSimulationHandler _autoSimulationHandler;
		private ICharacterPhysicsVolumeHandler _physicsVolumeHandler;

		/// <summary>
		/// Initializes physics-related handler collaborators.
		/// </summary>
		protected void InitializePhysicsHandlers()
		{
			if (_autoSimulationHandler == null)
			{
				_autoSimulationHandler = new DefaultCharacterAutoSimulationHandler();
				_autoSimulationHandler.Initialize(this, OnLateFixedUpdate);
			}

			if (_physicsVolumeHandler == null)
			{
				_physicsVolumeHandler = new DefaultCharacterPhysicsVolumeHandler();
				_physicsVolumeHandler.Initialize(this);
			}
		}

		/// <summary>
		/// Internal bridge so handlers can change Character's PhysicsVolume safely.
		/// </summary>
		internal void __SetPhysicsVolume_Internal(PhysicsVolume volume)
		{
			SetPhysicsVolume(volume);
		}
	}
}