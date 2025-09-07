using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(LayerMaskSelectorAttribute))]
    public class LayerMaskSelectorPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer &&
                property.propertyType != SerializedPropertyType.LayerMask)
            {
                // Fallback for unsupported types
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Get all named layers and their real indices
            string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;
            if (layerNames == null || layerNames.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }
            int[] layerIndices = new int[layerNames.Length];
            for (int i = 0; i < layerNames.Length; i++)
            {
                layerIndices[i] = LayerMask.NameToLayer(layerNames[i]);
            }

            // Current mask in real layer bits (0..31)
            int currentMask = property.intValue;

            // Map to compact mask for MaskField (only named layers)
            int compactMask = 0;
            for (int i = 0; i < layerIndices.Length; i++)
            {
                int li = layerIndices[i];
                if (li >= 0 && (currentMask & (1 << li)) != 0)
                {
                    compactMask |= (1 << i);
                }
            }

            int newCompactMask = EditorGUI.MaskField(position, label, compactMask, layerNames);
            if (newCompactMask != compactMask)
            {
                int newMask = 0;
                for (int i = 0; i < layerIndices.Length; i++)
                {
                    if ((newCompactMask & (1 << i)) != 0)
                    {
                        int li = layerIndices[i];
                        if (li >= 0)
                            newMask |= (1 << li);
                    }
                }
                property.intValue = newMask;
            }

            EditorGUI.EndProperty();
        }
    }
}

