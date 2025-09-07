using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(ValidateInputAttribute))]
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
            var attr = (ValidateInputAttribute)attribute;
            var targets = property.serializedObject.targetObjects;

            // If any target is invalid, we display the message
            foreach (var target in targets)
            {
                var res = EvaluateForTarget(target, property, attr);
                if (!res.isValid)
                    return res;
            }

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
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
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
