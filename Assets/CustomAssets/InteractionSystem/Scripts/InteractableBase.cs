using CustomAssets.EditorTools;
using UnityEngine;

/** Summary:
 * The InteractableBase class is an abstract base class for all interactable objects in the interaction system.
 * It implements the IInteractable interface and provides default implementations for interaction methods.
 * Derived classes can override these methods to provide specific interaction behaviors.
 */
namespace CustomAssets.InteractionSystem
{
    public enum InteractionType
    {
        Instant,
        Hold,
        Toggle,
        Trigger,
        Custom
    }

    public enum PostInteractionAction
    {
        None,
        Reset,
        Disable,
        Destroy,
        Custom
    }

    public enum InteractionState
    {
        Idle,
        Interacting,
        Completed,
        Cancelled
    }

    /// <summary>
    /// Base class for interactable objects in the interaction system.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Title("Interaction Settings")]
        [Tooltip("How this interactable behaves when used.")]
        [SerializeField] private InteractionType _interactionType = InteractionType.Instant;
        [Tooltip("What happens to this interactable after a completed interaction.")]
        [SerializeField] private PostInteractionAction _postAction = PostInteractionAction.None;
        [Tooltip("Duration (seconds) the input must be held when InteractionType is Hold.")]
        [SerializeField] private float _holdDuration = 0.5f;

        [Title("State (runtime)")]
        [ReadOnly("This is just to see the state in the inspector.")]
        [SerializeField, Tooltip("Current interaction state. Managed at runtime.")]
        private InteractionState _state = InteractionState.Idle;

        /// <summary>
        /// Interaction behavior for this interactable.
        /// </summary>
        public virtual InteractionType InteractionMode => _interactionType;
        /// <summary>
        /// Post-interaction action for this interactable when a cycle completes.
        /// </summary>
        public virtual PostInteractionAction PostAction => _postAction;
        /// <summary>
        /// For Hold interaction type, how long the hold must last (seconds).
        /// </summary>
        public virtual float HoldDuration => _holdDuration;
        /// <summary>
        /// Current state of this interactable.
        /// </summary>
        public virtual InteractionState State
        {
            get => _state;
            protected set => _state = value;
        }

        protected virtual void OnEnable()
        {
            // Auto-register with the global manager on enable
            var mgr = InteractableManager.Instance;
            if (mgr != null) mgr.RegisterInteractable(this);
        }

        protected virtual void OnDisable()
        {
            // Unregister on disable; tolerate manager shutdown
            var mgr = InteractableManager.Instance;
            if (mgr != null) mgr.UnregisterInteractable(this);
        }

        // If this object itself has a trigger collider, auto-fire Trigger interactions on enter
        private void OnTriggerEnter(Collider other)
        {
            if (InteractionMode != InteractionType.Trigger) return;

            var handler = other.GetComponentInParent<InteractionHandler>();
            if (handler == null) return;

            var data = new InteractionData { Interactor = handler.gameObject, InteractionTarget = gameObject };

            BeginInteraction(data);
            CompleteInteraction(data);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (InteractionMode != InteractionType.Trigger) return;

            var handler = other.GetComponentInParent<InteractionHandler>();
            if (handler == null) return;

            var data = new InteractionData { Interactor = handler.gameObject, InteractionTarget = gameObject };

            BeginInteraction(data);
            CompleteInteraction(data);
        }

        /// <summary>
        /// Called when an interaction starts.
        /// </summary>
        /// <param name="interactionData">Data about the interaction.</param>
        public virtual void OnInteractStart(InteractionData interactionData)
        {
            Debug.Log($"{gameObject.name} interacted with by {interactionData.Interactor.name}");
        }
        /// <summary>
        /// Called to report progress of an ongoing interaction.
        /// </summary>
        /// <param name="interactionData">Data about the interaction.</param>
        /// <param name="progress">Progress of the interaction (0.0 to 1.0).
        /// Note: For Toggle/Trigger/Instant, progress is typically 1.
        /// </param>
        public virtual void OnInteractProgress(InteractionData interactionData, float progress)
        {
            Debug.Log($"{gameObject.name} interaction progress: {progress * 100}%");
        }
        /// <summary>
        /// Called when an interaction ends.
        /// </summary>
        /// <param name="interactionData">Data about the interaction.</param>
        public virtual void OnInteractEnd(InteractionData interactionData)
        {
            Debug.Log($"{gameObject.name} interaction ended with {interactionData.Interactor.name}");
        }

        /// <summary>
        /// Helper to begin an interaction cycle and update state.
        /// </summary>
        public virtual void BeginInteraction(InteractionData data)
        {
            State = InteractionState.Interacting;
            OnInteractStart(data);
        }
        /// <summary>
        /// Helper to report progress during an interaction.
        /// </summary>
        public virtual void ReportProgress(InteractionData data, float progress)
        {
            OnInteractProgress(data, progress);
        }
        /// <summary>
        /// Helper to complete an interaction cycle and apply post action.
        /// </summary>
        public virtual void CompleteInteraction(InteractionData data)
        {
            OnInteractEnd(data);
            State = InteractionState.Completed;
            DoPostAction();
        }
        /// <summary>
        /// Helper to cancel an interaction cycle.
        /// </summary>
        public virtual void CancelInteraction(InteractionData data)
        {
            OnInteractEnd(data);
            State = InteractionState.Cancelled;
        }

        /// <summary>
        /// Reset state to Idle. Override for additional reset behavior.
        /// </summary>
        public virtual void ResetInteraction()
        {
            State = InteractionState.Idle;
        }

        /// <summary>
        /// Applies the configured post-interaction action.
        /// </summary>
        protected virtual void DoPostAction()
        {
            switch (PostAction)
            {
                case PostInteractionAction.None:
                    break;
                case PostInteractionAction.Reset:
                    ResetInteraction();
                    break;
                case PostInteractionAction.Disable:
                    gameObject.SetActive(false);
                    break;
                case PostInteractionAction.Destroy:
                    Destroy(gameObject);
                    break;
                case PostInteractionAction.Custom:
                    OnPostInteractionCustom();
                    break;
            }
        }

        /// <summary>
        /// Extend for custom post-interaction behaviors.
        /// </summary>
        protected virtual void OnPostInteractionCustom() { }
    }
}
