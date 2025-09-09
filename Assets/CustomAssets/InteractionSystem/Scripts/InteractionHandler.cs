using UnityEngine;
using UnityEngine.InputSystem;

namespace CustomAssets.InteractionSystem
{
    /// <summary>
    /// Player-facing component that discovers interactables (2D and/or 3D) and drives their lifecycle
    /// via Unity's new Input System. Supports press or hold-to-interact, and chooses targets based on
    /// ray/sphere casts (3D) and overlap queries (2D). Designed to be reusable across projects.
    /// </summary>
    public class InteractionHandler : MonoBehaviour
    {
        [Header("Input System")]
        [Tooltip("Input Action used to trigger interactions (e.g. " + "'Player/Interact'" + ").")]
        [SerializeField] private InputActionReference _interactAction;

        [Header("Targeting & Detection")]
        [SerializeField] private Camera _camera;
        [Tooltip("Layer mask to limit interactable detection.")]
        [SerializeField] private LayerMask _layerMask = ~0;
        [Tooltip("Maximum detection range in world units.")]
        [SerializeField] private float _range = 3f;
        [Tooltip("Enable 3D Physics detection via ray/sphere cast.")]
        [SerializeField] private bool _use3D = true;
        [Tooltip("Enable 2D Physics detection via overlap circle.")]
        [SerializeField] private bool _use2D = true;
        [Tooltip("If true, uses a sphere cast in 3D (better for small targets). If false, uses a raycast.")]
        [SerializeField] private bool _useSphereCast3D = true;
        [SerializeField] private float _sphereCastRadius = 0.2f;
        [Header("View Gate")]
        [Tooltip("Require the target to be near the screen center (normalized viewport radius, 0..0.5). Set 0 to disable.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float _viewportCenterRadius = 0.08f;

        [Header("Debug")]
        [SerializeField] private bool _drawGizmos = true;

        private IInteractable _currentTarget;
        private IInteractable _activeInteractionTarget; // The one we started interacting with
        private bool _isInteracting;
        private float _interactStartTime;

        private readonly RaycastHit[] _hits3D = new RaycastHit[8];

        private void Reset()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void OnEnable()
        {
            if (_interactAction != null)
            {
                var action = _interactAction.action;
                if (!action.enabled) action.Enable();
                action.started += OnInteractStarted;
                action.canceled += OnInteractCanceled;
            }
            else
            {
                Debug.LogWarning("InteractionHandler: No InputActionReference assigned for Interact.", this);
            }
        }

        private void OnDisable()
        {
            if (_interactAction != null)
            {
                var action = _interactAction.action;
                action.started -= OnInteractStarted;
                action.canceled -= OnInteractCanceled;
            }

            // If we're disabling while interacting, end gracefully
            if (_isInteracting)
                EndInteraction(true);
        }

        private void Update()
        {
            // Keep target up to date
            _currentTarget = AcquireTarget();

            // If target changed while interacting, cancel the old one
            if (_isInteracting && _activeInteractionTarget != _currentTarget)
            {
                EndInteraction(true);
            }

            // Update interaction if in progress
            if (_isInteracting && _activeInteractionTarget != null)
            {
                GetInteractionConfig(_activeInteractionTarget, out var type, out var duration, out _);
                var data = BuildInteractionData(_activeInteractionTarget);
                var baseTarget = _activeInteractionTarget as InteractableBase;

                if (type == InteractionType.Hold)
                {
                    float progress = duration > 0f ? Mathf.Clamp01((Time.time - _interactStartTime) / duration) : 1f;

                    if (baseTarget != null) baseTarget.ReportProgress(data, progress);
                    else _activeInteractionTarget.OnInteractProgress(data, progress);

                    if (progress >= 1f)
                    {
                        EndInteraction(false);
                    }
                }
                else if (type == InteractionType.Custom)
                {
                    // Let custom interactables define their own completion; we just ping progress as 0..1 if desired.
                    if (baseTarget != null) baseTarget.ReportProgress(data, 0f);
                    else _activeInteractionTarget.OnInteractProgress(data, 0f);
                }
            }
        }

        private void OnInteractStarted(InputAction.CallbackContext ctx)
        {
            if (_currentTarget == null)
                return;

            _activeInteractionTarget = _currentTarget;
            var data = BuildInteractionData(_activeInteractionTarget);

            // Read per-target config
            GetInteractionConfig(_activeInteractionTarget, out var type, out var holdDuration, out _);
            var baseTarget = _activeInteractionTarget as InteractableBase;


            switch (type)
            {
                case InteractionType.Instant:
                {
                    if (baseTarget != null)
                    {
                        baseTarget.BeginInteraction(data);
                        baseTarget.ReportProgress(data, 1f);
                        baseTarget.CompleteInteraction(data);
                    }
                    else
                    {
                        _activeInteractionTarget.OnInteractStart(data);
                        _activeInteractionTarget.OnInteractProgress(data, 1f);
                        _activeInteractionTarget.OnInteractEnd(data);
                    }
                    _activeInteractionTarget = null;
                    _isInteracting = false;
                    break;
                }
                case InteractionType.Trigger:
                {
                    // Fire and forget like Instant, but without progress (or with 1 if desired)
                    if (baseTarget != null)
                    {
                        baseTarget.BeginInteraction(data);
                        baseTarget.CompleteInteraction(data);
                    }
                    else
                    {
                        _activeInteractionTarget.OnInteractStart(data);
                        _activeInteractionTarget.OnInteractEnd(data);
                    }
                    _activeInteractionTarget = null;
                    _isInteracting = false;
                    break;
                }
                case InteractionType.Toggle:
                {
                    if (baseTarget != null)
                    {
                        if (baseTarget.State == InteractionState.Interacting)
                        {
                            baseTarget.CompleteInteraction(data);
                        }
                        else
                        {
                            baseTarget.BeginInteraction(data);
                        }
                    }
                    else
                    {
                        // Fallback: simulate toggle by start/end immediately
                        if (!_isInteracting)
                        {
                            _activeInteractionTarget.OnInteractStart(data);
                            _isInteracting = true;
                        }
                        else
                        {
                            _activeInteractionTarget.OnInteractEnd(data);
                            _isInteracting = false;
                        }
                    }
                    // No continuous interaction loop for toggle
                    _activeInteractionTarget = null;
                    break;
                }
                case InteractionType.Hold:
                {
                    _isInteracting = true;
                    _interactStartTime = Time.time;
                    if (baseTarget != null)
                        baseTarget.BeginInteraction(data);
                    else
                        _activeInteractionTarget.OnInteractStart(data);
                    break;
                }
                case InteractionType.Custom:
                {
                    // Minimal semantics: start on press; complete on release
                    _isInteracting = true; // allow progress if derived class wants to use it
                    _interactStartTime = Time.time;
                    if (baseTarget != null)
                        baseTarget.BeginInteraction(data);
                    else
                        _activeInteractionTarget.OnInteractStart(data);
                    break;
                }
            }
        }

        private void OnInteractCanceled(InputAction.CallbackContext ctx)
        {
            if (_isInteracting)
            {
                // Determine whether to cancel or complete based on interaction type
                GetInteractionConfig(_activeInteractionTarget, out var type, out _, out _);
                bool cancel = type != InteractionType.Custom; // Custom completes on release
                EndInteraction(cancel);
            }
        }

        private void EndInteraction(bool cancel)
        {
            if (_activeInteractionTarget != null)
            {
                var data = BuildInteractionData(_activeInteractionTarget);
                var baseTarget = _activeInteractionTarget as InteractableBase;

                if (cancel)
                {
                    if (baseTarget != null) baseTarget.CancelInteraction(data);
                    else _activeInteractionTarget.OnInteractEnd(data);
                }
                else
                {
                    if (baseTarget != null) baseTarget.CompleteInteraction(data);
                    else _activeInteractionTarget.OnInteractEnd(data);
                }
            }

            _isInteracting = false;
            _activeInteractionTarget = null;
        }

        private InteractionData BuildInteractionData(IInteractable target)
        {
            var targetGO = (target as Component) != null ? ((Component)target).gameObject : null;
            return new InteractionData
            {
                Interactor = gameObject,
                InteractionTarget = targetGO
            };
        }

        private bool PassesViewGate(Transform target)
        {
            if (_camera == null) return true; // no camera: skip gate
            if (_viewportCenterRadius <= 0f) return true; // gate disabled

            var vp = _camera.WorldToViewportPoint(target.position);
            if (vp.z <= 0f) return false; // behind camera
            var offset = new Vector2(vp.x - 0.5f, vp.y - 0.5f);
            return offset.magnitude <= _viewportCenterRadius;
        }

        private float GetViewportCenterOffsetSqr(Vector3 worldPos)
        {
            if (_camera == null) return 0f; // no camera: neutral score
            var vp = _camera.WorldToViewportPoint(worldPos);
            var offset = new Vector2(vp.x - 0.5f, vp.y - 0.5f);
            return offset.sqrMagnitude;
        }

        private void GetInteractionConfig(IInteractable target, out InteractionType type, out float holdDuration, out PostInteractionAction postAction)
        {
            var baseTarget = target as InteractableBase;
            if (baseTarget != null)
            {
                type = baseTarget.InteractionMode;
                holdDuration = Mathf.Max(0f, baseTarget.HoldDuration);
                postAction = baseTarget.PostAction;
            }
            else
            {
                type = InteractionType.Instant;
                holdDuration = 0f;
                postAction = PostInteractionAction.None;
            }
        }


        private IInteractable AcquireTarget()
        {
            IInteractable found = null;

            if (_use3D)
            {
                var cam = _camera != null ? _camera : Camera.main;
                if (cam != null)
                {
                    var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                    if (_useSphereCast3D)
                    {
                        int count = Physics.SphereCastNonAlloc(ray, _sphereCastRadius, _hits3D, _range, _layerMask, QueryTriggerInteraction.Collide);
                        float bestDist = float.PositiveInfinity;
                        for (int i = 0; i < count; i++)
                        {
                            var hit = _hits3D[i];
                            var candidate = hit.collider.GetComponentInParent<IInteractable>();
                            if (candidate != null && hit.distance < bestDist)
                            {
                                bestDist = hit.distance;
                                found = candidate;
                            }
                        }
                    }
                    else if (Physics.Raycast(ray, out var hit, _range, _layerMask, QueryTriggerInteraction.Collide))
                    {
                        found = hit.collider.GetComponentInParent<IInteractable>();
                    }
                }
                else
                {
                    // Fallback ray from transform if no camera is available
                    var ray = new Ray(transform.position, transform.forward);
                    if (Physics.Raycast(ray, out var hit, _range, _layerMask, QueryTriggerInteraction.Collide))
                    {
                        found = hit.collider.GetComponentInParent<IInteractable>();
                    }
                }
            }

            if (found == null && _use2D)
            {
                var pos2D = (Vector2)transform.position;
                var hits2D = Physics2D.OverlapCircleAll(pos2D, _range, _layerMask);
                float bestScore = float.PositiveInfinity;
                IInteractable best = null;
                for (int i = 0; i < hits2D.Length; i++)
                {
                    var c = hits2D[i];
                    var candidate = c.GetComponentInParent<IInteractable>();
                    if (candidate == null) continue;
                    var comp = candidate as Component;
                    if (comp == null) continue;
                    if (!PassesViewGate(comp.transform)) continue; // must be near screen center if gate enabled

                    float score = GetViewportCenterOffsetSqr(comp.transform.position);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        best = candidate;
                    }
                }
                found = best;
            }


            return found;
        }

        public void SetInteractAction(InputActionReference action)
        {
            // Allow rebinding at runtime if needed
            if (_interactAction != null)
            {
                var a = _interactAction.action;
                a.started -= OnInteractStarted;
                a.canceled -= OnInteractCanceled;
            }

            _interactAction = action;
            if (_interactAction != null)
            {
                var a = _interactAction.action;
                if (!a.enabled) a.Enable();
                a.started += OnInteractStarted;
                a.canceled += OnInteractCanceled;
            }
        }

        public void SetCamera(Camera cam)
        {
            _camera = cam;
        }

        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos) return;

            Gizmos.color = Color.cyan;
            if (_use3D)
            {
                var cam = _camera != null ? _camera : Camera.main;
                if (cam != null)
                {
                    var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                    Gizmos.DrawRay(ray.origin, ray.direction * _range);
                    if (_useSphereCast3D)
                    {
                        // Approximate sphere cast start and end spheres
                        var start = ray.origin;
                        var end = ray.origin + ray.direction * _range;
                        Gizmos.DrawWireSphere(start, _sphereCastRadius);
                        Gizmos.DrawWireSphere(end, _sphereCastRadius);
                    }
                }
            }

            if (_use2D)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, _range);
            }
        }
    }
}
