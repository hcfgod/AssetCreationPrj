using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Custom editor that adds buttons for methods marked with the Button attribute.
    /// This editor automatically discovers and renders buttons for all Button attributes in the target class.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class ButtonEditor : UnityEditor.Editor
    {
        private List<ButtonInfo> buttons = new List<ButtonInfo>();
        private bool buttonsInitialized = false;

        private struct ButtonInfo
        {
            public MethodInfo method;
            public ButtonAttribute attribute;
            public string displayName;
            public int order;
        }

        private void OnEnable()
        {
            buttonsInitialized = false;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Initialize buttons if not done yet
            if (!buttonsInitialized)
            {
                InitializeButtons();
                buttonsInitialized = true;
            }

            // Draw buttons if any exist
            if (buttons.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                
                // Sort buttons by order
                var sortedButtons = buttons.OrderBy(b => b.order).ToList();
                
                foreach (var buttonInfo in sortedButtons)
                {
                    DrawButton(buttonInfo);
                }
            }
        }

        private void InitializeButtons()
        {
            buttons.Clear();
            
            Type targetType = target.GetType();
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                ButtonAttribute buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttr != null)
                {
                    // Validate method signature
                    if (method.GetParameters().Length > 0)
                    {
                        Debug.LogWarning($"Button method '{method.Name}' in {targetType.Name} must have no parameters. Skipping.");
                        continue;
                    }

                    if (method.ReturnType != typeof(void))
                    {
                        Debug.LogWarning($"Button method '{method.Name}' in {targetType.Name} must return void. Skipping.");
                        continue;
                    }

                    string displayName = !string.IsNullOrEmpty(buttonAttr.ButtonText) 
                        ? buttonAttr.ButtonText 
                        : ObjectNames.NicifyVariableName(method.Name);

                    buttons.Add(new ButtonInfo
                    {
                        method = method,
                        attribute = buttonAttr,
                        displayName = displayName,
                        order = buttonAttr.Order
                    });
                }
            }
        }

        private void DrawButton(ButtonInfo buttonInfo)
        {
            ButtonAttribute attr = buttonInfo.attribute;
            MethodInfo method = buttonInfo.method;

            // Check if button should be enabled based on play mode
            bool shouldEnable = true;
            if (attr.PlayModeOnly && !Application.isPlaying)
            {
                shouldEnable = false;
            }
            else if (attr.EditModeOnly && Application.isPlaying)
            {
                shouldEnable = false;
            }

            // Store original GUI state
            Color originalColor = GUI.color;
            Color originalBackgroundColor = GUI.backgroundColor;
            Color originalContentColor = GUI.contentColor;

            // Apply custom colors if specified
            if (attr.HasCustomColors)
            {
                if (attr.BackgroundColor != Color.clear)
                {
                    GUI.backgroundColor = attr.BackgroundColor;
                }
                if (attr.TextColor != Color.white)
                {
                    GUI.contentColor = attr.TextColor;
                }
            }

            // Calculate button dimensions
            float buttonWidth = attr.ButtonWidth > 0 ? attr.ButtonWidth : EditorGUIUtility.currentViewWidth - 20f;
            float buttonHeight = attr.ButtonHeight;

            // Create button style
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal
            };

            // Draw button
            EditorGUI.BeginDisabledGroup(!shouldEnable);
            
            if (GUILayout.Button(buttonInfo.displayName, buttonStyle, GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)))
            {
                if (attr.ShowConfirmation)
                {
                    if (EditorUtility.DisplayDialog("Confirmation", attr.ConfirmationMessage, "Yes", "No"))
                    {
                        ExecuteButtonMethod(method);
                    }
                }
                else
                {
                    ExecuteButtonMethod(method);
                }
            }
            
            EditorGUI.EndDisabledGroup();

            // Restore original GUI state
            GUI.color = originalColor;
            GUI.backgroundColor = originalBackgroundColor;
            GUI.contentColor = originalContentColor;
        }

        private void ExecuteButtonMethod(MethodInfo method)
        {
            try
            {
                // Record undo for the target object
                Undo.RecordObject(target, $"Execute {method.Name}");
                
                // Execute the method
                method.Invoke(target, null);
                
                // Mark the object as dirty so changes are saved
                EditorUtility.SetDirty(target);
                
                // Repaint the inspector to reflect any changes
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error executing button method '{method.Name}': {e.Message}");
            }
        }
    }

    /// <summary>
    /// Custom editor for ScriptableObjects that adds buttons for methods marked with the Button attribute.
    /// </summary>
    [CustomEditor(typeof(ScriptableObject), true)]
    [CanEditMultipleObjects]
    public class ScriptableObjectButtonEditor : UnityEditor.Editor
    {
        private List<ButtonInfo> buttons = new List<ButtonInfo>();
        private bool buttonsInitialized = false;

        private struct ButtonInfo
        {
            public MethodInfo method;
            public ButtonAttribute attribute;
            public string displayName;
            public int order;
        }

        private void OnEnable()
        {
            buttonsInitialized = false;
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Initialize buttons if not done yet
            if (!buttonsInitialized)
            {
                InitializeButtons();
                buttonsInitialized = true;
            }

            // Draw buttons if any exist
            if (buttons.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
                
                // Sort buttons by order
                var sortedButtons = buttons.OrderBy(b => b.order).ToList();
                
                foreach (var buttonInfo in sortedButtons)
                {
                    DrawButton(buttonInfo);
                }
            }
        }

        private void InitializeButtons()
        {
            buttons.Clear();
            
            Type targetType = target.GetType();
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                ButtonAttribute buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttr != null)
                {
                    // Validate method signature
                    if (method.GetParameters().Length > 0)
                    {
                        Debug.LogWarning($"Button method '{method.Name}' in {targetType.Name} must have no parameters. Skipping.");
                        continue;
                    }

                    if (method.ReturnType != typeof(void))
                    {
                        Debug.LogWarning($"Button method '{method.Name}' in {targetType.Name} must return void. Skipping.");
                        continue;
                    }

                    string displayName = !string.IsNullOrEmpty(buttonAttr.ButtonText) 
                        ? buttonAttr.ButtonText 
                        : ObjectNames.NicifyVariableName(method.Name);

                    buttons.Add(new ButtonInfo
                    {
                        method = method,
                        attribute = buttonAttr,
                        displayName = displayName,
                        order = buttonAttr.Order
                    });
                }
            }
        }

        private void DrawButton(ButtonInfo buttonInfo)
        {
            ButtonAttribute attr = buttonInfo.attribute;
            MethodInfo method = buttonInfo.method;

            // Check if button should be enabled based on play mode
            bool shouldEnable = true;
            if (attr.PlayModeOnly && !Application.isPlaying)
            {
                shouldEnable = false;
            }
            else if (attr.EditModeOnly && Application.isPlaying)
            {
                shouldEnable = false;
            }

            // Store original GUI state
            Color originalColor = GUI.color;
            Color originalBackgroundColor = GUI.backgroundColor;
            Color originalContentColor = GUI.contentColor;

            // Apply custom colors if specified
            if (attr.HasCustomColors)
            {
                if (attr.BackgroundColor != Color.clear)
                {
                    GUI.backgroundColor = attr.BackgroundColor;
                }
                if (attr.TextColor != Color.white)
                {
                    GUI.contentColor = attr.TextColor;
                }
            }

            // Calculate button dimensions
            float buttonWidth = attr.ButtonWidth > 0 ? attr.ButtonWidth : EditorGUIUtility.currentViewWidth - 20f;
            float buttonHeight = attr.ButtonHeight;

            // Create button style
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal
            };

            // Draw button
            EditorGUI.BeginDisabledGroup(!shouldEnable);
            
            if (GUILayout.Button(buttonInfo.displayName, buttonStyle, GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)))
            {
                if (attr.ShowConfirmation)
                {
                    if (EditorUtility.DisplayDialog("Confirmation", attr.ConfirmationMessage, "Yes", "No"))
                    {
                        ExecuteButtonMethod(method);
                    }
                }
                else
                {
                    ExecuteButtonMethod(method);
                }
            }
            
            EditorGUI.EndDisabledGroup();

            // Restore original GUI state
            GUI.color = originalColor;
            GUI.backgroundColor = originalBackgroundColor;
            GUI.contentColor = originalContentColor;
        }

        private void ExecuteButtonMethod(MethodInfo method)
        {
            try
            {
                // Record undo for the target object
                Undo.RecordObject(target, $"Execute {method.Name}");
                
                // Execute the method
                method.Invoke(target, null);
                
                // Mark the object as dirty so changes are saved
                EditorUtility.SetDirty(target);
                
                // Repaint the inspector to reflect any changes
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error executing button method '{method.Name}': {e.Message}");
            }
        }
    }
}
