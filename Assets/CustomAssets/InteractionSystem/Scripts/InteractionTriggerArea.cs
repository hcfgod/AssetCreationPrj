using UnityEngine;

namespace CustomAssets.InteractionSystem
{
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
            if(_interactable == null) _interactable = GetComponent<InteractableBase>();
            if(_interactable == null) Debug.LogError("InteractionTriggerArea: No InteractableBase found on " + gameObject.name);
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
            // Draw trigger bounds
            Gizmos.color = Color.magenta;
            var c3d = GetComponent<Collider>();
            if (c3d != null)
            {
                var b = c3d.bounds;
                Gizmos.DrawWireCube(b.center, b.size);
            }
            var c2d = GetComponent<Collider2D>();
            if (c2d != null)
            {
                var b2 = c2d.bounds;
                Gizmos.DrawWireCube(b2.center, b2.size);
            }

            // Label - now shows InteractableBase settings
            if (_interactable != null)
            {
                string tagInfo = _interactable.UseTagFilter ? ($"Tag='{_interactable.RequiredTag}'") : "Tag=Any";
                string handlerInfo = _interactable.RequireInteractionHandler ? "Handler=Req" : "Handler=Opt";
                string stayInfo = _interactable.RetriggerOnStay ? ($"StayInt={_interactable.StayRepeatInterval:0.00}s") : "Stay=Off";
                string retrigInfo = $"ReCD={_interactable.RetriggerDelay:0.00}s";
                string label = $"TriggerArea [{tagInfo} | {handlerInfo} | {stayInfo} | {retrigInfo}]";

                var pos = transform.position;
                UnityEditor.Handles.color = Color.magenta;
                UnityEditor.Handles.Label(pos + Vector3.up * 0.25f, label);
            }
            else
            {
                var pos = transform.position;
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.Label(pos + Vector3.up * 0.25f, "TriggerArea [No InteractableBase]");
            }
        }
#endif
    }
}
