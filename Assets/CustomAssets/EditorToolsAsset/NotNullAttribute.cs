using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class NotNullAttribute : PropertyAttribute
    {
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public NotNullAttribute(string message = null, ValidateSeverity severity = ValidateSeverity.Error)
        {
            Message = message;
            Severity = severity;
        }
    }
}
