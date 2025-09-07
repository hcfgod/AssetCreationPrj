using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
    public class LayerSelectorPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                int newVal = EditorGUI.LayerField(position, label, property.intValue);
                if (newVal != property.intValue)
                {
                    property.intValue = newVal;
                }
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                // For string fields, present layer names and write the selected name
                var layers = UnityEditorInternal.InternalEditorUtility.layers;
                if (layers == null || layers.Length == 0)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                    EditorGUI.EndProperty();
                    return;
                }

                string current = property.stringValue ?? string.Empty;
                int index = 0;
                for (int i = 0; i < layers.Length; i++)
                {
                    if (string.Equals(layers[i], current))
                    {
                        index = i;
                        break;
                    }
                }

                int newIndex = EditorGUI.Popup(position, label.text, index, layers);
                if (newIndex != index)
                {
                    property.stringValue = layers[newIndex];
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            EditorGUI.EndProperty();
        }
    }
}

