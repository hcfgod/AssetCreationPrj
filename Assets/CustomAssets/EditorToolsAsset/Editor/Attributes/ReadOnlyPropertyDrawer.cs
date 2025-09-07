using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Custom property drawer for the ReadOnly attribute.
    /// Renders the field as read-only (grayed out) in the Unity Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Gets the height of the property field.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="label">The property label.</param>
        /// <returns>The height of the property field.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        /// <summary>
        /// Draws the property field as read-only.
        /// </summary>
        /// <param name="position">The position and size of the property field.</param>
        /// <param name="property">The serialized property to draw.</param>
        /// <param name="label">The property label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the ReadOnly attribute
            ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)attribute;
            
            // Set tooltip if provided
            if (!string.IsNullOrEmpty(readOnlyAttribute.Tooltip))
            {
                label.tooltip = readOnlyAttribute.Tooltip;
            }

            // Store the original GUI state
            bool wasEnabled = GUI.enabled;
            
            // Disable the GUI to make it read-only
            GUI.enabled = false;
            
            // Draw the property field
            EditorGUI.PropertyField(position, property, label, true);
            
            // Restore the original GUI state
            GUI.enabled = wasEnabled;
        }
    }
}
