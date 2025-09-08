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
                    
                    theme.AccentColor       = EditorGUILayout.ColorField("Accent", theme.AccentColor);
                    theme.SeparatorColor    = EditorGUILayout.ColorField("Separator", theme.SeparatorColor);
                    theme.ToggleActiveColor = EditorGUILayout.ColorField("Toggle Active", theme.ToggleActiveColor);

                    EditorGUILayout.Space();
                    theme.TextColor            = EditorGUILayout.ColorField("Text", theme.TextColor);
                    theme.SubTextColor         = EditorGUILayout.ColorField("Sub Text", theme.SubTextColor);
                    theme.HeaderTextColor      = EditorGUILayout.ColorField("Header Text", theme.HeaderTextColor);
                    theme.PanelBackgroundColor = EditorGUILayout.ColorField("Panel Background", theme.PanelBackgroundColor);
                    theme.PanelBorderColor     = EditorGUILayout.ColorField("Panel Border", theme.PanelBorderColor);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Button Colors", EditorStyles.boldLabel);
                    theme.ButtonNormalColor = EditorGUILayout.ColorField("Button Background", theme.ButtonNormalColor);
                    theme.ButtonTextColor   = EditorGUILayout.ColorField("Button Text", theme.ButtonTextColor);
                    theme.ButtonBorderColor = EditorGUILayout.ColorField("Button Border", theme.ButtonBorderColor);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Progress Bar Colors", EditorStyles.boldLabel);
                    theme.ProgressBackgroundColor = EditorGUILayout.ColorField("Progress Background", theme.ProgressBackgroundColor);
                    theme.ProgressFillColor       = EditorGUILayout.ColorField("Progress Fill", theme.ProgressFillColor);
                    theme.ProgressTextColor       = EditorGUILayout.ColorField("Progress Text", theme.ProgressTextColor);

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

