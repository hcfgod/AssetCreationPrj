using UnityEngine;

namespace KCharacterControler
{
	public interface IMovementModeHandler
	{
		void Initialize(Character owner);
		void UpdateWalking(float deltaTime);
		void UpdateFalling(float deltaTime);
		void UpdateFlying(float deltaTime);
		void UpdateSwimming(float deltaTime);
	}
}


