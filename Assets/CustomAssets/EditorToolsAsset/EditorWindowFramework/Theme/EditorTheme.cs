#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace CustomAssets.EditorTools
{
    [System.Serializable]
    public class EditorTheme
    {
        public Color BackgroundColor = new Color(0.13f, 0.13f, 0.13f);
        public Color AccentColor = new Color(0.2f, 0.6f, 1f);
        public Color SeparatorColor = new Color(0.3f, 0.3f, 0.3f);
        public Color FooterBorderColor = new Color(0f, 0f, 0f, 0.15f);
        public Color ToggleActiveColor = new Color(0.7f, 1f, 0.7f);

        public EditorTheme Clone()
        {
            return new EditorTheme
            {
                BackgroundColor = this.BackgroundColor,
                AccentColor = this.AccentColor,
                SeparatorColor = this.SeparatorColor,
                FooterBorderColor = this.FooterBorderColor,
                ToggleActiveColor = this.ToggleActiveColor
            };
        }

        public static EditorTheme Default()
        {
            return EditorGUIUtility.isProSkin ? DefaultDark() : DefaultLight();
        }

        public static EditorTheme DefaultDark()
        {
            return new EditorTheme
            {
                BackgroundColor = new Color(0.13f, 0.13f, 0.13f),
                AccentColor = new Color(0.2f, 0.6f, 1f),
                SeparatorColor = new Color(0.3f, 0.3f, 0.3f),
                FooterBorderColor = new Color(0f, 0f, 0f, 0.15f),
                ToggleActiveColor = new Color(0.7f, 1f, 0.7f)
            };
        }

        public static EditorTheme DefaultLight()
        {
            return new EditorTheme
            {
                BackgroundColor = new Color(0.92f, 0.92f, 0.92f),
                AccentColor = new Color(0.24f, 0.5f, 0.9f),
                SeparatorColor = new Color(0.75f, 0.75f, 0.75f),
                FooterBorderColor = new Color(0f, 0f, 0f, 0.1f),
                ToggleActiveColor = new Color(0.55f, 0.9f, 0.55f)
            };
        }

        public static EditorTheme LoadFromPrefs(string prefix, EditorTheme fallback = null)
        {
            var baseTheme = (fallback ?? Default()).Clone();
            baseTheme.BackgroundColor   = LoadColor(prefix + "bg", baseTheme.BackgroundColor);
            baseTheme.AccentColor       = LoadColor(prefix + "accent", baseTheme.AccentColor);
            baseTheme.SeparatorColor    = LoadColor(prefix + "sep", baseTheme.SeparatorColor);
            baseTheme.FooterBorderColor = LoadColor(prefix + "footer", baseTheme.FooterBorderColor);
            baseTheme.ToggleActiveColor = LoadColor(prefix + "toggle", baseTheme.ToggleActiveColor);
            return baseTheme;
        }

        public void SaveToPrefs(string prefix)
        {
            SaveColor(prefix + "bg", BackgroundColor);
            SaveColor(prefix + "accent", AccentColor);
            SaveColor(prefix + "sep", SeparatorColor);
            SaveColor(prefix + "footer", FooterBorderColor);
            SaveColor(prefix + "toggle", ToggleActiveColor);
        }

        public static void DeleteFromPrefs(string prefix)
        {
            DeleteColor(prefix + "bg");
            DeleteColor(prefix + "accent");
            DeleteColor(prefix + "sep");
            DeleteColor(prefix + "footer");
            DeleteColor(prefix + "toggle");
        }

        private static Color LoadColor(string key, Color fallback)
        {
            Color c = fallback;
            if (EditorPrefs.HasKey(key + ".r"))
            {
                c.r = EditorPrefs.GetFloat(key + ".r", c.r);
                c.g = EditorPrefs.GetFloat(key + ".g", c.g);
                c.b = EditorPrefs.GetFloat(key + ".b", c.b);
                c.a = EditorPrefs.GetFloat(key + ".a", c.a);
            }
            return c;
        }

        private static void SaveColor(string key, Color c)
        {
            EditorPrefs.SetFloat(key + ".r", c.r);
            EditorPrefs.SetFloat(key + ".g", c.g);
            EditorPrefs.SetFloat(key + ".b", c.b);
            EditorPrefs.SetFloat(key + ".a", c.a);
        }

        private static void DeleteColor(string key)
        {
            EditorPrefs.DeleteKey(key + ".r");
            EditorPrefs.DeleteKey(key + ".g");
            EditorPrefs.DeleteKey(key + ".b");
            EditorPrefs.DeleteKey(key + ".a");
        }
    }
}
#endif

