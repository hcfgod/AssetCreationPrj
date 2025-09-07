using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class ValidateInputExample : MonoBehaviour
    {
        [Title("Numeric Validation (>= 0)")]
        [ValidateInput(nameof(ValidateNonNegative), "Value must be >= 0", ValidateSeverity.Warning)]
        public float speed = 5f;

        [Title("String Not Empty")]
        [ValidateInput(nameof(ValidateNotEmpty), "Name must not be empty", ValidateSeverity.Error)]
        public string displayName = "Player";

        [Title("List Count (1..5)")]
        [ReorderableList("Tags")]
        [ValidateInput(nameof(ValidateTagsCount), "Provide between 1 and 5 tags", ValidateSeverity.Info)]
        public List<string> tags = new List<string> { "a" };

        private bool ValidateNonNegative(float v) => v >= 0f;
        private bool ValidateNotEmpty(string s) => !string.IsNullOrWhiteSpace(s);
        private string ValidateTagsCount(List<string> list)
        {
            if (list == null) return "Tags list is null";
            return (list.Count >= 1 && list.Count <= 5) ? null : "Provide between 1 and 5 tags";
        }
    }
}
