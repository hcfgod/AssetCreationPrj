using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use with string fields");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            string current = property.stringValue ?? string.Empty;
            string newTag = EditorGUI.TagField(position, label, string.IsNullOrEmpty(current) ? "Untagged" : current);
            if (!string.Equals(current, newTag))
            {
                property.stringValue = newTag;
            }
            EditorGUI.EndProperty();
        }
    }
}

