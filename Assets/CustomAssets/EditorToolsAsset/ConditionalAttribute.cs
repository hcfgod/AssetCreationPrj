using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Base class for conditional attributes to enable shared functionality.
    /// Provides common properties and behavior for ShowIf and HideIf attributes.
    /// </summary>
    public abstract class ConditionalAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the field to check for the condition.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// The expected value to compare against. If null, checks for truthy values.
        /// </summary>
        public object ExpectedValue { get; }

        /// <summary>
        /// Optional tooltip to display when hovering over the field.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Initializes a new instance of the ConditionalAttribute.
        /// </summary>
        /// <param name="fieldName">The name of the field to check.</param>
        /// <param name="expectedValue">The expected value to compare against.</param>
        /// <param name="tooltip">Optional tooltip text to display on hover.</param>
        protected ConditionalAttribute(string fieldName, object expectedValue, string tooltip = "")
        {
            FieldName = fieldName;
            ExpectedValue = expectedValue;
            Tooltip = tooltip;
        }
    }
}
