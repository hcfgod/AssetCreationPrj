using UnityEngine;

namespace CustomAssets.EditorTools
{
    public class RegexMatchAttribute : PropertyAttribute
    {
        public string Pattern { get; }
        public bool AllowEmpty { get; }
        public string Message { get; }
        public ValidateSeverity Severity { get; }

        public RegexMatchAttribute(string pattern, string message = null, ValidateSeverity severity = ValidateSeverity.Error, bool allowEmpty = true)
        {
            Pattern = pattern;
            Message = message;
            Severity = severity;
            AllowEmpty = allowEmpty;
        }
    }
}
