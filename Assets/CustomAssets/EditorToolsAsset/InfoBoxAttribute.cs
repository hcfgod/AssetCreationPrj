using UnityEngine;

namespace CustomAssets.EditorTools
{
    public enum InfoBoxType
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Displays a HelpBox above the decorated field. Optionally controlled by a boolean field/property/method.
    /// </summary>
    public class InfoBoxAttribute : PropertyAttribute
    {
        /// <summary>
        /// The message to display in the box.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The type (icon/severity) of the message.
        /// </summary>
        public InfoBoxType Type { get; }

        /// <summary>
        /// Optional name of a boolean field/property/method to determine visibility.
        /// If provided and evaluates to false, the info box is hidden.
        /// </summary>
        public string VisibleIf { get; }

        /// <summary>
        /// Fixed height for the box. If <= 0, height is auto-calculated from content.
        /// </summary>
        public float FixedHeight { get; }

        public InfoBoxAttribute(string message, InfoBoxType type = InfoBoxType.Info, float height = 0f)
        {
            Message = message;
            Type = type;
            VisibleIf = null;
            FixedHeight = height;
        }

        public InfoBoxAttribute(string message, string visibleIf, InfoBoxType type = InfoBoxType.Info, float height = 0f)
        {
            Message = message;
            Type = type;
            VisibleIf = visibleIf;
            FixedHeight = height;
        }
    }
}
