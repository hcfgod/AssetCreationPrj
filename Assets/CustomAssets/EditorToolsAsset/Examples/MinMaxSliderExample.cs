using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class MinMaxSliderExample : MonoBehaviour
    {
        [Header("Float Range [0..1]")]
        [MinMaxSlider(0f, 1f)]
        public Vector2 spawnTimeRange = new Vector2(0.2f, 0.8f);

        [Header("Int Range [0..100]")]
        [MinMaxSlider(0, 100)]
        public Vector2Int levelRange = new Vector2Int(10, 40);

        [Header("Float Range [10..50] with precise fields hidden")]
        [MinMaxSlider(10f, 50f, showFields: false)]
        public Vector2 speedLimits = new Vector2(12f, 30f);
    }
}
