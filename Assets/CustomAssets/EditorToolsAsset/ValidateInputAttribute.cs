using UnityEngine;

namespace CustomAssets.EditorTools
{
    public enum ValidateSeverity
    {
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// Attribute that validates a field using a method on the target object (or a static method).
    /// The validator method can have one of these signatures:
    /// - bool Validator(T value)
    /// - string Validator(T value)   // return null or empty when valid, otherwise an error message
    /// </summary>
    public class ValidateInputAttribute : PropertyAttribute
    {
        /// <summary>
        /// Name of the validator method. It can be an instance or static method on the target's type.
        /// The method should accept the field's type (or object) and return bool or string.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Message to display when validation fails (used when validator returns bool and false).
        /// If the validator returns a non-empty string, that string takes precedence.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Severity for the help box shown when invalid (Info, Warning, Error).
        /// </summary>
        public ValidateSeverity Severity { get; }

        public ValidateInputAttribute(string methodName)
        {
            MethodName = methodName;
            Message = "Invalid value";
            Severity = ValidateSeverity.Error;
        }

        public ValidateInputAttribute(string methodName, string message, ValidateSeverity severity = ValidateSeverity.Error)
        {
            MethodName = methodName;
            Message = string.IsNullOrEmpty(message) ? "Invalid value" : message;
            Severity = severity;
        }
    }
}
