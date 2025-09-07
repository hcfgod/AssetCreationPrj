using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class MinValueAttribute : PropertyAttribute
    {
        public float Min { get; }
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public MinValueAttribute(float min, string message = null, ValidateSeverity severity = ValidateSeverity.Error)
        {
            Min = min;
            Message = message;
            Severity = severity;
        }
    }
}
