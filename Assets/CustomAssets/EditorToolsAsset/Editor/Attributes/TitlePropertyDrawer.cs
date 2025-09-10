using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;
using System.Reflection;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Decorator drawer for the Title attribute.
    /// Renders colored titles with optional separators in the Unity Inspector without
    /// interfering with the property's own drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDecoratorDrawer : DecoratorDrawer
    {
        private static FieldInfo GetBackingFieldInfo(DecoratorDrawer drawer)
        {
            // Unity stores the decorated field info in a private member on the base type
            var fi = typeof(DecoratorDrawer).GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            return fi != null ? fi.GetValue(drawer) as FieldInfo : null;
        }

        private bool IsVisibleForActiveTab()
        {
            // Try to find a TabGroupAttribute on the same field; if found, only draw when that tab is active.
            var field = GetBackingFieldInfo(this);
            if (field == null)
                return true; // Can't determine context; draw by default

            var tabAttr = field.GetCustomAttribute<TabGroupAttribute>(true);
            if (tabAttr == null)
                return true; // Not part of a tab group

            // Consult TabGroupPropertyDrawer's global state keyed by group name only.
            return TabGroupPropertyDrawer.IsTabActiveForGroupName(tabAttr.GroupName, tabAttr.TabName);
        }

        /// <summary>
        /// Gets the height of the title decorator.
        /// </summary>
        public override float GetHeight()
        {
            if (!IsVisibleForActiveTab())
                return 0f;

            TitleAttribute titleAttribute = (TitleAttribute)attribute;
            float height = EditorGUIUtility.singleLineHeight;

            if (titleAttribute.ShowSeparator)
            {
                height += 2f; // Space for separator line
            }

            return height;
        }

        /// <summary>
        /// Draws the title with custom styling.
        /// </summary>
        /// <param name="position">The position and size of the decorator.</param>
        public override void OnGUI(Rect position)
        {
            if (!IsVisibleForActiveTab())
                return;

            TitleAttribute titleAttribute = (TitleAttribute)attribute;

            // Calculate title position
            Rect titleRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            // Store original GUI state
            Color originalColor = GUI.color;

            // Create custom style for the title
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = titleAttribute.FontSize,
                alignment = TextAnchor.MiddleLeft
            };

            // Set title color
            GUI.color = titleAttribute.TitleColor;

            // Draw the title
            EditorGUI.LabelField(titleRect, titleAttribute.Title, titleStyle);

            // Restore original GUI state
            GUI.color = originalColor;

            // Draw separator line if enabled
            if (titleAttribute.ShowSeparator)
            {
                Rect separatorRect = new Rect(
                    position.x,
                    position.y + EditorGUIUtility.singleLineHeight + 1f,
                    position.width,
                    1f
                );

                // Store original color
                Color originalSeparatorColor = GUI.color;

                // Set separator color and draw
                GUI.color = titleAttribute.SeparatorColor;
                EditorGUI.DrawRect(separatorRect, titleAttribute.SeparatorColor);

                // Restore original color
                GUI.color = originalSeparatorColor;
            }
        }
    }
}
