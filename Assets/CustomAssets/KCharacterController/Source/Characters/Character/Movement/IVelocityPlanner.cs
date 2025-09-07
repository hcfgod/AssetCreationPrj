using UnityEngine;

namespace KCharacterControler
{
	public interface IVelocityPlanner
	{
		void Initialize(Character owner);
		Vector3 CalcDesiredVelocity(float deltaTime);
	}
}


