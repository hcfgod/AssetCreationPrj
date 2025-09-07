using UnityEngine;
using System.Collections.Generic;

namespace CustomAssets.EditorTools.Examples
{
    public class ValidateConvenienceExample : MonoBehaviour
    {
        [Title("Numeric Range and Bounds")]
        [MinValue(0, "Speed must be ≥ 0", ValidateSeverity.Warning)]
        [MaxValue(20, "Speed must be ≤ 20", ValidateSeverity.Warning)]
        public float speed = 5f;

        [RangeValue(1, 100, "Health must be in [1, 100]")]
        public int health = 50;

        [NonZero("Damage cannot be zero")] public int damage = 10;

        [Title("Strings and Patterns")]
        [NotEmpty("Display name cannot be empty")] public string displayName = "Player";
        [RegexMatch("^[a-zA-Z0-9_]{3,16}$", "Username must be 3-16 alphanumeric characters or underscore")]
        public string userName = "Player_01";

        [Title("References and Collections")]
        [NotNull("Prefab must be assigned")] public GameObject prefab;

        [ReorderableList("Tags")]
        [NotEmpty("At least one tag required", ValidateSeverity.Info)]
        public List<string> tags = new List<string> { "tag1" };
    }
}