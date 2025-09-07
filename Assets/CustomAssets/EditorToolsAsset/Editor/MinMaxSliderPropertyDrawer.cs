using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Property drawer for MinMaxSliderAttribute. Supports Vector2 (float) and Vector2Int (int) fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderPropertyDrawer : PropertyDrawer
    {
        private const float FieldWidth = 60f;
        private const float Spacing = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (MinMaxSliderAttribute)attribute;

            // Only Vector2 or Vector2Int are supported
            if (property.propertyType != SerializedPropertyType.Vector2 && property.propertyType != SerializedPropertyType.Vector2Int)
            {
                EditorGUI.LabelField(position, label.text, "Use with Vector2 or Vector2Int");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Draw prefix and get remaining rect
            Rect contentRect = EditorGUI.PrefixLabel(position, label);
            float x = contentRect.x;
            float w = contentRect.width;

            bool showFields = attr.ShowFields;
            float leftFieldWidth = showFields ? FieldWidth : 0f;
            float rightFieldWidth = showFields ? FieldWidth : 0f;
            float sliderWidth = w - (showFields ? (leftFieldWidth + rightFieldWidth + Spacing * 2f) : 0f);

            Rect leftFieldRect = new Rect(x, contentRect.y, leftFieldWidth, contentRect.height);
            Rect sliderRect = new Rect(x + (showFields ? leftFieldWidth + Spacing : 0f), contentRect.y, sliderWidth, contentRect.height);
            Rect rightFieldRect = new Rect(sliderRect.x + sliderRect.width + (showFields ? Spacing : 0f), contentRect.y, rightFieldWidth, contentRect.height);

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 value = property.vector2Value;
                float minVal = value.x;
                float maxVal = value.y;

                // Numeric fields (left min, right max)
                if (showFields)
                {
                    minVal = DrawFloatField(leftFieldRect, minVal, attr.Decimals);
                }

                // Slider (operates in floats)
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, attr.MinLimitFloat, attr.MaxLimitFloat);

                if (showFields)
                {
                    maxVal = DrawFloatField(rightFieldRect, maxVal, attr.Decimals);
                }

                // Clamp and order
                minVal = Mathf.Clamp(minVal, attr.MinLimitFloat, attr.MaxLimitFloat);
                maxVal = Mathf.Clamp(maxVal, attr.MinLimitFloat, attr.MaxLimitFloat);
                if (maxVal < minVal) maxVal = minVal;

                // Apply back
                if (!Mathf.Approximately(value.x, minVal) || !Mathf.Approximately(value.y, maxVal))
                {
                    property.vector2Value = new Vector2(minVal, maxVal);
                }
            }
            else // Vector2Int
            {
                Vector2Int value = property.vector2IntValue;
                int minVal = value.x;
                int maxVal = value.y;

                if (showFields)
                {
                    minVal = EditorGUI.IntField(leftFieldRect, minVal);
                }

                // Slider uses floats internally; round to ints
                float fMin = minVal;
                float fMax = maxVal;
                EditorGUI.MinMaxSlider(sliderRect, ref fMin, ref fMax, attr.MinLimitInt, attr.MaxLimitInt);
                minVal = Mathf.RoundToInt(fMin);
                maxVal = Mathf.RoundToInt(fMax);

                if (showFields)
                {
                    maxVal = EditorGUI.IntField(rightFieldRect, maxVal);
                }

                // Clamp and order
                minVal = Mathf.Clamp(minVal, attr.MinLimitInt, attr.MaxLimitInt);
                maxVal = Mathf.Clamp(maxVal, attr.MinLimitInt, attr.MaxLimitInt);
                if (maxVal < minVal) maxVal = minVal;

                if (value.x != minVal || value.y != maxVal)
                {
                    property.vector2IntValue = new Vector2Int(minVal, maxVal);
                }
            }

            EditorGUI.EndProperty();
        }

        private static float DrawFloatField(Rect rect, float val, int decimals)
        {
            // Use a delayed field to reduce jitter while sliding
            string format = "F" + Mathf.Clamp(decimals, 0, 6).ToString();
            string s = EditorGUI.TextField(rect, val.ToString(format));
            if (float.TryParse(s, out float parsed))
            {
                return parsed;
            }
            return val;
        }
    }
}
