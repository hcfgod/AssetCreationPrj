using System;
using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Attribute that creates a button in the inspector for a method.
    /// The method must be public and have no parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        /// <summary>
        /// The text displayed on the button. If null, uses the method name.
        /// </summary>
        public string ButtonText { get; }

        /// <summary>
        /// The height of the button in pixels.
        /// </summary>
        public float ButtonHeight { get; }

        /// <summary>
        /// The width of the button in pixels. If 0, uses full width.
        /// </summary>
        public float ButtonWidth { get; }

        /// <summary>
        /// The order in which this button appears in the inspector.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Whether the button should be enabled in play mode only.
        /// </summary>
        public bool PlayModeOnly { get; }

        /// <summary>
        /// Whether the button should be enabled in edit mode only.
        /// </summary>
        public bool EditModeOnly { get; }

        /// <summary>
        /// The color of the button text.
        /// </summary>
        public Color TextColor { get; }

        /// <summary>
        /// The color of the button background.
        /// </summary>
        public Color BackgroundColor { get; }

        /// <summary>
        /// Whether to show a confirmation dialog before executing the method.
        /// </summary>
        public bool ShowConfirmation { get; }

        /// <summary>
        /// The confirmation message to display.
        /// </summary>
        public string ConfirmationMessage { get; }

        /// <summary>
        /// Whether custom colors have been set.
        /// </summary>
        public bool HasCustomColors { get; }

        /// <summary>
        /// Creates a new Button attribute with default settings.
        /// </summary>
        public ButtonAttribute()
        {
            ButtonHeight = 20f;
            ButtonWidth = 0f; // Full width
            Order = 0;
            PlayModeOnly = false;
            EditModeOnly = false;
            TextColor = Color.white;
            BackgroundColor = Color.clear; // Use default button color
            ShowConfirmation = false;
            ConfirmationMessage = "Are you sure?";
            HasCustomColors = false;
        }

        /// <summary>
        /// Creates a new Button attribute with custom text.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        public ButtonAttribute(string buttonText) : this()
        {
            ButtonText = buttonText;
        }

        /// <summary>
        /// Creates a new Button attribute with custom text and height.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="buttonHeight">The height of the button in pixels</param>
        public ButtonAttribute(string buttonText, float buttonHeight) : this(buttonText)
        {
            ButtonHeight = buttonHeight;
        }

        /// <summary>
        /// Creates a new Button attribute with custom text, height, and width.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="buttonHeight">The height of the button in pixels</param>
        /// <param name="buttonWidth">The width of the button in pixels (0 for full width)</param>
        public ButtonAttribute(string buttonText, float buttonHeight, float buttonWidth) : this(buttonText, buttonHeight)
        {
            ButtonWidth = buttonWidth;
        }

        /// <summary>
        /// Creates a new Button attribute with custom text, height, width, and order.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="buttonHeight">The height of the button in pixels</param>
        /// <param name="buttonWidth">The width of the button in pixels (0 for full width)</param>
        /// <param name="order">The order in which this button appears</param>
        public ButtonAttribute(string buttonText, float buttonHeight, float buttonWidth, int order) : this(buttonText, buttonHeight, buttonWidth)
        {
            Order = order;
        }

        /// <summary>
        /// Creates a new Button attribute with play mode restriction.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="playModeOnly">Whether the button should only work in play mode</param>
        public ButtonAttribute(string buttonText, bool playModeOnly) : this(buttonText)
        {
            PlayModeOnly = playModeOnly;
            EditModeOnly = false;
        }

        /// <summary>
        /// Creates a new Button attribute with edit mode restriction.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="playModeOnly">Whether the button should only work in play mode</param>
        /// <param name="editModeOnly">Whether the button should only work in edit mode</param>
        public ButtonAttribute(string buttonText, bool playModeOnly, bool editModeOnly) : this(buttonText, playModeOnly)
        {
            EditModeOnly = editModeOnly;
            if (editModeOnly) PlayModeOnly = false;
        }

        /// <summary>
        /// Creates a new Button attribute with confirmation dialog.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="showConfirmation">Whether to show confirmation dialog</param>
        /// <param name="confirmationMessage">The confirmation message</param>
        public ButtonAttribute(string buttonText, bool showConfirmation, string confirmationMessage) : this(buttonText)
        {
            ShowConfirmation = showConfirmation;
            ConfirmationMessage = confirmationMessage;
        }

        /// <summary>
        /// Creates a new Button attribute with custom colors.
        /// </summary>
        /// <param name="buttonText">The text to display on the button</param>
        /// <param name="textColorHex">The hex color of the button text (e.g., "#FF0000" for red)</param>
        /// <param name="backgroundColorHex">The hex color of the button background (e.g., "#00FF00" for green)</param>
        public ButtonAttribute(string buttonText, string textColorHex, string backgroundColorHex) : this(buttonText)
        {
            if (ColorUtility.TryParseHtmlString(textColorHex, out Color textColor))
            {
                TextColor = textColor;
            }
            if (ColorUtility.TryParseHtmlString(backgroundColorHex, out Color backgroundColor))
            {
                BackgroundColor = backgroundColor;
            }
            HasCustomColors = true;
        }
    }
}