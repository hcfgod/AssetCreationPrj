using UnityEngine;

namespace KCharacterControler
{
	public interface IRotationHandler
	{
		void Initialize(Character owner);
		void UpdateRotation(float deltaTime);
	}
}


