using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Attribute that creates a colored title/header in the Unity Inspector.
    /// Provides more visual appeal than the standard Header attribute with color support.
    /// </summary>
    /// <example>
    /// <code>
    /// [Title("Basic Settings")]
    /// public float speed = 5f;
    /// 
    /// [Title("Advanced Settings", "#FF6B6B")]
    /// public bool enableAdvancedMode = false;
    /// 
    /// [Title("Player Stats", Color.blue)]
    /// public int health = 100;
    /// 
    /// [Title("Weapon Configuration", "#4ECDC4", 16)]
    /// public string weaponName = "Sword";
    /// </code>
    /// </example>
    public class TitleAttribute : HeaderAttribute
    {
        /// <summary>
        /// The title text to display.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The color of the title text.
        /// </summary>
        public Color TitleColor { get; }

        /// <summary>
        /// The font size of the title text.
        /// </summary>
        public int FontSize { get; }

        /// <summary>
        /// Whether to add a separator line below the title.
        /// </summary>
        public bool ShowSeparator { get; }

        /// <summary>
        /// The color of the separator line.
        /// </summary>
        public Color SeparatorColor { get; }

        /// <summary>
        /// Initializes a new instance of the TitleAttribute with default styling.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        public TitleAttribute(string title) : base(title)
        {
            Title = title;
            TitleColor = Color.white;
            FontSize = 12;
            ShowSeparator = true;
            SeparatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Initializes a new instance of the TitleAttribute with custom color.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="color">The color of the title text.</param>
        public TitleAttribute(string title, Color color) : base(title)
        {
            Title = title;
            TitleColor = color;
            FontSize = 12;
            ShowSeparator = true;
            SeparatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Initializes a new instance of the TitleAttribute with hex color string.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="hexColor">The hex color string (e.g., "#FF6B6B").</param>
        public TitleAttribute(string title, string hexColor) : base(title)
        {
            Title = title;
            TitleColor = ParseHexColor(hexColor);
            FontSize = 12;
            ShowSeparator = true;
            SeparatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Initializes a new instance of the TitleAttribute with custom color and font size.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="color">The color of the title text.</param>
        /// <param name="fontSize">The font size of the title text.</param>
        public TitleAttribute(string title, Color color, int fontSize) : base(title)
        {
            Title = title;
            TitleColor = color;
            FontSize = fontSize;
            ShowSeparator = true;
            SeparatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Initializes a new instance of the TitleAttribute with hex color and font size.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="hexColor">The hex color string (e.g., "#FF6B6B").</param>
        /// <param name="fontSize">The font size of the title text.</param>
        public TitleAttribute(string title, string hexColor, int fontSize) : base(title)
        {
            Title = title;
            TitleColor = ParseHexColor(hexColor);
            FontSize = fontSize;
            ShowSeparator = true;
            SeparatorColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Initializes a new instance of the TitleAttribute with full customization.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        /// <param name="color">The color of the title text.</param>
        /// <param name="fontSize">The font size of the title text.</param>
        /// <param name="showSeparator">Whether to show a separator line below the title.</param>
        /// <param name="separatorColor">The color of the separator line.</param>
        public TitleAttribute(string title, Color color, int fontSize, bool showSeparator, Color separatorColor) : base(title)
        {
            Title = title;
            TitleColor = color;
            FontSize = fontSize;
            ShowSeparator = showSeparator;
            SeparatorColor = separatorColor;
        }

        /// <summary>
        /// Parses a hex color string into a Unity Color.
        /// </summary>
        /// <param name="hexColor">The hex color string (e.g., "#FF6B6B" or "FF6B6B").</param>
        /// <returns>The parsed Color, or white if parsing fails.</returns>
        private Color ParseHexColor(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return Color.white;

            // Remove # if present
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            // Ensure we have 6 characters
            if (hexColor.Length != 6)
                return Color.white;

            try
            {
                // Parse RGB values
                int r = int.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                return new Color(r / 255f, g / 255f, b / 255f, 1f);
            }
            catch
            {
                return Color.white;
            }
        }
    }
}
