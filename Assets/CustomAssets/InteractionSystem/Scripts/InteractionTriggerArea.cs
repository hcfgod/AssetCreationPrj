using UnityEngine;

namespace CustomAssets.InteractionSystem
{
    /// <summary>
    /// Place this on a GameObject with a trigger Collider or Collider2D to define a custom trigger area
    /// for an InteractableBase set to InteractionType.Trigger. When the player (with InteractionHandler)
    /// enters this trigger, the interaction auto-fires.
    /// </summary>
    [AddComponentMenu("CustomAssets/InteractionSystem/Interaction Trigger Area")]
    public class InteractionTriggerArea : MonoBehaviour
    {
        [Tooltip("The interactable this trigger area belongs to. If not set, will search in parents.")]
        [SerializeField] private InteractableBase _interactable;

        private void Reset()
        {
            if (_interactable == null) _interactable = GetComponentInParent<InteractableBase>();
            var c3d = GetComponent<Collider>();
            if (c3d != null) c3d.isTrigger = true;
            var c2d = GetComponent<Collider2D>();
            if (c2d != null) c2d.isTrigger = true;
        }

        private void Awake()
        {
            if (_interactable == null) _interactable = GetComponentInParent<InteractableBase>();
        }

        private void OnTriggerEnter(Collider other)
        {
            TryTrigger(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryTrigger(other.gameObject);
        }

        private void TryTrigger(GameObject otherGO)
        {
            if (_interactable == null) return;
            if (_interactable.InteractionMode != InteractionType.Trigger) return;

            var handler = otherGO.GetComponentInParent<InteractionHandler>();
            if (handler == null) return;

            var data = new InteractionData
            {
                Interactor = handler.gameObject,
                InteractionTarget = _interactable.gameObject
            };

            _interactable.BeginInteraction(data);
            _interactable.CompleteInteraction(data);
        }
    }
}
