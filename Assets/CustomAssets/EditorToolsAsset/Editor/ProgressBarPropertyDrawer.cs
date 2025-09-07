using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarPropertyDrawer : PropertyDrawer
    {
        private static readonly Color kBack = new Color(0.25f, 0.25f, 0.25f, 0.6f);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (ProgressBarAttribute)attribute;
            return Mathf.Max(attr.Height, EditorGUIUtility.singleLineHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (ProgressBarAttribute)attribute;

            // Only int or float are supported
            float value;
            bool isInt = false;
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                value = property.intValue;
                isInt = true;
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                value = property.floatValue;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use with int or float fields");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Split rect: draw prefix label and compute content rect for bar + value label
            Rect contentRect = EditorGUI.PrefixLabel(position, label);

            float valueLabelWidth = attr.ShowValue ? 54f : 0f;
            float spacing = attr.ShowValue ? 6f : 0f;
            Rect barRect = new Rect(contentRect.x, contentRect.y, Mathf.Max(0f, contentRect.width - (valueLabelWidth + spacing)), attr.Height);
            Rect valRect = new Rect(barRect.xMax + spacing, contentRect.y, valueLabelWidth, attr.Height);

            // Normalize value to 0..1 range
            float t = 0f;
            if (attr.Max > attr.Min)
            {
                t = Mathf.InverseLerp(attr.Min, attr.Max, value);
            }
            t = Mathf.Clamp01(t);

            // Draw background and fill inside barRect
            EditorGUI.DrawRect(barRect, kBack);
            Rect fill = new Rect(barRect.x, barRect.y, Mathf.Round(barRect.width * t), barRect.height);
            EditorGUI.DrawRect(fill, attr.BarColor);

            // Handle editing via mouse if enabled (within barRect only)
            if (attr.Editable)
            {
                EditorGUIUtility.AddCursorRect(barRect, MouseCursor.SlideArrow);
                Event e = Event.current;
                if (e != null && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag || e.type == EventType.MouseUp))
                {
                    // Inclusive hit-test with slight tolerance on the right edge
                    bool within = e.mousePosition.y >= barRect.y && e.mousePosition.y <= barRect.yMax &&
                                  e.mousePosition.x >= barRect.x - 1f && e.mousePosition.x <= barRect.xMax + 1f;
                    if (within)
                    {
                        float mx = Mathf.Clamp(e.mousePosition.x, barRect.x, barRect.xMax);
                        float nt = (mx - barRect.x) / Mathf.Max(1e-6f, (barRect.xMax - barRect.x));
                        nt = Mathf.Clamp01(nt);
                        float newVal = Mathf.Lerp(attr.Min, attr.Max, nt);
                        bool changed = false;
                        if (isInt)
                        {
                            int iv = Mathf.RoundToInt(newVal);
                            if (iv != property.intValue) { property.intValue = iv; changed = true; }
                        }
                        else
                        {
                            if (!Mathf.Approximately(newVal, property.floatValue)) { property.floatValue = newVal; changed = true; }
                        }
                        if (changed)
                        {
                            GUI.changed = true;
                            // Ensure the underlying serialized value gets committed
                            property.serializedObject.ApplyModifiedProperties();
                        }
                        // Update t used for bar/label redraw
                        t = nt;
                        e.Use();
                    }
                }
            }

            // Draw value text to the right, if requested
            if (attr.ShowValue)
            {
                int pct = Mathf.RoundToInt(t * 100f);
                var rightStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
                EditorGUI.LabelField(valRect, $"{pct}%", rightStyle);
            }

            EditorGUI.EndProperty();
        }
    }
}

