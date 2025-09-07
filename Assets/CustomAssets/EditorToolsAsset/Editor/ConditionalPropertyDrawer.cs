using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;
using System.Reflection;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Custom property drawer for the ShowIf and HideIf attributes.
    /// Conditionally shows or hides fields based on other field values.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class ConditionalPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Gets the height of the property field. Returns 0 if the field should be hidden.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="label">The property label.</param>
        /// <returns>The height of the property field, or 0 if hidden.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldShowProperty(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
            return 0f;
        }

        /// <summary>
        /// Draws the property field if it should be shown based on the conditional attribute.
        /// </summary>
        /// <param name="position">The position and size of the property field.</param>
        /// <param name="property">The serialized property to draw.</param>
        /// <param name="label">The property label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldShowProperty(property))
            {
                return;
            }

            // Get the conditional attribute
            ConditionalAttribute conditionalAttribute = (ConditionalAttribute)attribute;
            
            // Set tooltip if provided
            if (!string.IsNullOrEmpty(conditionalAttribute.Tooltip))
            {
                label.tooltip = conditionalAttribute.Tooltip;
            }

            // Draw the property field
            EditorGUI.PropertyField(position, property, label, true);
        }

        /// <summary>
        /// Determines whether the property should be shown based on the conditional attribute.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <returns>True if the property should be shown, false otherwise.</returns>
        private bool ShouldShowProperty(SerializedProperty property)
        {
            ConditionalAttribute conditionalAttribute = (ConditionalAttribute)attribute;
            
            // Get the target object
            object targetObject = property.serializedObject.targetObject;
            
            // Find the field to check
            FieldInfo fieldInfo = GetFieldInfo(targetObject.GetType(), conditionalAttribute.FieldName);
            if (fieldInfo == null)
            {
                Debug.LogWarning($"ShowIf/HideIf: Field '{conditionalAttribute.FieldName}' not found on {targetObject.GetType().Name}");
                return true; // Show by default if field not found
            }

            // Get the field value
            object fieldValue = fieldInfo.GetValue(targetObject);
            
            // Evaluate the condition
            bool conditionMet = EvaluateCondition(fieldValue, conditionalAttribute.ExpectedValue);
            
            // Return result based on attribute type
            if (conditionalAttribute is ShowIfAttribute)
            {
                return conditionMet;
            }
            else if (conditionalAttribute is HideIfAttribute)
            {
                return !conditionMet;
            }
            
            return true; // Default to showing
        }

        /// <summary>
        /// Evaluates whether the condition is met based on the field value and expected value.
        /// </summary>
        /// <param name="fieldValue">The actual field value.</param>
        /// <param name="expectedValue">The expected value to compare against.</param>
        /// <returns>True if the condition is met, false otherwise.</returns>
        private bool EvaluateCondition(object fieldValue, object expectedValue)
        {
            if (expectedValue == null)
            {
                // Check for truthy values
                return IsTruthy(fieldValue);
            }
            else
            {
                // Check for equality
                return fieldValue != null && fieldValue.Equals(expectedValue);
            }
        }

        /// <summary>
        /// Determines if a value is truthy (non-null, non-zero, non-empty, non-false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value is truthy, false otherwise.</returns>
        private bool IsTruthy(object value)
        {
            if (value == null)
                return false;

            if (value is bool boolValue)
                return boolValue;

            if (value is int intValue)
                return intValue != 0;

            if (value is float floatValue)
                return !Mathf.Approximately(floatValue, 0f);

            if (value is string stringValue)
                return !string.IsNullOrEmpty(stringValue);

            if (value is UnityEngine.Object unityObject)
                return unityObject != null;

            // For other types, consider non-null as truthy
            return true;
        }

        /// <summary>
        /// Recursively searches for a field in the given type and its base types.
        /// </summary>
        /// <param name="type">The type to search in.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The FieldInfo if found, null otherwise.</returns>
        private FieldInfo GetFieldInfo(System.Type type, string fieldName)
        {
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (fieldInfo == null && type.BaseType != null)
            {
                return GetFieldInfo(type.BaseType, fieldName);
            }
            
            return fieldInfo;
        }
    }
}
