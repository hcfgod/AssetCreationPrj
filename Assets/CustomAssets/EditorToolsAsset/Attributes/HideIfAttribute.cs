using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Attribute that conditionally hides a field in the Unity Inspector based on another field's value.
    /// The field will be hidden when the specified condition is met.
    /// </summary>
    /// <example>
    /// <code>
    /// public bool hideAdvancedSettings = true;
    /// 
    /// [HideIf("hideAdvancedSettings")]
    /// public float advancedValue = 1.0f;
    /// 
    /// [HideIf("health", 0f)]
    /// public string aliveMessage = "Player is alive!";
    /// 
    /// [HideIf("playerType", PlayerType.Guest)]
    /// public bool loggedInOnlySetting = true;
    /// </code>
    /// </example>
    public class HideIfAttribute : ConditionalAttribute
    {
        /// <summary>
        /// Initializes a new instance of the HideIfAttribute that hides the field when the specified field is truthy.
        /// </summary>
        /// <param name="fieldName">The name of the field to check.</param>
        /// <param name="tooltip">Optional tooltip text to display on hover.</param>
        public HideIfAttribute(string fieldName, string tooltip = "") : base(fieldName, null, tooltip)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HideIfAttribute that hides the field when the specified field equals the expected value.
        /// </summary>
        /// <param name="fieldName">The name of the field to check.</param>
        /// <param name="expectedValue">The expected value to compare against.</param>
        /// <param name="tooltip">Optional tooltip text to display on hover.</param>
        public HideIfAttribute(string fieldName, object expectedValue, string tooltip = "") : base(fieldName, expectedValue, tooltip)
        {
        }
    }
}
