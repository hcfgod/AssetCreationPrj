using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (InfoBoxAttribute)attribute;
            bool show = EvaluateVisible(property, attr.VisibleIf);
            float baseHeight = EditorGUI.GetPropertyHeight(property, label, true);
            if (!show || string.IsNullOrEmpty(attr.Message))
                return baseHeight;

            float width = EditorGUIUtility.currentViewWidth - 40f;
            float autoHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(attr.Message), width);
            float helpHeight = attr.FixedHeight > 0f ? attr.FixedHeight : autoHeight;
            return helpHeight + 4f + baseHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (InfoBoxAttribute)attribute;
            bool show = EvaluateVisible(property, attr.VisibleIf);

            float y = position.y;
            if (show && !string.IsNullOrEmpty(attr.Message))
            {
                float width = position.width;
                float autoHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(attr.Message), width);
                float helpHeight = attr.FixedHeight > 0f ? attr.FixedHeight : autoHeight;
                Rect helpRect = new Rect(position.x, y, width, helpHeight);
                EditorGUI.HelpBox(helpRect, attr.Message, ToMessageType(attr.Type));
                y += helpHeight + 4f;
            }

            Rect fieldRect = new Rect(position.x, y, position.width, EditorGUI.GetPropertyHeight(property, label, true));
            EditorGUI.PropertyField(fieldRect, property, label, true);
        }

        private static MessageType ToMessageType(InfoBoxType t)
        {
            switch (t)
            {
                case InfoBoxType.Warning: return MessageType.Warning;
                case InfoBoxType.Error: return MessageType.Error;
                default: return MessageType.Info;
            }
        }

        private bool EvaluateVisible(SerializedProperty property, string visibleIf)
        {
            if (string.IsNullOrEmpty(visibleIf)) return true;
            try
            {
                object target = property.serializedObject.targetObject;
                var type = target.GetType();

                // Try method first
                var mi = type.GetMethod(visibleIf, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (mi != null && mi.ReturnType == typeof(bool) && mi.GetParameters().Length == 0)
                {
                    object instance = mi.IsStatic ? null : target;
                    return (bool)mi.Invoke(instance, null);
                }

                // Try property
                var pi = type.GetProperty(visibleIf, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (pi != null && pi.PropertyType == typeof(bool))
                {
                    object instance = (pi.GetGetMethod(true)?.IsStatic ?? false) ? null : target;
                    return (bool)pi.GetValue(instance);
                }

                // Try field
                var fi = type.GetField(visibleIf, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (fi != null && fi.FieldType == typeof(bool))
                {
                    object instance = fi.IsStatic ? null : target;
                    return (bool)fi.GetValue(instance);
                }
            }
            catch { }
            // If evaluation fails, default to showing
            return true;
        }
    }
}
