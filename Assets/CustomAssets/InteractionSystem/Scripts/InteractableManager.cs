using System.Collections.Generic;
using UnityEngine;

/** Summary:
 * The InteractableManager class is a singleton that tracks interactable objects in the scene.
 * It provides methods to register, unregister, and enumerate interactable objects.
 * Spatial queries (finding/aiming/selection) are handled by InteractionHandler.
 */

namespace CustomAssets.InteractionSystem
{
    public class InteractableManager : Singleton<InteractableManager>
    {
        private readonly List<IInteractable> _interactables = new List<IInteractable>();

        protected override void Awake()
        {
            base.Awake();
        }

        public void RegisterInteractable(IInteractable interactable)
        {
            if (_interactables.Contains(interactable)) return;

            _interactables.Add(interactable);
        }

        public void UnregisterInteractable(IInteractable interactable)
        {
            if (!_interactables.Contains(interactable)) return;

            _interactables.Remove(interactable);
        }


        public List<IInteractable> GetAllInteractables()
        {
            return _interactables;
        }
    }
}
