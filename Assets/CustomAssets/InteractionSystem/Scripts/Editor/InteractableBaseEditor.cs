using UnityEngine;
using UnityEditor;

namespace CustomAssets.InteractionSystem.Editor
{
    [CustomEditor(typeof(InteractableBase), true)]
    public class InteractableBaseEditor : UnityEditor.Editor
    {
        private InteractionType _previousInteractionType;
        private bool _initialized = false;

        private void OnEnable()
        {
            var interactable = target as InteractableBase;
            if (interactable != null)
            {
                _previousInteractionType = interactable.InteractionMode;
                _initialized = true;
            }
        }

        public override void OnInspectorGUI()
        {
            var interactable = target as InteractableBase;
            if (interactable == null) return;

            // Ensure collider exists for all interactables
            EnsureColliderExists(interactable);

            // Store the current interaction type before drawing the inspector
            var currentType = interactable.InteractionMode;

            // Draw the default inspector
            DrawDefaultInspector();

            // Check if the interaction type changed after drawing the inspector
            if (_initialized && currentType != _previousInteractionType)
            {
                HandleInteractionTypeChange(interactable, _previousInteractionType, currentType);
                _previousInteractionType = currentType;
            }
            else if (!_initialized)
            {
                _previousInteractionType = currentType;
                _initialized = true;
            }

            // Show helpful info for Trigger type
            if (currentType == InteractionType.Trigger)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Trigger Info", EditorStyles.boldLabel);
                
                var existingArea = interactable.GetComponent<InteractionTriggerArea>();
                var hasCollider3D = interactable.GetComponent<Collider>() != null;
                var hasCollider2D = interactable.GetComponent<Collider2D>() != null;
                
                if (existingArea != null)
                {
                    if (hasCollider3D || hasCollider2D)
                    {
                        EditorGUILayout.HelpBox("This object has an InteractionTriggerArea component. The trigger will fire when objects enter this collider.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("InteractionTriggerArea component is present but no Collider/Collider2D found. Add a collider and set it to 'Is Trigger' for trigger events to work.", MessageType.Warning);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("InteractionTriggerArea component added. Make sure at least one object (player or trigger) has a Rigidbody/Rigidbody2D for trigger events to work.", MessageType.Info);
                }
            }
        }

        private void HandleInteractionTypeChange(InteractableBase interactable, InteractionType oldType, InteractionType newType)
        {
            if (newType == InteractionType.Trigger && oldType != InteractionType.Trigger)
            {
                // Switched TO Trigger
                AddInteractionTriggerArea(interactable);
            }
            else if (oldType == InteractionType.Trigger && newType != InteractionType.Trigger)
            {
                // Switched FROM Trigger
                RemoveInteractionTriggerArea(interactable);
            }
        }


        private void EnsureColliderExists(InteractableBase interactable)
        {
            var collider3D = interactable.GetComponent<Collider>();
            var collider2D = interactable.GetComponent<Collider2D>();
            
            // If no collider exists, add a BoxCollider as default
            if (collider3D == null && collider2D == null)
            {
                collider3D = interactable.gameObject.AddComponent<BoxCollider>(); 
                Undo.RegisterCreatedObjectUndo(collider3D, "Add BoxCollider for Interactable");
            }
        }

        private void AddInteractionTriggerArea(InteractableBase interactable)
        {
            // Check if it already exists
            var existing = interactable.GetComponent<InteractionTriggerArea>();
            if (existing != null) return;

            // Add the component (collider is ensured to exist by EnsureColliderExists)
            var triggerArea = interactable.gameObject.AddComponent<InteractionTriggerArea>();
            
            // Set colliders to trigger
            var collider3D = interactable.GetComponent<Collider>();
            var collider2D = interactable.GetComponent<Collider2D>();
            
            if (collider3D != null)
            {
                Undo.RecordObject(collider3D, "Set Collider Is Trigger");
                collider3D.isTrigger = true;
            }
            if (collider2D != null)
            {
                Undo.RecordObject(collider2D, "Set Collider2D Is Trigger");
                collider2D.isTrigger = true;
            }

            Undo.RegisterCreatedObjectUndo(triggerArea, "Add InteractionTriggerArea");
        }

        private void RemoveInteractionTriggerArea(InteractableBase interactable)
        {
            var existing = interactable.GetComponent<InteractionTriggerArea>();
            if (existing != null)
            {
                Undo.DestroyObjectImmediate(existing);
                Debug.Log($"Removed InteractionTriggerArea from {interactable.name}.");
            }

            // Optionally reset collider trigger state (user might want to keep it as trigger for other reasons)
            // We'll leave it as-is to avoid breaking other functionality
        }

    }
}
