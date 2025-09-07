using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(ValidateInputAttribute))]
    [CustomPropertyDrawer(typeof(MinValueAttribute))]
    [CustomPropertyDrawer(typeof(MaxValueAttribute))]
    [CustomPropertyDrawer(typeof(RangeValueAttribute))]
    [CustomPropertyDrawer(typeof(RegexMatchAttribute))]
    [CustomPropertyDrawer(typeof(NotNullAttribute))]
    [CustomPropertyDrawer(typeof(NotEmptyAttribute))]
    [CustomPropertyDrawer(typeof(NonZeroAttribute))]
    public class ValidateInputPropertyDrawer : PropertyDrawer
    {
        private struct ValidationResult
        {
            public bool isValid;
            public string message;
            public MessageType messageType;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = EditorGUI.GetPropertyHeight(property, label, true);
            var result = Evaluate(property);
            if (result.isValid || string.IsNullOrEmpty(result.message))
                return baseHeight;

            float width = EditorGUIUtility.currentViewWidth - 40f;
            float helpHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(result.message), width);
            return baseHeight + helpHeight + 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the field
            float baseHeight = EditorGUI.GetPropertyHeight(property, label, true);
            Rect fieldRect = new Rect(position.x, position.y, position.width, baseHeight);
            EditorGUI.PropertyField(fieldRect, property, label, true);

            // Evaluate and draw help box if invalid
            var result = Evaluate(property);
            if (!result.isValid && !string.IsNullOrEmpty(result.message))
            {
                float width = position.width;
                float helpHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(result.message), width);
                Rect helpRect = new Rect(position.x, position.y + baseHeight + 2f, width, helpHeight);
                EditorGUI.HelpBox(helpRect, result.message, result.messageType);
            }
        }

        private ValidationResult Evaluate(SerializedProperty property)
        {
            // Route to specific validators
            if (attribute is ValidateInputAttribute vi)
            {
                var targets = property.serializedObject.targetObjects;
                foreach (var target in targets)
                {
                    var res = EvaluateForTarget(target, property, vi);
                    if (!res.isValid) return res;
                }
                return new ValidationResult { isValid = true };
            }
            if (attribute is MinValueAttribute minAttr) return EvaluateMin(property, minAttr);
            if (attribute is MaxValueAttribute maxAttr) return EvaluateMax(property, maxAttr);
            if (attribute is RangeValueAttribute rangeAttr) return EvaluateRange(property, rangeAttr);
            if (attribute is RegexMatchAttribute regexAttr) return EvaluateRegex(property, regexAttr);
            if (attribute is NotNullAttribute notNullAttr) return EvaluateNotNull(property, notNullAttr);
            if (attribute is NotEmptyAttribute notEmptyAttr) return EvaluateNotEmpty(property, notEmptyAttr);
            if (attribute is NonZeroAttribute nonZeroAttr) return EvaluateNonZero(property, nonZeroAttr);

            return new ValidationResult
            {
                isValid = true,
                message = null,
                messageType = MessageType.None
            };
        }

        private ValidationResult EvaluateForTarget(UnityEngine.Object target, SerializedProperty property, ValidateInputAttribute attr)
        {
            if (target == null || string.IsNullOrEmpty(attr.MethodName))
            {
                return new ValidationResult { isValid = true };
            }

            // Get current field value via reflection (safer for nested/managed refs)
            object value = GetFieldValue(target, property);

            // Find validator method
            Type targetType = target.GetType();
            MethodInfo method = FindValidatorMethod(targetType, attr.MethodName, fieldInfo.FieldType) ??
                                FindValidatorMethod(targetType, attr.MethodName, typeof(object));

            if (method == null)
            {
                // No validator found - treat as valid to avoid blocking inspector
                return new ValidationResult { isValid = true };
            }

            try
            {
                object instance = method.IsStatic ? null : target;
                object result = method.GetParameters().Length == 1
                    ? method.Invoke(instance, new object[] { value })
                    : method.Invoke(instance, null);

                if (result is bool ok)
                {
                    return ok
                        ? new ValidationResult { isValid = true }
                        : new ValidationResult
                        {
                            isValid = false,
                            message = attr.Message,
                            messageType = ToMessageType(attr.Severity)
                        };
                }
                if (result is string msg)
                {
                    bool success = string.IsNullOrEmpty(msg);
                    return success
                        ? new ValidationResult { isValid = true }
                        : new ValidationResult
                        {
                            isValid = false,
                            message = msg,
                            messageType = ToMessageType(attr.Severity)
                        };
                }
            }
            catch (Exception e)
            {
                return new ValidationResult
                {
                    isValid = false,
                    message = $"ValidateInput: Exception in '{attr.MethodName}': {e.Message}",
                    messageType = MessageType.Error
                };
            }

            return new ValidationResult { isValid = true };
        }

        private static MessageType ToMessageType(ValidateSeverity severity)
        {
            switch (severity)
            {
                case ValidateSeverity.Info: return MessageType.Info;
                case ValidateSeverity.Warning: return MessageType.Warning;
                default: return MessageType.Error;
            }
        }

        private ValidationResult EvaluateMin(SerializedProperty property, MinValueAttribute attr)
        {
            string msg = string.IsNullOrEmpty(attr.Message) ? $"Value must be ≥ {attr.Min}" : attr.Message;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return new ValidationResult
                    {
                        isValid = property.intValue >= attr.Min,
                        message = property.intValue >= attr.Min ? null : msg,
                        messageType = ToMessageType(attr.Severity)
                    };
                case SerializedPropertyType.Float:
                    return new ValidationResult
                    {
                        isValid = property.floatValue >= attr.Min,
                        message = property.floatValue >= attr.Min ? null : msg,
                        messageType = ToMessageType(attr.Severity)
                    };
                default:
                    return new ValidationResult { isValid = true };
            }
        }

        private ValidationResult EvaluateMax(SerializedProperty property, MaxValueAttribute attr)
        {
            string msg = string.IsNullOrEmpty(attr.Message) ? $"Value must be ≤ {attr.Max}" : attr.Message;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return new ValidationResult
                    {
                        isValid = property.intValue <= attr.Max,
                        message = property.intValue <= attr.Max ? null : msg,
                        messageType = ToMessageType(attr.Severity)
                    };
                case SerializedPropertyType.Float:
                    return new ValidationResult
                    {
                        isValid = property.floatValue <= attr.Max,
                        message = property.floatValue <= attr.Max ? null : msg,
                        messageType = ToMessageType(attr.Severity)
                    };
                default:
                    return new ValidationResult { isValid = true };
            }
        }

        private ValidationResult EvaluateRange(SerializedProperty property, RangeValueAttribute attr)
        {
            string msg = string.IsNullOrEmpty(attr.Message) ? $"Value must be in [{attr.Min}, {attr.Max}]" : attr.Message;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    bool okI = property.intValue >= attr.Min && property.intValue <= attr.Max;
                    return new ValidationResult { isValid = okI, message = okI ? null : msg, messageType = ToMessageType(attr.Severity) };
                case SerializedPropertyType.Float:
                    bool okF = property.floatValue >= attr.Min && property.floatValue <= attr.Max;
                    return new ValidationResult { isValid = okF, message = okF ? null : msg, messageType = ToMessageType(attr.Severity) };
                default:
                    return new ValidationResult { isValid = true };
            }
        }

        private ValidationResult EvaluateRegex(SerializedProperty property, RegexMatchAttribute attr)
        {
            if (property.propertyType != SerializedPropertyType.String) return new ValidationResult { isValid = true };
            string value = property.stringValue ?? string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                return new ValidationResult { isValid = attr.AllowEmpty, message = attr.AllowEmpty ? null : (attr.Message ?? "Value cannot be empty"), messageType = ToMessageType(attr.Severity) };
            }
            bool match = false;
            try { match = Regex.IsMatch(value, attr.Pattern); } catch { match = false; }
            return new ValidationResult
            {
                isValid = match,
                message = match ? null : (attr.Message ?? "Value does not match the required pattern"),
                messageType = ToMessageType(attr.Severity)
            };
        }

        private ValidationResult EvaluateNotNull(SerializedProperty property, NotNullAttribute attr)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    bool okObj = property.objectReferenceValue != null;
                    return new ValidationResult { isValid = okObj, message = okObj ? null : (attr.Message ?? "Reference cannot be null"), messageType = ToMessageType(attr.Severity) };
                case SerializedPropertyType.String:
                    bool okStr = !string.IsNullOrEmpty(property.stringValue);
                    return new ValidationResult { isValid = okStr, message = okStr ? null : (attr.Message ?? "String cannot be null or empty"), messageType = ToMessageType(attr.Severity) };
                default:
                    return new ValidationResult { isValid = true };
            }
        }

        private ValidationResult EvaluateNotEmpty(SerializedProperty property, NotEmptyAttribute attr)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                bool ok = !string.IsNullOrEmpty(property.stringValue);
                return new ValidationResult { isValid = ok, message = ok ? null : (attr.Message ?? "String cannot be empty"), messageType = ToMessageType(attr.Severity) };
            }
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                bool ok = property.arraySize > 0;
                return new ValidationResult { isValid = ok, message = ok ? null : (attr.Message ?? "Collection cannot be empty"), messageType = ToMessageType(attr.Severity) };
            }
            return new ValidationResult { isValid = true };
        }

        private ValidationResult EvaluateNonZero(SerializedProperty property, NonZeroAttribute attr)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                bool ok = property.intValue != 0;
                return new ValidationResult { isValid = ok, message = ok ? null : (attr.Message ?? "Value cannot be zero"), messageType = ToMessageType(attr.Severity) };
            }
            if (property.propertyType == SerializedPropertyType.Float)
            {
                bool ok = Mathf.Abs(property.floatValue) > Mathf.Epsilon;
                return new ValidationResult { isValid = ok, message = ok ? null : (attr.Message ?? "Value cannot be zero"), messageType = ToMessageType(attr.Severity) };
            }
            return new ValidationResult { isValid = true };
        }

        private MethodInfo FindValidatorMethod(Type type, string methodName, Type paramType)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            foreach (var m in type.GetMethods(flags))
            {
                if (m.Name != methodName) continue;
                var pars = m.GetParameters();
                if (pars.Length == 1 && pars[0].ParameterType.IsAssignableFrom(paramType))
                    return m;
                if (pars.Length == 0) // Allow parameter-less validator
                    return m;
            }
            return null;
        }

        private object GetFieldValue(UnityEngine.Object target, SerializedProperty property)
        {
            try
            {
                // Use reflection via fieldInfo when possible
                if (fieldInfo != null)
                {
                    return fieldInfo.GetValue(target);
                }
            }
            catch { }

            // Fallbacks for common types from SerializedProperty
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer: return property.intValue;
                case SerializedPropertyType.Float: return property.floatValue;
                case SerializedPropertyType.Boolean: return property.boolValue;
                case SerializedPropertyType.String: return property.stringValue;
                case SerializedPropertyType.Vector2: return property.vector2Value;
                case SerializedPropertyType.Vector3: return property.vector3Value;
                case SerializedPropertyType.Vector2Int: return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return property.vector3IntValue;
                case SerializedPropertyType.ObjectReference: return property.objectReferenceValue;
                default: return null;
            }
        }
    }
}
