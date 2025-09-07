using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class FoldoutGroupExample : MonoBehaviour
    {
        [FoldoutGroup("Character", order: 0, expandedByDefault: true)] public string nameTag = "Hero";
        [FoldoutGroup("Character", order: 0)] public int level = 1;
        [FoldoutGroup("Character", order: 0)] public float health = 100f;

        [FoldoutGroup("Combat", order: 1)] public float damage = 10f;
        [FoldoutGroup("Combat", order: 1)] public float attackSpeed = 1.0f;
        [FoldoutGroup("Combat", order: 1)] public bool canCrit = true;

        [FoldoutGroup("Movement", order: 2, expandedByDefault: false)] public float speed = 5f;
        [FoldoutGroup("Movement", order: 2)] public float jumpForce = 5f;
    }
}
