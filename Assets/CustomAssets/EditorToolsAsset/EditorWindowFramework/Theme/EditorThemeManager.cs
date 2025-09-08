#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace CustomAssets.EditorTools
{
    public static class EditorThemeManager
    {
        private const string GlobalPrefix = "KEditorWindow.GlobalTheme.";

        private static EditorTheme _globalTheme;
        private static readonly Dictionary<Type, EditorTheme> _windowThemes = new Dictionary<Type, EditorTheme>();
        private static readonly HashSet<Type> _windowsInheritGlobal = new HashSet<Type>();

        public static event Action ThemeChanged;

        public static EditorTheme GetGlobalTheme()
        {
            if (_globalTheme == null)
            {
                _globalTheme = EditorTheme.LoadFromPrefs(GlobalPrefix, EditorTheme.Default());
            }
            return _globalTheme;
        }

        public static EditorTheme GetEditableGlobalThemeClone()
        {
            return GetGlobalTheme().Clone();
        }

        public static void SaveGlobalTheme(EditorTheme theme)
        {
            if (theme == null) return;
            theme.SaveToPrefs(GlobalPrefix);
            _globalTheme = theme.Clone();

            // Propagate to windows that inherit from global
            foreach (var t in _windowsInheritGlobal)
            {
                if (_windowThemes.TryGetValue(t, out var wTheme))
                {
                    CopyInto(_globalTheme, wTheme);
                }
            }

            RaiseThemeChanged();
        }

        public static EditorTheme GetThemeForWindow(Type windowType)
        {
            if (windowType == null) throw new ArgumentNullException(nameof(windowType));

            if (_windowThemes.TryGetValue(windowType, out var theme))
                return theme;

            string prefix = GetWindowPrefix(windowType);
            EditorTheme loaded;
            if (HasAnyColor(prefix))
            {
                loaded = EditorTheme.LoadFromPrefs(prefix, EditorTheme.Default());
                _windowsInheritGlobal.Remove(windowType);
            }
            else
            {
                loaded = GetGlobalTheme().Clone();
                _windowsInheritGlobal.Add(windowType);
            }

            _windowThemes[windowType] = loaded;
            return loaded;
        }

        public static EditorTheme GetEditableThemeForWindow(Type windowType)
        {
            return GetThemeForWindow(windowType);
        }

        public static void SaveThemeForWindow(Type windowType, EditorTheme theme)
        {
            if (windowType == null || theme == null) return;
            string prefix = GetWindowPrefix(windowType);
            theme.SaveToPrefs(prefix);
            _windowThemes[windowType] = theme;
            _windowsInheritGlobal.Remove(windowType);
            RaiseThemeChanged();
        }

        public static void ResetThemeForWindow(Type windowType)
        {
            if (windowType == null) return;
            string prefix = GetWindowPrefix(windowType);
            EditorTheme.DeleteFromPrefs(prefix);
            var newTheme = GetGlobalTheme().Clone();
            _windowThemes[windowType] = newTheme;
            _windowsInheritGlobal.Add(windowType);
            RaiseThemeChanged();
        }

        public static void ResetGlobalThemeToDefaults(bool autoDetect = true)
        {
            var def = autoDetect ? EditorTheme.Default() : (EditorGUIUtility.isProSkin ? EditorTheme.DefaultDark() : EditorTheme.DefaultLight());
            SaveGlobalTheme(def);
        }

        public static string GetWindowPrefix(Type windowType)
        {
            return $"KEditorWindow.{windowType.FullName}.theme.";
        }

        private static bool HasAnyColor(string prefix)
        {
            return EditorPrefs.HasKey(prefix + "bg.r") ||
                   EditorPrefs.HasKey(prefix + "accent.r") ||
                   EditorPrefs.HasKey(prefix + "sep.r") ||
                   EditorPrefs.HasKey(prefix + "footer.r") ||
                   EditorPrefs.HasKey(prefix + "toggle.r");
        }

        private static void CopyInto(EditorTheme src, EditorTheme dst)
        {
            if (src == null || dst == null) return;
            dst.BackgroundColor = src.BackgroundColor;
            dst.AccentColor = src.AccentColor;
            dst.SeparatorColor = src.SeparatorColor;
            dst.FooterBorderColor = src.FooterBorderColor;
            dst.ToggleActiveColor = src.ToggleActiveColor;
        }

        private static void RaiseThemeChanged()
        {
            try { ThemeChanged?.Invoke(); } catch { }
        }
    }
}
#endif

