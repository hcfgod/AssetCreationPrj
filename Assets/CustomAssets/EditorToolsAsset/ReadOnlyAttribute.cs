using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Attribute that makes a field read-only in the Unity Inspector.
    /// The field will be grayed out and cannot be edited directly.
    /// </summary>
    /// <example>
    /// <code>
    /// [ReadOnly]
    /// public float health = 100f;
    /// 
    /// [ReadOnly]
    /// public string playerName = "Player";
    /// </code>
    /// </example>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Optional tooltip to display when hovering over the read-only field.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyAttribute.
        /// </summary>
        /// <param name="tooltip">Optional tooltip text to display on hover.</param>
        public ReadOnlyAttribute(string tooltip = "")
        {
            Tooltip = tooltip;
        }
    }
}
