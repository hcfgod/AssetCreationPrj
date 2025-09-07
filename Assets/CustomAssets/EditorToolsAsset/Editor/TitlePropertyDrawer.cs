using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Custom property drawer for the Title attribute.
    /// Renders colored titles with optional separators in the Unity Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitlePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Gets the height of the title field.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="label">The property label.</param>
        /// <returns>The height of the title field.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
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
        /// <param name="position">The position and size of the property field.</param>
        /// <param name="property">The serialized property to draw.</param>
        /// <param name="label">The property label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TitleAttribute titleAttribute = (TitleAttribute)attribute;
            
            // Calculate title position
            Rect titleRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            // Store original GUI state
            Color originalColor = GUI.color;
            GUIStyle originalStyle = GUI.skin.label;
            
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
                
                // Set separator color
                GUI.color = titleAttribute.SeparatorColor;
                
                // Draw separator line
                EditorGUI.DrawRect(separatorRect, titleAttribute.SeparatorColor);
                
                // Restore original color
                GUI.color = originalSeparatorColor;
            }
        }
    }
}
