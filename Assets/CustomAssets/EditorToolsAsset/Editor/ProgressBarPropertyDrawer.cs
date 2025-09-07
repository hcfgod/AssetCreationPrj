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
            if (property.propertyType == SerializedPropertyType.Integer)
                value = property.intValue;
            else if (property.propertyType == SerializedPropertyType.Float)
                value = property.floatValue;
            else
            {
                EditorGUI.LabelField(position, label.text, "Use with int or float fields");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Normalize value to 0..1 range
            float t = 0f;
            if (attr.Max > attr.Min)
            {
                t = Mathf.InverseLerp(attr.Min, attr.Max, value);
            }
            t = Mathf.Clamp01(t);

            // Draw background and fill
            Rect bg = position;
            EditorGUI.DrawRect(bg, kBack);

            Rect fill = new Rect(bg.x, bg.y, Mathf.Round(bg.width * t), bg.height);
            EditorGUI.DrawRect(fill, attr.BarColor);

            // Compose label text
            string text = string.IsNullOrEmpty(attr.Label) ? label.text : attr.Label;
            if (attr.ShowValue)
            {
                int pct = Mathf.RoundToInt(t * 100f);
                text = string.IsNullOrEmpty(text) ? $"{pct}%" : $"{text} {pct}%";
            }

            // Draw centered label
            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            EditorGUI.LabelField(bg, text, style);

            EditorGUI.EndProperty();
        }
    }
}

