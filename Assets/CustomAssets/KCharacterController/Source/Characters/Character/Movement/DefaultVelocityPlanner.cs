using UnityEngine;

namespace KCharacterControler
{
	public sealed class DefaultVelocityPlanner : IVelocityPlanner
	{
		private Character _owner;

		public void Initialize(Character owner)
		{
			_owner = owner;
		}

		public Vector3 CalcDesiredVelocity(float deltaTime)
		{
			// Current movement direction

			Vector3 movementDirection = Vector3.ClampMagnitude(_owner.GetMovementDirection(), 1.0f);

			// The desired velocity from animation (if using root motion) or from input movement vector

			Vector3 desiredVelocity = _owner.useRootMotion && _owner.rootMotionController
				? _owner.rootMotionController.ConsumeRootMotionVelocity(deltaTime)
				: movementDirection * _owner.GetMaxSpeed();

			// Return constrained desired velocity

			return _owner.ConstrainInputVector(desiredVelocity);
		}
	}
}


