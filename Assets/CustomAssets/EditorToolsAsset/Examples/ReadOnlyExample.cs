using UnityEngine;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Examples
{
    /// <summary>
    /// Example script demonstrating the usage of the ReadOnly attribute.
    /// This script shows various ways to use ReadOnly fields in the Unity Inspector.
    /// </summary>
    public class ReadOnlyExample : MonoBehaviour
    {
        [Header("ReadOnly Examples")]
        
        [ReadOnly("This field cannot be edited in the inspector")]
        public float health = 100f;
        
        [ReadOnly]
        public string playerName = "Player";
        
        [ReadOnly("This value is calculated automatically")]
        public int score = 0;
        
        [ReadOnly]
        public bool isAlive = true;
        
        [ReadOnly("This vector represents the player's spawn position")]
        public Vector3 spawnPosition = Vector3.zero;
        
        [Header("Editable Fields (for comparison)")]
        public float maxHealth = 100f;
        public string displayName = "Player Name";
        
        [Header("Mixed Fields")]
        [ReadOnly("This damage value is calculated")]
        public float damage = 25f;
        
        public float damageMultiplier = 1.5f;
        
        [ReadOnly("Final damage after multiplier")]
        public float finalDamage = 37.5f;
        
        /// <summary>
        /// Called when the script starts.
        /// Demonstrates how to update ReadOnly fields programmatically.
        /// </summary>
        void Start()
        {
            // You can still modify ReadOnly fields in code
            health = maxHealth;
            score = 0;
            isAlive = true;
            spawnPosition = transform.position;
            
            // Calculate final damage
            CalculateFinalDamage();
        }
        
        /// <summary>
        /// Updates the final damage calculation.
        /// This demonstrates how ReadOnly fields can be updated programmatically.
        /// </summary>
        void CalculateFinalDamage()
        {
            finalDamage = damage * damageMultiplier;
        }
        
        /// <summary>
        /// Called every frame.
        /// Demonstrates dynamic updates to ReadOnly fields.
        /// </summary>
        void Update()
        {
            // Update ReadOnly fields based on game logic
            isAlive = health > 0;
            
            // Recalculate final damage if multiplier changes
            CalculateFinalDamage();
        }
        
        /// <summary>
        /// Method to simulate taking damage.
        /// Shows how ReadOnly fields can be updated through gameplay.
        /// </summary>
        /// <param name="damageAmount">Amount of damage to take.</param>
        public void TakeDamage(float damageAmount)
        {
            health = Mathf.Max(0, health - damageAmount);
            isAlive = health > 0;
        }
        
        /// <summary>
        /// Method to add score points.
        /// Demonstrates updating ReadOnly fields through game events.
        /// </summary>
        /// <param name="points">Points to add to the score.</param>
        public void AddScore(int points)
        {
            score += points;
        }
    }
}
