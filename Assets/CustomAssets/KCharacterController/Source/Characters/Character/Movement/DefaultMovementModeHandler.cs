using UnityEngine;

namespace KCharacterControler
{
	public sealed class DefaultMovementModeHandler : IMovementModeHandler
	{
		private Character _owner;

		public void Initialize(Character owner)
		{
			_owner = owner;
		}

		public void UpdateWalking(float deltaTime)
		{
			// If using root motion, use animation velocity

			if (_owner.useRootMotion && _owner.rootMotionController)
				_owner.characterMovement.velocity = _owner.GetDesiredVelocity();
			else
			{
				// Calculate new velocity

				_owner.characterMovement.velocity =
					_owner.CalcVelocity(_owner.characterMovement.velocity, _owner.GetDesiredVelocity(), _owner.groundFriction, false, deltaTime);
			}
			
			// Apply downwards force

			if (_owner.applyStandingDownwardForce)
				_owner.ApplyDownwardsForce();
		}

		public void UpdateFalling(float deltaTime)
		{
			// Current target velocity

			Vector3 desiredVelocity = _owner.GetDesiredVelocity();
			
			// World-up defined by gravity direction

			Vector3 worldUp = -_owner.GetGravityDirection();
			
			// On not walkable ground...

			if (_owner.IsOnGround() && !_owner.IsOnWalkableGround())
			{
				// If moving into the 'wall', limit contribution

				Vector3 groundNormal = _owner.characterMovement.groundNormal;

				if (Vector3.Dot(desiredVelocity, groundNormal) < 0.0f)
				{
					// Allow movement parallel to the wall, but not into it because that may push us up

					Vector3 groundNormal2D = Vector3.ProjectOnPlane(groundNormal, worldUp).normalized;
					desiredVelocity = Vector3.ProjectOnPlane(desiredVelocity, groundNormal2D);
				}

				// Make velocity calculations planar by projecting the up vector into non-walkable surface

				worldUp = Vector3.ProjectOnPlane(worldUp, groundNormal).normalized;
			}

			// Separate velocity into its components

			Vector3 verticalVelocity = Vector3.Project(_owner.characterMovement.velocity, worldUp);
			Vector3 lateralVelocity = _owner.characterMovement.velocity - verticalVelocity;

			// Update lateral velocity

			lateralVelocity = _owner.CalcVelocity(lateralVelocity, desiredVelocity, _owner.fallingLateralFriction, false, deltaTime);

			// Update vertical velocity

			verticalVelocity += _owner.gravity * deltaTime;

			// Don't exceed terminal velocity.

			float actualFallSpeed = _owner.maxFallSpeed;
			if (_owner.physicsVolume)
				actualFallSpeed = _owner.physicsVolume.maxFallSpeed;

			if (Vector3.Dot(verticalVelocity, worldUp) < -actualFallSpeed)
				verticalVelocity = Vector3.ClampMagnitude(verticalVelocity, actualFallSpeed);

			// Apply new velocity

			_owner.characterMovement.velocity = lateralVelocity + verticalVelocity;

			// Update falling timer
			_owner.AddFallingTime(deltaTime) ;
		}

		public void UpdateFlying(float deltaTime)
		{
			if (_owner.useRootMotion && _owner.rootMotionController)
				_owner.characterMovement.velocity = _owner.GetDesiredVelocity();
			else
			{
				float actualFriction = _owner.IsInWaterPhysicsVolume() ? _owner.physicsVolume.friction : _owner.flyingFriction;

				_owner.characterMovement.velocity
					= _owner.CalcVelocity(_owner.characterMovement.velocity, _owner.GetDesiredVelocity(), actualFriction, true, deltaTime);
			}
		}

		public void UpdateSwimming(float deltaTime)
		{
			// Compute actual buoyancy factoring current immersion depth
			
			float depth = _owner.CalcImmersionDepth();
			float actualBuoyancy = _owner.buoyancy * depth;
			
			// Calculate new velocity

			Vector3 desiredVelocity = _owner.GetDesiredVelocity();
			Vector3 newVelocity = _owner.characterMovement.velocity;
			
			Vector3 worldUp = -_owner.GetGravityDirection();
			float verticalSpeed = Vector3.Dot(newVelocity, worldUp);
			
			if (verticalSpeed > _owner.maxSwimSpeed * 0.33f && actualBuoyancy > 0.0f)
			{
				// Damp positive vertical speed (out of water)

				verticalSpeed = Mathf.Max(_owner.maxSwimSpeed * 0.33f, verticalSpeed * depth * depth);
				newVelocity = Vector3.ProjectOnPlane(newVelocity, worldUp) + worldUp * verticalSpeed;
			}
			else if (depth < 0.65f)
			{
				// Damp positive vertical desired speed

				float verticalDesiredSpeed = Vector3.Dot(desiredVelocity, worldUp);
				
				desiredVelocity =
					Vector3.ProjectOnPlane(desiredVelocity, worldUp) + worldUp * Mathf.Min(0.1f, verticalDesiredSpeed);
			}
			
			// Using root motion...

			if (_owner.useRootMotion && _owner.rootMotionController)
			{
				// Preserve current vertical velocity as we want to keep the effect of gravity

				Vector3 verticalVelocity = Vector3.Project(newVelocity, worldUp);

				// Updates new velocity

				newVelocity = Vector3.ProjectOnPlane(desiredVelocity, worldUp) + verticalVelocity;
			}
			else
			{
				// Actual friction

				float actualFriction = _owner.IsInWaterPhysicsVolume()
					? _owner.physicsVolume.friction * depth
					: _owner.swimmingFriction * depth;

				newVelocity = _owner.CalcVelocity(newVelocity, desiredVelocity, actualFriction, true, deltaTime);
			}

			// If swimming freely, apply gravity acceleration scaled by (1.0f - actualBuoyancy)

			newVelocity += _owner.gravity * ((1.0f - actualBuoyancy) * deltaTime);

			// Update velocity

			_owner.characterMovement.velocity = newVelocity;
		}
	}
}


