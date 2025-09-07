using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;

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
        /// <summary>
        /// Gets the height of the title decorator.
        /// </summary>
        public override float GetHeight()
        {
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
