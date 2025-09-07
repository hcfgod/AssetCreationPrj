using System;
using System.Collections;
using UnityEngine;

namespace KCharacterControler
{
	/// <summary>
	/// Default implementation that mirrors Character's previous coroutine-based auto-simulation behavior.
	/// </summary>
	public sealed class DefaultCharacterAutoSimulationHandler : ICharacterAutoSimulationHandler
	{
		private MonoBehaviour _owner;
		private Action _onLateFixedTick;
		private Coroutine _coroutine;

		public void Initialize(MonoBehaviour owner, Action onLateFixedTick)
		{
			_owner = owner;
			_onLateFixedTick = onLateFixedTick;
		}

		public void SetEnabled(bool enable)
		{
			if (_owner == null)
				return;

			if (enable)
			{
				if (_coroutine != null)
					_owner.StopCoroutine(_coroutine);

				_coroutine = _owner.StartCoroutine(LateFixedUpdate());
			}
			else
			{
				if (_coroutine != null)
					_owner.StopCoroutine(_coroutine);

				_coroutine = null;
			}
		}

		private IEnumerator LateFixedUpdate()
		{
			WaitForFixedUpdate waitTime = new WaitForFixedUpdate();
			while (true)
			{
				yield return waitTime;
				_onLateFixedTick?.Invoke();
			}
		}
	}
}