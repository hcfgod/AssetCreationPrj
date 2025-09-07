using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class ProgressBarExample : MonoBehaviour
    {
        [Title("Health", "#FF6B6B")]
        [RangeValue(0, 100)]
        [ProgressBar(0, 100, label: "Health", hexColor: "#FF6B6B", height: 18f, showValue: true, editable: true)]
        public int health = 75;

        [Title("Charge", "#4ECDC4")]
        [RangeValue(0, 1)]
        [ProgressBar(0f, 1f, label: "Charge", hexColor: "#4ECDC4", height: 18f)]
        public float charge = 0.64f;

        [Title("Custom Color & Height", "#45B7D1")]
        [ProgressBar(0f, 1f, label: "Loading", hexColor: "#45B7D1", height: 24f, showValue: true)]
        public float loading = 0.3f;
    }
}