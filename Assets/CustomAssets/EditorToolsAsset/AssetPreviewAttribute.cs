using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Renders a preview thumbnail for object reference fields (Sprite, Texture, Material, Prefab, etc.).
    /// Optionally draws the object field above the preview.
    /// Supports string fields (paths) by resolving assets via AssetDatabase in the editor drawer.
    /// </summary>
    public class AssetPreviewAttribute : PropertyAttribute
    {
        public float Width { get; }
        public float Height { get; }
        public bool ShowObjectField { get; }
        public bool AllowSceneObjects { get; }
        public Color BackgroundColor { get; }
        public Color TintColor { get; }
        public bool KeepAspect { get; }

        /// <summary>
        /// Configure preview size, object field visibility, background/tint, and aspect behavior.
        /// </summary>
        public AssetPreviewAttribute(
            float width = 64f,
            float height = 64f,
            bool showObjectField = true,
            bool allowSceneObjects = false,
            string backgroundHex = "#292929",
            string tintHex = "#FFFFFF",
            bool keepAspect = true)
        {
            Width = Mathf.Max(16f, width);
            Height = Mathf.Max(16f, height);
            ShowObjectField = showObjectField;
            AllowSceneObjects = allowSceneObjects;
            BackgroundColor = ParseHexColor(backgroundHex, new Color(0.16f, 0.16f, 0.16f, 0.5f));
            TintColor = ParseHexColor(tintHex, Color.white);
            KeepAspect = keepAspect;
        }

        private static Color ParseHexColor(string hexColor, Color fallback)
        {
            if (string.IsNullOrEmpty(hexColor))
                return fallback;
            if (hexColor.StartsWith("#")) hexColor = hexColor.Substring(1);
            if (hexColor.Length != 6)
                return fallback;
            try
            {
                int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color(r / 255f, g / 255f, b / 255f, fallback.a);
            }
            catch
            {
                return fallback;
            }
        }
    }
}

