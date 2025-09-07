using System;
using UnityEngine;

namespace KCharacterControler
{
	/// <summary>
	/// Handles Character auto-simulation lifecycle (start / stop coroutine and tick callback).
	/// </summary>
	public interface ICharacterAutoSimulationHandler
	{
		/// <summary>
		/// Initialize the handler with the owning MonoBehaviour and the tick callback.
		/// </summary>
		/// <param name="owner">Owner MonoBehaviour used to start/stop coroutines.</param>
		/// <param name="onLateFixedTick">Callback invoked every FixedUpdate cycle when enabled.</param>
		void Initialize(MonoBehaviour owner, Action onLateFixedTick);

		/// <summary>
		/// Enable or disable auto-simulation ticking.
		/// </summary>
		/// <param name="enable">True to enable, false to disable.</param>
		void SetEnabled(bool enable);
	}
}