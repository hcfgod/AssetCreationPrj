using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(FoldoutGroupAttribute))]
    public class FoldoutGroupPropertyDrawer : PropertyDrawer
    {
        private const float HEADER_HEIGHT = 20f;
        private const float SPACING = 2f;

        private static readonly Dictionary<string, bool> expandedByGroupKey = new Dictionary<string, bool>();
        private static readonly Dictionary<string, string> headerOwnerByGroupKey = new Dictionary<string, string>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (FoldoutGroupAttribute)attribute;
            string groupKey = BuildGroupKey(property, attr);

            EnsureHeaderOwner(property, attr, groupKey);
            EnsureExpandedInitialized(attr, groupKey);

            bool isOwner = IsHeaderOwner(property, groupKey);
            bool expanded = expandedByGroupKey[groupKey];

            float height = 0f;
            if (isOwner)
            {
                height += HEADER_HEIGHT + SPACING;
                if (expanded)
                {
                    height += EditorGUI.GetPropertyHeight(property, label, true);
                }
            }
            else
            {
                if (expanded)
                {
                    height += EditorGUI.GetPropertyHeight(property, label, true);
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (FoldoutGroupAttribute)attribute;
            string groupKey = BuildGroupKey(property, attr);

            EnsureHeaderOwner(property, attr, groupKey);
            EnsureExpandedInitialized(attr, groupKey);

            bool isOwner = IsHeaderOwner(property, groupKey);
            bool expanded = expandedByGroupKey[groupKey];

            float y = position.y;
            if (isOwner)
            {
                // Draw header
                var headerRect = new Rect(position.x, y, position.width, HEADER_HEIGHT);
                bool newExpanded = EditorGUI.Foldout(headerRect, expanded, attr.GroupName, true, EditorStyles.foldoutHeader);
                if (newExpanded != expanded)
                {
                    expandedByGroupKey[groupKey] = newExpanded;
                    expanded = newExpanded;
                }
                y += HEADER_HEIGHT + SPACING;
            }

            if (expanded)
            {
                var contentRect = new Rect(position.x, y, position.width, EditorGUI.GetPropertyHeight(property, label, true));
                EditorGUI.PropertyField(contentRect, property, label, true);
            }
        }

        private static string BuildGroupKey(SerializedProperty property, FoldoutGroupAttribute attr)
        {
            int objId = property.serializedObject.targetObject != null ? property.serializedObject.targetObject.GetInstanceID() : 0;
            return objId + "::foldout::" + attr.GroupName;
        }

        private static void EnsureExpandedInitialized(FoldoutGroupAttribute attr, string groupKey)
        {
            if (!expandedByGroupKey.ContainsKey(groupKey))
            {
                expandedByGroupKey[groupKey] = attr.ExpandedByDefault;
            }
        }

        private static void EnsureHeaderOwner(SerializedProperty property, FoldoutGroupAttribute attr, string groupKey)
        {
            if (!headerOwnerByGroupKey.TryGetValue(groupKey, out string ownerKey))
            {
                headerOwnerByGroupKey[groupKey] = BuildOwnerKey(attr.Order, property.propertyPath);
            }
            else
            {
                (string ownerPath, int currentOrder) = ParseOwnerKey(ownerKey);
                if (attr.Order < currentOrder)
                {
                    headerOwnerByGroupKey[groupKey] = BuildOwnerKey(attr.Order, property.propertyPath);
                }
            }
        }

        private static string BuildOwnerKey(int order, string propertyPath)
        {
            return order.ToString() + "|" + propertyPath;
        }

        private static (string path, int order) ParseOwnerKey(string key)
        {
            int sep = key.IndexOf('|');
            if (sep <= 0) return (key, int.MaxValue);
            int.TryParse(key.Substring(0, sep), out int order);
            string path = key.Substring(sep + 1);
            return (path, order);
        }

        private static bool IsHeaderOwner(SerializedProperty property, string groupKey)
        {
            if (!headerOwnerByGroupKey.TryGetValue(groupKey, out string ownerKey)) return false;
            (string ownerPath, int _) = ParseOwnerKey(ownerKey);
            return ownerPath == property.propertyPath;
        }
    }
}
