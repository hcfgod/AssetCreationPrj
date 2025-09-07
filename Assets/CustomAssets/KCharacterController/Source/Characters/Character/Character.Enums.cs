using UnityEngine;

namespace KCharacterControler
{
    public partial class Character : MonoBehaviour
    {
        #region ENUMS

        public enum MovementMode
        {
            /// <summary>
            /// Disables movement clearing velocity and any pending forces / impulsed on Character.
            /// </summary>
            
            None,
            
            /// <summary>
            /// Walking on a surface, under the effects of friction, and able to "step up" barriers. Vertical velocity is zero.
            /// </summary>
            
            Walking,
            
            /// <summary>
            /// Falling under the effects of gravity, after jumping or walking off the edge of a surface.
            /// </summary>
            
            Falling,
            
            /// <summary>
            /// Flying, ignoring the effects of gravity.
            /// </summary>
            
            Flying,
            
            /// <summary>
            /// Swimming through a fluid volume, under the effects of gravity and buoyancy.
            /// </summary>
            
            Swimming,
            
            /// <summary>
            /// User-defined custom movement mode, including many possible sub-modes.
            /// </summary>
            
            Custom
        }

        public enum RotationMode
        {
            /// <summary>
            /// Disable Character's rotation.
            /// </summary>
            
            None,
            
            /// <summary>
            /// Smoothly rotate the Character toward the direction of acceleration, using rotationRate as the rate of rotation change.
            /// </summary>
            
            OrientRotationToMovement,
            
            /// <summary>
            /// Smoothly rotate the Character toward camera's view direction, using rotationRate as the rate of rotation change.
            /// </summary>
            
            OrientRotationToViewDirection,
            
            /// <summary>
            /// Let root motion handle Character rotation.
            /// </summary>
            
            OrientWithRootMotion,
            
            /// <summary>
            /// User-defined custom rotation mode.
            /// </summary>
            
            Custom
        }

        #endregion
    }
}