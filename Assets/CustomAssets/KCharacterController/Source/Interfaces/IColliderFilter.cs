using UnityEngine;

namespace KCharacterControler
{
    public interface IColliderFilter
    {
        /// <summary>
        /// Determines if the given collider should be filtered (eg: ignored).
        /// Return true if should be filtered, false otherwise.
        /// </summary>

        bool Filter(Collider otherCollider);
    }
}