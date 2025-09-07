using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(AssetPreviewAttribute))]
    public class AssetPreviewPropertyDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

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

            EditorGUI.BeginProperty(position, label, property);

            float y = position.y;
            Object obj = null;

            // Optional object field and object resolution
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
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
                obj = property.objectReferenceValue;
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                string currentPath = property.stringValue ?? string.Empty;
                Object currentObject = !string.IsNullOrEmpty(currentPath) ? AssetDatabase.LoadMainAssetAtPath(currentPath) : null;
                if (attr.ShowObjectField)
                {
                    Rect fieldRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                    Object picked = EditorGUI.ObjectField(fieldRect, label, currentObject, typeof(Object), false);
                    if (picked != currentObject)
                    {
                        property.stringValue = picked != null ? AssetDatabase.GetAssetPath(picked) : string.Empty;
                        currentObject = picked;
                    }
                    y += EditorGUIUtility.singleLineHeight + Spacing;
                }
                obj = currentObject;
            }
            else
            {
                // Fallback for unsupported types
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }

            Rect previewRect = new Rect(position.x, y, position.width, attr.Height);

            // Draw background
            EditorGUI.DrawRect(previewRect, attr.BackgroundColor);

            // Fetch preview texture (fallback to mini thumbnail)
            Texture2D tex = null;
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
                // Compute draw rect (keep aspect vs fill)
                Rect imgRect = previewRect;
                if (attr.KeepAspect)
                {
                    float texAspect = (tex.height > 0) ? (float)tex.width / tex.height : 1f;
                    float rectAspect = previewRect.width / Mathf.Max(1f, previewRect.height);
                    if (texAspect > rectAspect)
                    {
                        // fit width
                        float h = previewRect.width / Mathf.Max(0.0001f, texAspect);
                        imgRect = new Rect(previewRect.x, previewRect.y + (previewRect.height - h) * 0.5f, previewRect.width, h);
                    }
                    else
                    {
                        // fit height
                        float w = previewRect.height * texAspect;
                        imgRect = new Rect(previewRect.x + (previewRect.width - w) * 0.5f, previewRect.y, w, previewRect.height);
                    }
                }

                // Apply tint
                var prevColor = GUI.color;
                GUI.color = attr.TintColor;
                GUI.DrawTexture(imgRect, tex, attr.KeepAspect ? ScaleMode.ScaleToFit : ScaleMode.StretchToFill, true);
                GUI.color = prevColor;
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
                if (obj != null)
                {
                    // Hint editor to repaint so preview can appear when ready
                    EditorApplication.delayCall += () =>
                    {
                        var window = EditorWindow.focusedWindow;
                        if (window != null) window.Repaint();
                    };
                }
            }

            EditorGUI.EndProperty();
        }
    }
}

