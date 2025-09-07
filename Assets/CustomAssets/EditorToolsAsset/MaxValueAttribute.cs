using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class MaxValueAttribute : PropertyAttribute
    {
        public float Max { get; }
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public MaxValueAttribute(float max, string message = null, ValidateSeverity severity = ValidateSeverity.Error)
        {
            Max = max;
            Message = message;
            Severity = severity;
        }
    }
}
