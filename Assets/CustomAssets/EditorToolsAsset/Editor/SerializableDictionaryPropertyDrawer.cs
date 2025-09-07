using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Custom property drawer for SerializableDictionary that provides a user-friendly interface
    /// for editing key-value pairs in the Unity Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private const float LINE_HEIGHT = 18f;
        private const float SPACING = 2f;
        private const float BUTTON_WIDTH = 60f;
        private const float DELETE_BUTTON_WIDTH = 20f;

        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

        /// <summary>
        /// Gets the height of the property field.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="label">The property label.</param>
        /// <returns>The height of the property field.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string key = property.propertyPath;
            bool isExpanded = foldoutStates.ContainsKey(key) && foldoutStates[key];

            if (!isExpanded)
            {
                return LINE_HEIGHT;
            }

            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");

            if (keysProperty == null || valuesProperty == null)
            {
                return LINE_HEIGHT;
            }

            float height = LINE_HEIGHT; // Header line
            height += LINE_HEIGHT; // Add button line
            
            // Add column headers line only if there are entries
            if (keysProperty.arraySize > 0)
            {
                height += LINE_HEIGHT; // Column headers line
            }
            
            height += keysProperty.arraySize * (LINE_HEIGHT + SPACING); // Key-value pairs
            height += SPACING; // Extra spacing

            return height;
        }

        /// <summary>
        /// Draws the property field.
        /// </summary>
        /// <param name="position">The position and size of the property field.</param>
        /// <param name="property">The serialized property to draw.</param>
        /// <param name="label">The property label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Ensure managed reference instance exists for generic usage with [SerializeReference]
            if (property.propertyType == SerializedPropertyType.ManagedReference && property.managedReferenceValue == null)
            {
                var t = fieldInfo != null ? fieldInfo.FieldType : null;
                if (t != null && !t.IsAbstract)
                {
                    try
                    {
                        property.managedReferenceValue = Activator.CreateInstance(t);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"SerializableDictionary: Failed to create instance of {t}: {e.Message}");
                    }
                }
            }

            string key = property.propertyPath;
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");

            if (keysProperty == null || valuesProperty == null)
            {
                EditorGUI.LabelField(position, label.text, "SerializableDictionary (keys/values not found)");
                return;
            }

            // Initialize foldout state
            if (!foldoutStates.ContainsKey(key))
            {
                foldoutStates[key] = false;
            }

            // Header with foldout
            Rect headerRect = new Rect(position.x, position.y, position.width, LINE_HEIGHT);
            foldoutStates[key] = EditorGUI.Foldout(headerRect, foldoutStates[key], 
                $"{label.text} ({keysProperty.arraySize} entries)", true);

            if (!foldoutStates[key])
            {
                return;
            }

            // Add button
            Rect addButtonRect = new Rect(position.x, position.y + LINE_HEIGHT, BUTTON_WIDTH, LINE_HEIGHT);
            if (GUI.Button(addButtonRect, "Add"))
            {
                keysProperty.arraySize++;
                valuesProperty.arraySize++;
                
                // Set default values
                SerializedProperty newKey = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
                SerializedProperty newValue = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);
                
                SetDefaultValue(newKey);
                SetDefaultValue(newValue);
                
                property.serializedObject.ApplyModifiedProperties();
            }

            // Draw header labels only if there are entries
            if (keysProperty.arraySize > 0)
            {
                Rect keyLabelRect = new Rect(position.x, position.y + LINE_HEIGHT * 2, 
                    (position.width - DELETE_BUTTON_WIDTH - SPACING) * 0.35f, LINE_HEIGHT);
                Rect valueLabelRect = new Rect(position.x + keyLabelRect.width + SPACING, position.y + LINE_HEIGHT * 2, 
                    (position.width - DELETE_BUTTON_WIDTH - SPACING) * 0.65f, LINE_HEIGHT);
                
                EditorGUI.LabelField(keyLabelRect, "Key", EditorStyles.boldLabel);
                EditorGUI.LabelField(valueLabelRect, "Value", EditorStyles.boldLabel);
            }

            // Draw key-value pairs
            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0; // Ensure fields start at column edges

            float yOffset = keysProperty.arraySize > 0 ? LINE_HEIGHT * 3 : LINE_HEIGHT * 2;
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty valueProp = valuesProperty.GetArrayElementAtIndex(i);

                // Calculate column rects once to avoid drift
                float keyWidth = (position.width - DELETE_BUTTON_WIDTH - SPACING) * 0.35f;
                float valueWidth = (position.width - DELETE_BUTTON_WIDTH - SPACING) * 0.65f;

                Rect keyRect = new Rect(position.x, position.y + yOffset, keyWidth, LINE_HEIGHT);
                Rect valueRect = new Rect(position.x + keyWidth + SPACING, position.y + yOffset, valueWidth, LINE_HEIGHT);
                Rect deleteRect = new Rect(position.x + position.width - DELETE_BUTTON_WIDTH, position.y + yOffset, 
                    DELETE_BUTTON_WIDTH, LINE_HEIGHT);

                // Draw key field (no inline label; headers above indicate meaning)
                EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none, true);

                // Draw value field (no inline label)
                EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none, true);

                // Draw delete button
                if (GUI.Button(deleteRect, "×"))
                {
                    keysProperty.DeleteArrayElementAtIndex(i);
                    valuesProperty.DeleteArrayElementAtIndex(i);
                    property.serializedObject.ApplyModifiedProperties();
                    break; // Exit loop since array size changed
                }

                yOffset += LINE_HEIGHT + SPACING;
            }
            EditorGUI.indentLevel = previousIndent;

            // Validation
            ValidateDictionary(position, property, keysProperty, valuesProperty);
        }

        /// <summary>
        /// Sets a default value for a serialized property based on its type.
        /// </summary>
        /// <param name="property">The property to set a default value for.</param>
        private void SetDefaultValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = "";
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = 0f;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = false;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = Color.white;
                    break;
                case SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = null;
                    break;
                default:
                    // For other types, Unity will handle the default
                    break;
            }
        }

        /// <summary>
        /// Validates the dictionary for duplicate keys and other issues.
        /// </summary>
        /// <param name="position">The position rectangle for the property.</param>
        /// <param name="property">The main property.</param>
        /// <param name="keysProperty">The keys array property.</param>
        /// <param name="valuesProperty">The values array property.</param>
        private void ValidateDictionary(Rect position, SerializedProperty property, SerializedProperty keysProperty, SerializedProperty valuesProperty)
        {
            if (keysProperty.arraySize != valuesProperty.arraySize)
            {
                EditorGUI.HelpBox(new Rect(position.x, position.y + GetPropertyHeight(property, GUIContent.none) - 20, 
                    position.width, 20), "Key and value arrays have different sizes!", MessageType.Error);
            }

            // Check for duplicate keys
            HashSet<string> keyStrings = new HashSet<string>();
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                SerializedProperty keyProp = keysProperty.GetArrayElementAtIndex(i);
                string keyString = GetPropertyValueAsString(keyProp);
                
                if (!string.IsNullOrEmpty(keyString) && !keyStrings.Add(keyString))
                {
                    EditorGUI.HelpBox(new Rect(position.x, position.y + GetPropertyHeight(property, GUIContent.none) - 20, 
                        position.width, 20), $"Duplicate key found: {keyString}", MessageType.Warning);
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the string representation of a property value for comparison.
        /// </summary>
        /// <param name="property">The property to convert.</param>
        /// <returns>The string representation of the property value.</returns>
        private string GetPropertyValueAsString(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Integer:
                    return property.intValue.ToString();
                case SerializedPropertyType.Float:
                    return property.floatValue.ToString();
                case SerializedPropertyType.Boolean:
                    return property.boolValue.ToString();
                case SerializedPropertyType.Vector2:
                    return property.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return property.vector3Value.ToString();
                case SerializedPropertyType.Color:
                    return property.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue?.name ?? "null";
                default:
                    return property.displayName;
            }
        }
    }
}
