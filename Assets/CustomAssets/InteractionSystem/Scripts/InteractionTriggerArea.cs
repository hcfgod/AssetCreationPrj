using UnityEngine;

namespace CustomAssets.InteractionSystem
{
    public enum TriggerShape
    {
        Box,
        Sphere,
        Capsule
    }
    /// <summary>
    /// Place this on a GameObject with a trigger Collider or Collider2D to define a custom trigger area
    /// for an InteractableBase set to InteractionType.Trigger. The trigger area will use the
    /// filtering and timing options configured on the InteractableBase itself.
    /// </summary>
    [AddComponentMenu("CustomAssets/InteractionSystem/Interaction Trigger Area")]
    public class InteractionTriggerArea : MonoBehaviour
    {
        [Tooltip("The interactable this trigger area belongs to. If not set, will search in parents.")]
        [SerializeField] private InteractableBase _interactable;

        [Header("Trigger Area Shape")]
        [Tooltip("The shape of the trigger area.")]
        [SerializeField] private TriggerShape _triggerShape = TriggerShape.Box;
        [Tooltip("Size of the trigger area.")]
        [SerializeField] private Vector3 _triggerSize = Vector3.one * 2f;
        [Tooltip("Center offset of the trigger area relative to this transform.")]
        [SerializeField] private Vector3 _triggerCenter = Vector3.zero;

        private Collider _cachedCollider;

        private void Reset()
        {
            if (_interactable == null) _interactable = GetComponentInParent<InteractableBase>();
            SetupCollider();
        }

        private void Awake()
        {
            if (_interactable == null) _interactable = GetComponentInParent<InteractableBase>();
            if(_interactable == null) _interactable = GetComponent<InteractableBase>();
            if(_interactable == null) Debug.LogError("InteractionTriggerArea: No InteractableBase found on " + gameObject.name);
            SetupCollider();
        }

        private void OnValidate()
        {
            // Defer collider setup to avoid issues during OnValidate
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => {
                if (this != null) // Check if object still exists
                    SetupCollider();
            };
#endif
        }

        private void SetupCollider()
        {
            switch (_triggerShape)
            {
                case TriggerShape.Box:
                    EnsureColliderType<BoxCollider>();
                    if (_cachedCollider is BoxCollider box)
                    {
                        box.center = _triggerCenter;
                        box.size = _triggerSize;
                    }
                    break;
                    
                case TriggerShape.Sphere:
                    EnsureColliderType<SphereCollider>();
                    if (_cachedCollider is SphereCollider sphere)
                    {
                        sphere.center = _triggerCenter;
                        sphere.radius = _triggerSize.x * 0.5f; // Use X component as radius
                    }
                    break;
                    
                case TriggerShape.Capsule:
                    EnsureColliderType<CapsuleCollider>();
                    if (_cachedCollider is CapsuleCollider capsule)
                    {
                        capsule.center = _triggerCenter;
                        capsule.radius = _triggerSize.x * 0.5f;
                        capsule.height = _triggerSize.y;
                    }
                    break;
            }
            
            if (_cachedCollider != null)
            {
                _cachedCollider.isTrigger = true;
            }
        }
        
        private void EnsureColliderType<T>() where T : Collider
        {
            var existing = GetComponent<Collider>();
            if (existing != null && existing.GetType() != typeof(T))
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    Destroy(existing);
                else
                    DestroyImmediate(existing);
#else
                Destroy(existing);
#endif
                existing = null;
            }
            
            if (existing == null)
            {
                _cachedCollider = gameObject.AddComponent<T>();
            }
            else
            {
                _cachedCollider = existing;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_interactable != null) _interactable.TryAutoTrigger(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_interactable != null) _interactable.TryAutoTrigger(other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_interactable != null) _interactable.TryStayTrigger(other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_interactable != null) _interactable.TryStayTrigger(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactable != null) _interactable.CleanupStay(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_interactable != null) _interactable.CleanupStay(other.gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw trigger shape
            Gizmos.color = Color.magenta;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            
            switch (_triggerShape)
            {
                case TriggerShape.Box:
                    Gizmos.DrawWireCube(_triggerCenter, _triggerSize);
                    break;
                case TriggerShape.Sphere:
                    Gizmos.DrawWireSphere(_triggerCenter, _triggerSize.x * 0.5f);
                    break;
                case TriggerShape.Capsule:
                    // Unity doesn't have a built-in capsule gizmo, so we'll approximate
                    var radius = _triggerSize.x * 0.5f;
                    var height = _triggerSize.y;
                    var halfHeight = height * 0.5f;
                    // Draw top and bottom spheres
                    Gizmos.DrawWireSphere(_triggerCenter + Vector3.up * (halfHeight - radius), radius);
                    Gizmos.DrawWireSphere(_triggerCenter + Vector3.down * (halfHeight - radius), radius);
                    // Draw connecting lines
                    var p1 = _triggerCenter + Vector3.right * radius + Vector3.up * (halfHeight - radius);
                    var p2 = _triggerCenter + Vector3.right * radius + Vector3.down * (halfHeight - radius);
                    Gizmos.DrawLine(p1, p2);
                    p1 = _triggerCenter + Vector3.left * radius + Vector3.up * (halfHeight - radius);
                    p2 = _triggerCenter + Vector3.left * radius + Vector3.down * (halfHeight - radius);
                    Gizmos.DrawLine(p1, p2);
                    p1 = _triggerCenter + Vector3.forward * radius + Vector3.up * (halfHeight - radius);
                    p2 = _triggerCenter + Vector3.forward * radius + Vector3.down * (halfHeight - radius);
                    Gizmos.DrawLine(p1, p2);
                    p1 = _triggerCenter + Vector3.back * radius + Vector3.up * (halfHeight - radius);
                    p2 = _triggerCenter + Vector3.back * radius + Vector3.down * (halfHeight - radius);
                    Gizmos.DrawLine(p1, p2);
                    break;
            }
            
            Gizmos.matrix = Matrix4x4.identity;

            // Label with shape info
            if (_interactable != null)
            {
                string shapeInfo = $"{_triggerShape}({_triggerSize.x:0.1},{_triggerSize.y:0.1},{_triggerSize.z:0.1})";
                string retrigInfo = $"ReCD={_interactable.RetriggerDelay:0.00}s";
                string label = $"TriggerArea [{shapeInfo} | {retrigInfo}]";

                var pos = transform.position + _triggerCenter;
                UnityEditor.Handles.color = Color.magenta;
                UnityEditor.Handles.Label(pos + Vector3.up * (_triggerSize.y * 0.5f + 0.25f), label);
            }
            else
            {
                var pos = transform.position + _triggerCenter;
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.Label(pos + Vector3.up * (_triggerSize.y * 0.5f + 0.25f), "TriggerArea [No InteractableBase]");
            }
        }
#endif
    }
}
