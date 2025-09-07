using UnityEngine;

namespace KCharacterControler
{
	public sealed class DefaultRotationHandler : IRotationHandler
	{
		private Character _owner;

		public void Initialize(Character owner)
		{
			_owner = owner;
		}

		public void UpdateRotation(float deltaTime)
		{
			var rotationMode = _owner.GetRotationMode();
			if (rotationMode == Character.RotationMode.None)
			{
				return;
			}
			else if (rotationMode == Character.RotationMode.OrientRotationToMovement)
			{
				bool shouldRemainVertical = _owner.IsWalking() || _owner.IsFalling();
				_owner.RotateTowards(_owner.GetMovementDirection(), deltaTime, shouldRemainVertical);
			}
			else if (rotationMode == Character.RotationMode.OrientRotationToViewDirection && _owner.camera != null)
			{
				bool shouldRemainVertical = _owner.IsWalking() || _owner.IsFalling();
				_owner.RotateTowards(_owner.cameraTransform.forward, deltaTime, shouldRemainVertical);
			}
			else if (rotationMode == Character.RotationMode.OrientWithRootMotion)
			{
				_owner.RotateWithRootMotion();
			}
			else if (rotationMode == Character.RotationMode.Custom)
			{
				_owner.CustomRotationMode(deltaTime);
			}
		}
	}
}


