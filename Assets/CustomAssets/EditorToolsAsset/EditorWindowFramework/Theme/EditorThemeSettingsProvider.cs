#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace CustomAssets.EditorTools
{
    internal static class EditorThemePreferencesProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateThemePreferences()
        {
            var provider = new SettingsProvider("Preferences/KEditor/Theme", SettingsScope.User)
            {
                label = "KEditor Theme",
                guiHandler = (searchContext) =>
                {
                    var theme = EditorThemeManager.GetGlobalTheme();

                    EditorGUILayout.LabelField("Global Theme", EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    theme.BackgroundColor   = EditorGUILayout.ColorField("Background", theme.BackgroundColor);
                    theme.AccentColor       = EditorGUILayout.ColorField("Accent", theme.AccentColor);
                    theme.SeparatorColor    = EditorGUILayout.ColorField("Separator", theme.SeparatorColor);
                    theme.FooterBorderColor = EditorGUILayout.ColorField("Footer Border", theme.FooterBorderColor);
                    theme.ToggleActiveColor = EditorGUILayout.ColorField("Toggle Active", theme.ToggleActiveColor);
                    bool changed = EditorGUI.EndChangeCheck();

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Use Light Defaults"))
                    {
                        theme = EditorTheme.DefaultLight();
                        changed = true;
                    }
                    if (GUILayout.Button("Use Dark Defaults"))
                    {
                        theme = EditorTheme.DefaultDark();
                        changed = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (changed)
                    {
                        EditorThemeManager.SaveGlobalTheme(theme);
                    }
                }
            };

            return provider;
        }
    }
}
#endif

