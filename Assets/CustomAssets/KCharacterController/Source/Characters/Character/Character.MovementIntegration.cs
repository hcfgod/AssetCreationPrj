using UnityEngine;

namespace KCharacterControler
{
	public partial class Character : MonoBehaviour
	{
		private IMovementModeHandler _movementModeHandler;
		private IRotationHandler _rotationHandler;
		private IVelocityPlanner _velocityPlanner;

		protected void InitializeMovementHandlers()
		{
			if (_movementModeHandler == null)
			{
				_movementModeHandler = new DefaultMovementModeHandler();
				_movementModeHandler.Initialize(this);
			}

			if (_rotationHandler == null)
			{
				_rotationHandler = new DefaultRotationHandler();
				_rotationHandler.Initialize(this);
			}

			if (_velocityPlanner == null)
			{
				_velocityPlanner = new DefaultVelocityPlanner();
				_velocityPlanner.Initialize(this);
			}
		}
	}
}


