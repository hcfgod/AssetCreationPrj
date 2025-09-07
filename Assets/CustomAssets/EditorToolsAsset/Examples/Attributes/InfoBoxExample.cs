using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class InfoBoxExample : MonoBehaviour
    {
        [InfoBox("This is an info message.", InfoBoxType.Info)]
        public int someValue = 42;

        [TextArea(3,6)]
        public string longDescription = "This is a long message that may span multiple lines when shown in an info box.";

        [InfoBox("This info box is set to a fixed height of 60 pixels to accommodate multi-line text.", InfoBoxType.Info, 60f)]
        public int fixedHeightExample = 7;

        [InfoBox("Warning: This value is experimental.", InfoBoxType.Warning)]
        public float experimentalValue = 1.0f;

        public bool showError;
        [InfoBox("Error shown when 'showError' is true.", nameof(showError), InfoBoxType.Error)]
        public string guardedField = "";

        // Method-based condition
        [InfoBox("Shown when IsAdvancedMode() returns true.", nameof(IsAdvancedMode), InfoBoxType.Info)]
        public int advancedSetting = 5;

        private bool IsAdvancedMode() => true;
    }
}
