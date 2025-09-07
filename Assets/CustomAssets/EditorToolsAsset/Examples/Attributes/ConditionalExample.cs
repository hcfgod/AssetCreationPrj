using UnityEngine;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Examples
{
    /// <summary>
    /// Example script demonstrating the ShowIf and HideIf attributes.
    /// This script shows various use cases for conditional field visibility.
    /// </summary>
    public class ConditionalExample : MonoBehaviour
    {
        [Title("Basic Boolean Conditions", "#FF6B6B")]
        [Tooltip("Toggle this to show/hide conditional fields")]
        public bool showAdvancedSettings = false;

        [ShowIf("showAdvancedSettings")]
        [Tooltip("This field only appears when showAdvancedSettings is true")]
        public float advancedValue = 1.0f;

        [HideIf("showAdvancedSettings")]
        [Tooltip("This field is hidden when showAdvancedSettings is true")]
        public string basicMessage = "Basic settings are visible";

        [Title("Numeric Value Conditions", "#4ECDC4")]
        [Range(0, 100)]
        public float health = 100f;

        [ShowIf("health", 100f)]
        [Tooltip("Only shows when health is exactly 100")]
        public string maxHealthMessage = "At full health!";

        [ShowIf("health", 0f)]
        [Tooltip("Only shows when health is exactly 0")]
        public string deathMessage = "Player is dead!";

        [HideIf("health", 0f)]
        [Tooltip("Hidden when health is 0")]
        public string aliveMessage = "Player is alive!";

        [Title("Enum Conditions", "#45B7D1")]
        public PlayerType playerType = PlayerType.Player;

        [ShowIf("playerType", PlayerType.Admin)]
        [Tooltip("Only visible for admin players")]
        public bool adminOnlySetting = true;

        [HideIf("playerType", PlayerType.Guest)]
        [Tooltip("Hidden for guest players")]
        public bool loggedInOnlySetting = true;

        [Title("String Conditions", "#96CEB4")]
        public string playerName = "";

        [ShowIf("playerName")]
        [Tooltip("Only shows when playerName is not empty")]
        public string welcomeMessage = "Welcome, ";

        [ShowIf("playerName")]
        [ReadOnly("Dynamic welcome message with player name")]
        [Tooltip("This field shows the complete welcome message")]
        public string dynamicWelcomeMessage = "";

        [HideIf("playerName")]
        [Tooltip("Hidden when playerName is not empty")]
        public string enterNameMessage = "Please enter your name";

        [Title("Button to Update Welcome Message", "#FFEAA7")]
        [Tooltip("This section contains the button to update the welcome message")]
        [ReadOnly("This field is just for the title - use the button below")]
        public string buttonSectionTitle = "Welcome Message Controls";

        [Button("Update Welcome Message")]
        [Tooltip("Click this button to update the welcome message with the current player name")]
        public void UpdateWelcomeMessage()
        {
            if (!string.IsNullOrEmpty(playerName))
            {
                dynamicWelcomeMessage = $"Welcome, {playerName}!";
                Debug.Log($"Welcome message updated: {dynamicWelcomeMessage}");
            }
            else
            {
                dynamicWelcomeMessage = "";
                Debug.Log("Player name is empty, welcome message cleared");
            }
        }

        [Title("Complex Conditions", "#DDA0DD")]
        public bool enableComplexMode = false;
        public int complexityLevel = 1;

        [ShowIf("enableComplexMode")]
        [Tooltip("Shows when complex mode is enabled")]
        public string complexModeMessage = "Complex mode is active";

        [ShowIf("complexityLevel", 3)]
        [Tooltip("Only shows when complexity level is 3")]
        public string maxComplexityMessage = "Maximum complexity reached!";

        [HideIf("enableComplexMode")]
        [Tooltip("Hidden when complex mode is enabled")]
        public string simpleModeMessage = "Simple mode is active";

        /// <summary>
        /// Called when values are changed in the Inspector.
        /// This automatically updates the welcome message when the player name changes.
        /// </summary>
        private void OnValidate()
        {
            // Automatically update the welcome message when player name changes
            if (!string.IsNullOrEmpty(playerName))
            {
                dynamicWelcomeMessage = $"Welcome, {playerName}!";
            }
            else
            {
                dynamicWelcomeMessage = "";
            }
        }
    }

    /// <summary>
    /// Example enum for demonstrating enum-based conditions.
    /// </summary>
    public enum PlayerType
    {
        Guest,
        Player,
        Moderator,
        Admin
    }
}
