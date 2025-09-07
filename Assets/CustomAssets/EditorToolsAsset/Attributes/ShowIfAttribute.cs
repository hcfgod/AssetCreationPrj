using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Attribute that conditionally shows a field in the Unity Inspector based on another field's value.
    /// The field will only be visible when the specified condition is met.
    /// </summary>
    /// <example>
    /// <code>
    /// public bool showAdvancedSettings = false;
    /// 
    /// [ShowIf("showAdvancedSettings")]
    /// public float advancedValue = 1.0f;
    /// 
    /// [ShowIf("health", 100f)]
    /// public string maxHealthMessage = "At full health!";
    /// 
    /// [ShowIf("playerType", PlayerType.Admin)]
    /// public bool adminOnlySetting = true;
    /// </code>
    /// </example>
    public class ShowIfAttribute : ConditionalAttribute
    {
        /// <summary>
        /// Initializes a new instance of the ShowIfAttribute that shows the field when the specified field is truthy.
        /// </summary>
        /// <param name="fieldName">The name of the field to check.</param>
        /// <param name="tooltip">Optional tooltip text to display on hover.</param>
        public ShowIfAttribute(string fieldName, string tooltip = "") : base(fieldName, null, tooltip)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ShowIfAttribute that shows the field when the specified field equals the expected value.
        /// </summary>
        /// <param name="fieldName">The name of the field to check.</param>
        /// <param name="expectedValue">The expected value to compare against.</param>
        /// <param name="tooltip">Optional tooltip text to display on hover.</param>
        public ShowIfAttribute(string fieldName, object expectedValue, string tooltip = "") : base(fieldName, expectedValue, tooltip)
        {
        }
    }
}
