using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Property drawer for ReorderableListAttribute. Draws arrays and List<T> as a draggable list
    /// with add/remove buttons and proper element rendering.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListPropertyDrawer : PropertyDrawer
    {
        private class ListCache
        {
            public ReorderableList List;
            public SerializedObject SO;
            public SerializedProperty Property;
            public ReorderableListAttribute Options;
            public GUIContent HeaderLabel;
        }

        private static readonly Dictionary<string, ListCache> _listsByKey = new Dictionary<string, ListCache>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // If this attribute ends up applied to a non-array element (Unity can pass it down to children),
            // draw default height to avoid spurious messages.
            if (!IsArrayOrList(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            var cache = GetOrCreateList(property, label);
            return cache.List != null ? cache.List.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If this drawer is invoked for a non-array/list property (e.g., an array element),
            // fall back to default field drawing to prevent incorrect warnings.
            if (!IsArrayOrList(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            var cache = GetOrCreateList(property, label);
            if (cache.List == null)
            {
                // Should not happen, but draw default field if it does
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            // Ensure the serialized object is up-to-date
            cache.SO.Update();
            cache.List.DoList(position);
            cache.SO.ApplyModifiedProperties();
        }

        private static bool IsArrayOrList(SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }

        private ListCache GetOrCreateList(SerializedProperty property, GUIContent label)
        {
            var key = BuildKey(property);
            if (_listsByKey.TryGetValue(key, out var cached))
            {
                // If property moved or target changed, rebuild
                if (!SerializedProperty.DataEquals(cached.Property, property) || cached.SO != property.serializedObject)
                {
                    _listsByKey.Remove(key);
                }
                else
                {
                    return cached;
                }
            }

            var attr = (ReorderableListAttribute)attribute;

            // Only arrays/lists (not strings)
            if (!property.isArray || property.propertyType == SerializedPropertyType.String)
            {
                return new ListCache();
            }

            var list = new ReorderableList(property.serializedObject, property, attr.Draggable, true, attr.ShowAdd, attr.ShowRemove);

            // Header
            var headerLabel = new GUIContent(string.IsNullOrEmpty(attr.Header) ? label.text : attr.Header);
            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, $"{headerLabel.text} ({property.arraySize})", EditorStyles.boldLabel);
            };

            // Element drawer with variable height
            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                if (index < 0 || index >= property.arraySize) return;
                var element = property.GetArrayElementAtIndex(index);
                rect.height = EditorGUI.GetPropertyHeight(element, true);
                var content = attr.ShowElementLabels ? new GUIContent($"Element {index}") : GUIContent.none;
                EditorGUI.PropertyField(rect, element, content, true);
            };

            list.elementHeightCallback = index =>
            {
                if (index < 0 || index >= property.arraySize) return EditorGUIUtility.singleLineHeight + 2f;
                var element = property.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, true) + 2f;
            };

            // Add / Remove
            list.onAddCallback = l =>
            {
                int newIndex = property.arraySize;
                property.arraySize++;
                // Initialize defaults similar to Unity behavior
                var newEl = property.GetArrayElementAtIndex(newIndex);
                InitializeDefaultValue(newEl);
                property.serializedObject.ApplyModifiedProperties();
            };

            list.onRemoveCallback = l =>
            {
                if (l.index >= 0 && l.index < property.arraySize)
                {
                    var el = property.GetArrayElementAtIndex(l.index);
                    if (el.propertyType == SerializedPropertyType.ObjectReference && el.objectReferenceValue != null)
                    {
                        el.objectReferenceValue = null; // first clear reference
                    }
                    else
                    {
                        property.DeleteArrayElementAtIndex(l.index);
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
            };

            var cache = new ListCache
            {
                List = list,
                SO = property.serializedObject,
                Property = property.Copy(),
                Options = attr,
                HeaderLabel = headerLabel
            };
            _listsByKey[key] = cache;
            return cache;
        }

        private static string BuildKey(SerializedProperty property)
        {
            int id = property.serializedObject.targetObject != null ? property.serializedObject.targetObject.GetInstanceID() : 0;
            return id + "::" + property.propertyPath;
        }

        private static void InitializeDefaultValue(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.String:
                    prop.stringValue = string.Empty;
                    break;
                case SerializedPropertyType.Integer:
                    prop.intValue = 0;
                    break;
                case SerializedPropertyType.Float:
                    prop.floatValue = 0f;
                    break;
                case SerializedPropertyType.Boolean:
                    prop.boolValue = false;
                    break;
                case SerializedPropertyType.Vector2:
                    prop.vector2Value = Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    prop.vector3Value = Vector3.zero;
                    break;
                case SerializedPropertyType.Color:
                    prop.colorValue = Color.white;
                    break;
                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = null;
                    break;
                case SerializedPropertyType.Vector2Int:
                    prop.vector2IntValue = Vector2Int.zero;
                    break;
                case SerializedPropertyType.Vector3Int:
                    prop.vector3IntValue = Vector3Int.zero;
                    break;
                default:
                    // For generics and other complex types, Unity will handle defaulting
                    break;
            }
        }
    }
}
