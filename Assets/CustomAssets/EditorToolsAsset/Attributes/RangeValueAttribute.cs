using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class RangeValueAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public RangeValueAttribute(float min, float max, string message = null, ValidateSeverity severity = ValidateSeverity.Error)
        {
            Min = min;
            Max = max;
            Message = message;
            Severity = severity;
        }
    }
}
