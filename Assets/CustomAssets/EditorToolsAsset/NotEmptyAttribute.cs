using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class NotEmptyAttribute : PropertyAttribute
    {
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public NotEmptyAttribute(string message = null, ValidateSeverity severity = ValidateSeverity.Error)
        {
            Message = message;
            Severity = severity;
        }
    }
}
