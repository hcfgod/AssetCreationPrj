using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class NonZeroAttribute : PropertyAttribute
    {
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public NonZeroAttribute(string message = null, ValidateSeverity severity = ValidateSeverity.Error)
        {
            Message = message;
            Severity = severity;
        }
    }
}
