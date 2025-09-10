using System.Collections.Generic;
using UnityEngine;
using CustomAssets.EditorTools;

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
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Title("Interaction Settings")]
        [Tooltip("How this interactable behaves when used.")]
        [TabGroup("Interaction", "Interaction Enums")]
        [SerializeField] private InteractionType _interactionType = InteractionType.Instant;

        [Tooltip("What happens to this interactable after a completed interaction.")]
        [TabGroup("Interaction", "Interaction Enums")]
        [SerializeField] private PostInteractionAction _postAction = PostInteractionAction.None;

        [Tooltip("Duration (seconds) the input must be held when InteractionType is Hold.")]
        [TabGroup("Interaction", "Interaction Settings")]
        [SerializeField] private float _holdDuration = 0.5f;

        [Title("Trigger Settings")]
        [Tooltip("Cooldown before this Trigger interactable can be triggered again. Only used when Interaction Mode is Trigger.")]
        [TabGroup("Trigger", "Trigger Settings")]
        [SerializeField] private float _retriggerDelay = 0f;

        [Title("Trigger Filters")]
        [Tooltip("Only objects on these layers can trigger the interaction.")]
        [TabGroup("Trigger", "Trigger Filters")]
        [SerializeField] private LayerMask _allowedLayers = ~0;

        [Tooltip("If enabled, only objects with this tag can trigger the interaction.")]
        [TabGroup("Trigger", "Trigger Filters")]
        [SerializeField] private bool _useTagFilter = false;

        [Tooltip("Required tag for objects to trigger the interaction.")]
        [TabGroup("Trigger", "Trigger Filters")]
        [SerializeField] private string _requiredTag = "Player";

        [Tooltip("Require the entering object (or its parent) to have an InteractionHandler component.")]
        [TabGroup("Trigger", "Trigger Filters")]
        [SerializeField] private bool _requireInteractionHandler = true;

        [Title("Stay Triggering (optional)")]
        [Tooltip("If enabled, will attempt to re-trigger while the object stays inside the trigger.")]
        [TabGroup("Trigger", "Stay Triggering")]
        [SerializeField] private bool _retriggerOnStay = false;
        [Tooltip("Minimum interval between stay re-trigger attempts per object (seconds). 0 uses only the interactable's RetriggerDelay.")]
        [MinValue(0.0f)]
        [TabGroup("Trigger", "Stay Triggering")]
        [SerializeField] private float _stayRepeatInterval = 0f;

        [Title("State (runtime)")]
        [ReadOnly("This is just to see the state in the inspector.")]
        [SerializeField, Tooltip("Current interaction state. Managed at runtime.")]
        private InteractionState _state = InteractionState.Idle;

        private float _lastTriggerTime = float.NegativeInfinity;
        private readonly Dictionary<int, float> _lastStayAttemptTimes = new Dictionary<int, float>();

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
        /// Cooldown before this Trigger interactable can be triggered again.
        /// </summary>
        public virtual float RetriggerDelay => _retriggerDelay;
        /// <summary>
        /// Only objects on these layers can trigger the interaction.
        /// </summary>
        public virtual LayerMask AllowedLayers => _allowedLayers;
        /// <summary>
        /// If enabled, only objects with the required tag can trigger the interaction.
        /// </summary>
        public virtual bool UseTagFilter => _useTagFilter;
        /// <summary>
        /// Required tag for objects to trigger the interaction.
        /// </summary>
        public virtual string RequiredTag => _requiredTag;
        /// <summary>
        /// Require the entering object to have an InteractionHandler component.
        /// </summary>
        public virtual bool RequireInteractionHandler => _requireInteractionHandler;
        /// <summary>
        /// If enabled, will attempt to re-trigger while the object stays inside the trigger.
        /// </summary>
        public virtual bool RetriggerOnStay => _retriggerOnStay;
        /// <summary>
        /// Minimum interval between stay re-trigger attempts per object (seconds).
        /// </summary>
        public virtual float StayRepeatInterval => _stayRepeatInterval;
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
            TryAutoTrigger(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryAutoTrigger(other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            TryStayTrigger(other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryStayTrigger(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            CleanupStay(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CleanupStay(other.gameObject);
        }

        /// <summary>
        /// Attempts to auto-trigger this interactable when in Trigger mode.
        /// Respects RetriggerDelay and applies all configured filters.
        /// </summary>
        public virtual bool TryAutoTrigger(GameObject interactorGO)
        {
            if (InteractionMode != InteractionType.Trigger) return false;
            if (interactorGO == null) return false;
            if (!PassesFilters(interactorGO)) return false;

            // Use <= to avoid same-frame double trigger when delay is 0
            if (Time.time <= _lastTriggerTime + RetriggerDelay) return false;

            var handler = interactorGO.GetComponentInParent<InteractionHandler>();
            if (handler == null) return false;

            var data = new InteractionData { Interactor = handler.gameObject, InteractionTarget = gameObject };

            BeginInteraction(data);
            CompleteInteraction(data);

            _lastTriggerTime = Time.time;
            return true;
        }

        /// <summary>
        /// Attempts to re-trigger this interactable while an object stays in the trigger.
        /// Respects StayRepeatInterval and applies all configured filters.
        /// </summary>
        public virtual bool TryStayTrigger(GameObject interactorGO)
        {
            if (InteractionMode != InteractionType.Trigger) return false;
            if (interactorGO == null) return false;
            if (!PassesFilters(interactorGO)) return false;
            if (!RetriggerOnStay) return false;

            // Always respect the global retrigger delay to prevent double-triggering
            // Use <= to avoid same-frame double trigger when delay is 0
            if (Time.time <= _lastTriggerTime + RetriggerDelay) return false;

            int instanceId = interactorGO.GetInstanceID();
            bool hasLastAttempt = _lastStayAttemptTimes.TryGetValue(instanceId, out float lastAttempt);
            float interval = StayRepeatInterval > 0f ? StayRepeatInterval : RetriggerDelay;

            // First Stay after entering should not trigger; initialize tracking and return
            if (!hasLastAttempt)
            {
                _lastStayAttemptTimes[instanceId] = Time.time;
                return false;
            }

            // Use <= to avoid same-frame double trigger when interval is 0
            if (Time.time <= lastAttempt + interval) return false;

            var handler = interactorGO.GetComponentInParent<InteractionHandler>();
            if (handler == null) return false;

            var data = new InteractionData { Interactor = handler.gameObject, InteractionTarget = gameObject };

            BeginInteraction(data);
            CompleteInteraction(data);

            // Update both the global trigger time and the per-object stay time
            _lastTriggerTime = Time.time;
            _lastStayAttemptTimes[instanceId] = Time.time;
            return true;
        }

        /// <summary>
        /// Checks if the given GameObject passes all configured trigger filters.
        /// </summary>
        protected virtual bool PassesFilters(GameObject otherGO)
        {
            if (otherGO == null) return false;

            // Layer filter
            if ((AllowedLayers.value & (1 << otherGO.layer)) == 0) return false;

            // Tag filter
            if (UseTagFilter)
            {
                if (string.IsNullOrEmpty(RequiredTag) || !otherGO.CompareTag(RequiredTag)) return false;
            }

            // Require InteractionHandler if requested
            if (RequireInteractionHandler && otherGO.GetComponentInParent<InteractionHandler>() == null) return false;

            return true;
        }

        /// <summary>
        /// Cleans up stay trigger tracking for the given GameObject.
        /// </summary>
        public virtual void CleanupStay(GameObject otherGO)
        {
            if (otherGO == null) return;
            _lastStayAttemptTimes.Remove(otherGO.GetInstanceID());
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
