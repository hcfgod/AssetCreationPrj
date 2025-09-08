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

        // Extended theming
        public Color TextColor = new Color(0.85f, 0.85f, 0.85f);
        public Color SubTextColor = new Color(0.7f, 0.7f, 0.7f);
        public Color HeaderTextColor = new Color(0.95f, 0.95f, 0.95f);

        public Color PanelBackgroundColor = new Color(0.18f, 0.18f, 0.18f);
        public Color PanelBorderColor = new Color(0f, 0f, 0f, 0.35f);

        public Color ButtonNormalColor = new Color(0.22f, 0.22f, 0.22f);
        public Color ButtonTextColor = new Color(0.92f, 0.92f, 0.92f);
        public Color ButtonBorderColor = new Color(0f, 0f, 0f, 0.45f);

        public Color ProgressBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
        public Color ProgressFillColor = new Color(0.2f, 0.6f, 1f);
        public Color ProgressTextColor = new Color(0.95f, 0.95f, 0.95f);

        public EditorTheme Clone()
        {
            return new EditorTheme
            {
                BackgroundColor = this.BackgroundColor,
                AccentColor = this.AccentColor,
                SeparatorColor = this.SeparatorColor,
                FooterBorderColor = this.FooterBorderColor,
                ToggleActiveColor = this.ToggleActiveColor,
                TextColor = this.TextColor,
                SubTextColor = this.SubTextColor,
                HeaderTextColor = this.HeaderTextColor,
                PanelBackgroundColor = this.PanelBackgroundColor,
                PanelBorderColor = this.PanelBorderColor,
                ButtonNormalColor = this.ButtonNormalColor,
                ButtonTextColor = this.ButtonTextColor,
                ButtonBorderColor = this.ButtonBorderColor,
                ProgressBackgroundColor = this.ProgressBackgroundColor,
                ProgressFillColor = this.ProgressFillColor,
                ProgressTextColor = this.ProgressTextColor
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
                ToggleActiveColor = new Color(0.7f, 1f, 0.7f),
                TextColor = new Color(0.85f, 0.85f, 0.85f),
                SubTextColor = new Color(0.7f, 0.7f, 0.7f),
                HeaderTextColor = new Color(0.95f, 0.95f, 0.95f),
                PanelBackgroundColor = new Color(0.18f, 0.18f, 0.18f),
                PanelBorderColor = new Color(0f, 0f, 0f, 0.35f),
                ButtonNormalColor = new Color(0.22f, 0.22f, 0.22f),
                ButtonTextColor = new Color(0.92f, 0.92f, 0.92f),
                ButtonBorderColor = new Color(0f, 0f, 0f, 0.45f),
                ProgressBackgroundColor = new Color(0.2f, 0.2f, 0.2f),
                ProgressFillColor = new Color(0.2f, 0.6f, 1f),
                ProgressTextColor = new Color(0.95f, 0.95f, 0.95f)
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
                ToggleActiveColor = new Color(0.55f, 0.9f, 0.55f),
                TextColor = new Color(0.1f, 0.1f, 0.1f),
                SubTextColor = new Color(0.25f, 0.25f, 0.25f),
                HeaderTextColor = new Color(0.05f, 0.05f, 0.05f),
                PanelBackgroundColor = new Color(0.95f, 0.95f, 0.95f),
                PanelBorderColor = new Color(0f, 0f, 0f, 0.2f),
                ButtonNormalColor = new Color(0.92f, 0.92f, 0.92f),
                ButtonTextColor = new Color(0.12f, 0.12f, 0.12f),
                ButtonBorderColor = new Color(0f, 0f, 0f, 0.25f),
                ProgressBackgroundColor = new Color(0.85f, 0.85f, 0.85f),
                ProgressFillColor = new Color(0.24f, 0.5f, 0.9f),
                ProgressTextColor = new Color(0.12f, 0.12f, 0.12f)
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
            baseTheme.TextColor         = LoadColor(prefix + "text", baseTheme.TextColor);
            baseTheme.SubTextColor      = LoadColor(prefix + "subtext", baseTheme.SubTextColor);
            baseTheme.HeaderTextColor   = LoadColor(prefix + "headerText", baseTheme.HeaderTextColor);
            baseTheme.PanelBackgroundColor = LoadColor(prefix + "panelBg", baseTheme.PanelBackgroundColor);
            baseTheme.PanelBorderColor  = LoadColor(prefix + "panelBorder", baseTheme.PanelBorderColor);
            baseTheme.ButtonNormalColor = LoadColor(prefix + "btnBg", baseTheme.ButtonNormalColor);
            baseTheme.ButtonTextColor   = LoadColor(prefix + "btnText", baseTheme.ButtonTextColor);
            baseTheme.ButtonBorderColor = LoadColor(prefix + "btnBorder", baseTheme.ButtonBorderColor);
            baseTheme.ProgressBackgroundColor = LoadColor(prefix + "progBg", baseTheme.ProgressBackgroundColor);
            baseTheme.ProgressFillColor = LoadColor(prefix + "progFill", baseTheme.ProgressFillColor);
            baseTheme.ProgressTextColor = LoadColor(prefix + "progText", baseTheme.ProgressTextColor);
            return baseTheme;
        }

        public void SaveToPrefs(string prefix)
        {
            SaveColor(prefix + "bg", BackgroundColor);
            SaveColor(prefix + "accent", AccentColor);
            SaveColor(prefix + "sep", SeparatorColor);
            SaveColor(prefix + "footer", FooterBorderColor);
            SaveColor(prefix + "toggle", ToggleActiveColor);
            SaveColor(prefix + "text", TextColor);
            SaveColor(prefix + "subtext", SubTextColor);
            SaveColor(prefix + "headerText", HeaderTextColor);
            SaveColor(prefix + "panelBg", PanelBackgroundColor);
            SaveColor(prefix + "panelBorder", PanelBorderColor);
            SaveColor(prefix + "btnBg", ButtonNormalColor);
            SaveColor(prefix + "btnText", ButtonTextColor);
            SaveColor(prefix + "btnBorder", ButtonBorderColor);
            SaveColor(prefix + "progBg", ProgressBackgroundColor);
            SaveColor(prefix + "progFill", ProgressFillColor);
            SaveColor(prefix + "progText", ProgressTextColor);
        }

        public static void DeleteFromPrefs(string prefix)
        {
            DeleteColor(prefix + "bg");
            DeleteColor(prefix + "accent");
            DeleteColor(prefix + "sep");
            DeleteColor(prefix + "footer");
            DeleteColor(prefix + "toggle");
            DeleteColor(prefix + "text");
            DeleteColor(prefix + "subtext");
            DeleteColor(prefix + "headerText");
            DeleteColor(prefix + "panelBg");
            DeleteColor(prefix + "panelBorder");
            DeleteColor(prefix + "btnBg");
            DeleteColor(prefix + "btnText");
            DeleteColor(prefix + "btnBorder");
            DeleteColor(prefix + "progBg");
            DeleteColor(prefix + "progFill");
            DeleteColor(prefix + "progText");
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

