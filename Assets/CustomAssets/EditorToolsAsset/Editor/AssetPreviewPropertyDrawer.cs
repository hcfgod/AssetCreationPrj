using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;
        private static readonly Color kBack = new Color(0.16f, 0.16f, 0.16f, 0.5f);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (AssetPreviewAttribute)attribute;
            float h = 0f;
            if (attr.ShowObjectField)
                h += EditorGUIUtility.singleLineHeight;
            h += Spacing + attr.Height;
            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (AssetPreviewAttribute)attribute;

            // Only object references supported
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;

            // Optional object field
            if (attr.ShowObjectField)
            {
                Rect fieldRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                var fieldType = fieldInfo != null ? fieldInfo.FieldType : typeof(UnityEngine.Object);
                Object newObj = EditorGUI.ObjectField(fieldRect, label, property.objectReferenceValue, fieldType, attr.AllowSceneObjects);
                if (newObj != property.objectReferenceValue)
                {
                    property.objectReferenceValue = newObj;
                }
                y += EditorGUIUtility.singleLineHeight + Spacing;
            }

            Rect previewRect = new Rect(position.x, y, position.width, attr.Height);

            // Draw background
            EditorGUI.DrawRect(previewRect, kBack);

            // Compute centered draw rect
            float targetW = Mathf.Min(attr.Width, previewRect.width);
            float targetH = Mathf.Min(attr.Height, previewRect.height);
            Rect imgRect = new Rect(
                previewRect.x + (previewRect.width - targetW) * 0.5f,
                previewRect.y + (previewRect.height - targetH) * 0.5f,
                targetW,
                targetH
            );

            // Fetch preview texture (fallback to mini thumbnail)
            Texture2D tex = null;
            var obj = property.objectReferenceValue;
            if (obj != null)
            {
                tex = AssetPreview.GetAssetPreview(obj) as Texture2D;
                if (tex == null)
                {
                    tex = AssetPreview.GetMiniThumbnail(obj) as Texture2D;
                }
            }

            if (tex != null)
            {
                GUI.DrawTexture(imgRect, tex, ScaleMode.ScaleToFit, true);
            }
            else
            {
                // Draw placeholder text
                var style = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                string msg = obj == null ? "No Asset" : "Generating Preview...";
                EditorGUI.LabelField(previewRect, msg, style);
                // Hint editor to repaint so preview can appear when ready
                if (obj != null)
                    EditorApplication.delayCall += () => {
                        var window = EditorWindow.focusedWindow;
                        if (window != null) window.Repaint();
                    };
            }

            EditorGUI.EndProperty();
        }
    }
}

