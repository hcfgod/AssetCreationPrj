using UnityEngine;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Examples
{
    /// <summary>
    /// Example script demonstrating various uses of SerializableDictionary.
    /// This shows common patterns and use cases for serializable dictionaries.
    /// </summary>
    public class SerializableDictionaryExample : MonoBehaviour
    {
        [Header("Game Configuration")]
        [Tooltip("Item IDs mapped to their display names")]
        public SerializableDictionary<string, string> itemNames = new SerializableDictionary<string, string>();

        [Tooltip("Item IDs mapped to their prices")]
        public SerializableDictionary<string, int> itemPrices = new SerializableDictionary<string, int>();

        [Tooltip("Item IDs mapped to their weights")]
        public StringFloatDictionary itemWeights = new StringFloatDictionary();

        [Header("Level Data")]
        [Tooltip("Level IDs mapped to their names")]
        public IntStringDictionary levelNames = new IntStringDictionary();
        
        [Tooltip("Level IDs mapped to their difficulty multipliers")]
        public IntFloatDictionary levelDifficulty = new IntFloatDictionary();

        [Header("Player Stats")]
        [Tooltip("Stat names mapped to their base values")]
        public StringIntDictionary baseStats = new StringIntDictionary();
        
        [Tooltip("Stat names mapped to their current values")]
        public StringIntDictionary currentStats = new StringIntDictionary();

        [Header("Visual Elements")]
        [Tooltip("UI element names mapped to their colors")]
        public StringColorDictionary uiColors = new StringColorDictionary();
        
        [Tooltip("Animation names mapped to their durations")]
        public StringFloatDictionary animationDurations = new StringFloatDictionary();

        [Header("Audio")]
        [Tooltip("Sound effect names mapped to their audio clips")]
        public StringAudioClipDictionary soundEffects = new StringAudioClipDictionary();

        [Header("Generic Usage (Runtime + Inspector via SerializeReference)")]
        [Tooltip("A generic serializable dictionary using TKey,TValue. Uses managed reference to show in Inspector.")]
        [SerializeReference]
        public SerializableDictionary<string, string> localizedTexts = new SerializableDictionary<string, string>();

        /// <summary>
        /// Called when the script starts.
        /// Initializes the dictionaries with default values.
        /// </summary>
        void Start()
        {
            InitializeDictionaries();
        }

        /// <summary>
        /// Initializes all dictionaries with example data.
        /// </summary>
        private void InitializeDictionaries()
        {
            // Initialize item data
            if (itemNames.Count == 0)
            {
                itemNames.Add("sword_001", "Iron Sword");
                itemNames.Add("potion_001", "Health Potion");
                itemNames.Add("armor_001", "Leather Armor");
            }

            if (itemPrices.Count == 0)
            {
                itemPrices.Add("sword_001", 150);
                itemPrices.Add("potion_001", 25);
                itemPrices.Add("armor_001", 200);
            }

            if (itemWeights.Count == 0)
            {
                itemWeights.Add("sword_001", 2.5f);
                itemWeights.Add("potion_001", 0.5f);
                itemWeights.Add("armor_001", 3.0f);
            }

            // Initialize level data
            if (levelNames.Count == 0)
            {
                levelNames.Add(1, "Tutorial");
                levelNames.Add(2, "Forest");
                levelNames.Add(3, "Cave");
            }

            if (levelDifficulty.Count == 0)
            {
                levelDifficulty.Add(1, 1.0f);
                levelDifficulty.Add(2, 1.5f);
                levelDifficulty.Add(3, 2.0f);
            }

            // Initialize player stats
            if (baseStats.Count == 0)
            {
                baseStats.Add("Strength", 10);
                baseStats.Add("Intelligence", 8);
                baseStats.Add("Dexterity", 12);
                baseStats.Add("Vitality", 15);
            }

            if (currentStats.Count == 0)
            {
                currentStats.Add("Strength", 10);
                currentStats.Add("Intelligence", 8);
                currentStats.Add("Dexterity", 12);
                currentStats.Add("Vitality", 15);
            }

            // Initialize UI colors
            if (uiColors.Count == 0)
            {
                uiColors.Add("Primary", Color.blue);
                uiColors.Add("Secondary", Color.gray);
                uiColors.Add("Success", Color.green);
                uiColors.Add("Warning", Color.yellow);
                uiColors.Add("Error", Color.red);
            }

            // Initialize animation durations
            if (animationDurations.Count == 0)
            {
                animationDurations.Add("Idle", 1.0f);
                animationDurations.Add("Walk", 0.5f);
                animationDurations.Add("Run", 0.3f);
                animationDurations.Add("Jump", 0.8f);
            }
        }

        /// <summary>
        /// Gets the display name for an item ID.
        /// </summary>
        /// <param name="itemId">The item ID to look up.</param>
        /// <returns>The display name, or "Unknown Item" if not found.</returns>
        public string GetItemName(string itemId)
        {
            return itemNames.ContainsKey(itemId) ? itemNames[itemId] : "Unknown Item";
        }

        /// <summary>
        /// Gets the price for an item ID.
        /// </summary>
        /// <param name="itemId">The item ID to look up.</param>
        /// <returns>The price, or 0 if not found.</returns>
        public int GetItemPrice(string itemId)
        {
            return itemPrices.ContainsKey(itemId) ? itemPrices[itemId] : 0;
        }

        /// <summary>
        /// Gets the weight for an item ID.
        /// </summary>
        /// <param name="itemId">The item ID to look up.</param>
        /// <returns>The weight, or 0 if not found.</returns>
        public float GetItemWeight(string itemId)
        {
            return itemWeights.ContainsKey(itemId) ? itemWeights[itemId] : 0f;
        }

        /// <summary>
        /// Gets the level name for a level ID.
        /// </summary>
        /// <param name="levelId">The level ID to look up.</param>
        /// <returns>The level name, or "Unknown Level" if not found.</returns>
        public string GetLevelName(int levelId)
        {
            return levelNames.ContainsKey(levelId) ? levelNames[levelId] : "Unknown Level";
        }

        /// <summary>
        /// Gets the difficulty multiplier for a level ID.
        /// </summary>
        /// <param name="levelId">The level ID to look up.</param>
        /// <returns>The difficulty multiplier, or 1.0 if not found.</returns>
        public float GetLevelDifficulty(int levelId)
        {
            return levelDifficulty.ContainsKey(levelId) ? levelDifficulty[levelId] : 1.0f;
        }

        /// <summary>
        /// Gets a stat value by name.
        /// </summary>
        /// <param name="statName">The stat name to look up.</param>
        /// <returns>The stat value, or 0 if not found.</returns>
        public int GetStat(string statName)
        {
            return currentStats.ContainsKey(statName) ? currentStats[statName] : 0;
        }

        /// <summary>
        /// Sets a stat value by name.
        /// </summary>
        /// <param name="statName">The stat name to set.</param>
        /// <param name="value">The new stat value.</param>
        public void SetStat(string statName, int value)
        {
            currentStats[statName] = value;
        }

        /// <summary>
        /// Gets a UI color by name.
        /// </summary>
        /// <param name="colorName">The color name to look up.</param>
        /// <returns>The color, or white if not found.</returns>
        public Color GetUIColor(string colorName)
        {
            return uiColors.ContainsKey(colorName) ? uiColors[colorName] : Color.white;
        }

        /// <summary>
        /// Gets an animation duration by name.
        /// </summary>
        /// <param name="animationName">The animation name to look up.</param>
        /// <returns>The duration, or 1.0 if not found.</returns>
        public float GetAnimationDuration(string animationName)
        {
            return animationDurations.ContainsKey(animationName) ? animationDurations[animationName] : 1.0f;
        }

        /// <summary>
        /// Example method showing how to iterate through dictionaries.
        /// </summary>
        [ContextMenu("Print All Items")]
        public void PrintAllItems()
        {
            Debug.Log("=== Item Names ===");
            foreach (var kvp in itemNames)
            {
                Debug.Log($"ID: {kvp.Key}, Name: {kvp.Value}");
            }

            Debug.Log("=== Item Prices ===");
            foreach (var kvp in itemPrices)
            {
                Debug.Log($"ID: {kvp.Key}, Price: {kvp.Value}");
            }
        }

        /// <summary>
        /// Example method showing how to modify dictionaries at runtime.
        /// </summary>
        [ContextMenu("Add Random Item")]
        public void AddRandomItem()
        {
            string randomId = "item_" + Random.Range(1000, 9999);
            string randomName = "Random Item " + Random.Range(1, 100);
            int randomPrice = Random.Range(10, 500);
            float randomWeight = Random.Range(0.1f, 5.0f);

            itemNames.Add(randomId, randomName);
            itemPrices.Add(randomId, randomPrice);
            itemWeights.Add(randomId, randomWeight);

            Debug.Log($"Added new item: {randomName} (ID: {randomId}, Price: {randomPrice}, Weight: {randomWeight})");
        }
    }
}
