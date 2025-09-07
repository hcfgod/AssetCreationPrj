using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Draws a non-editable progress bar for numeric fields (int or float).
    /// Configure min/max range, color, height, and optional label/value display.
    /// </summary>
    public class ProgressBarAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public string Label { get; }
        public Color BarColor { get; }
        public float Height { get; }
        public bool ShowValue { get; }

        /// <summary>
        /// Progress bar with hex color specification.
        /// </summary>
        /// <param name="min">Minimum expected value.</param>
        /// <param name="max">Maximum expected value.</param>
        /// <param name="label">Optional label override; null uses property display name.</param>
        /// <param name="hexColor">Bar color as hex string (e.g., "#4ECDC4").</param>
        /// <param name="height">Bar height in pixels (default 18).</param>
        /// <param name="showValue">Whether to show percentage text.</param>
        public ProgressBarAttribute(float min = 0f, float max = 1f, string label = null, string hexColor = "#4ECDC4", float height = 18f, bool showValue = true)
        {
            Min = min;
            Max = max;
            Label = label;
            BarColor = ParseHexColor(hexColor);
            Height = Mathf.Max(12f, height);
            ShowValue = showValue;
        }

        /// <summary>
        /// Progress bar with UnityEngine.Color specification.
        /// </summary>
        public ProgressBarAttribute(float min, float max, string label, Color color, float height = 18f, bool showValue = true)
        {
            Min = min;
            Max = max;
            Label = label;
            BarColor = color;
            Height = Mathf.Max(12f, height);
            ShowValue = showValue;
        }

        private static Color ParseHexColor(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return new Color(0.306f, 0.804f, 0.769f); // default teal-ish

            if (hexColor.StartsWith("#")) hexColor = hexColor.Substring(1);
            if (hexColor.Length != 6) return new Color(0.306f, 0.804f, 0.769f);
            try
            {
                int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color(r / 255f, g / 255f, b / 255f, 1f);
            }
            catch
            {
                return new Color(0.306f, 0.804f, 0.769f);
            }
        }
    }
}

