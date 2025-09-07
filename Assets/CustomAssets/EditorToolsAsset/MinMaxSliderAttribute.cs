using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Attribute that renders a min-max slider in the Inspector for Vector2 (float) or Vector2Int (int) ranges.
    /// - For Vector2 fields, X = Min and Y = Max (float values).
    /// - For Vector2Int fields, X = Min and Y = Max (integer values).
    /// </summary>
    /// <example>
    /// <code>
    /// // Float range [0..1]
    /// [MinMaxSlider(0f, 1f)]
    /// public Vector2 spawnTimeRange = new Vector2(0.2f, 0.8f);
    ///
    /// // Integer range [0..100]
    /// [MinMaxSlider(0, 100)]
    /// public Vector2Int levelRange = new Vector2Int(10, 40);
    /// </code>
    /// </example>
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        /// <summary>
        /// Whether the backing type is intended to be integer (Vector2Int) or float (Vector2).
        /// </summary>
        public bool IsInt { get; }

        /// <summary>
        /// Minimum slider limit for float mode.
        /// </summary>
        public float MinLimitFloat { get; }

        /// <summary>
        /// Maximum slider limit for float mode.
        /// </summary>
        public float MaxLimitFloat { get; }

        /// <summary>
        /// Minimum slider limit for int mode.
        /// </summary>
        public int MinLimitInt { get; }

        /// <summary>
        /// Maximum slider limit for int mode.
        /// </summary>
        public int MaxLimitInt { get; }

        /// <summary>
        /// If true, renders numeric fields next to the slider for precise entry.
        /// </summary>
        public bool ShowFields { get; }

        /// <summary>
        /// Number of decimal places to show for float fields.
        /// </summary>
        public int Decimals { get; }

        /// <summary>
        /// Creates a min-max slider for float ranges (Vector2 fields).
        /// </summary>
        /// <param name="minLimit">Minimum allowed value for the slider.</param>
        /// <param name="maxLimit">Maximum allowed value for the slider.</param>
        /// <param name="showFields">Show numeric input fields alongside the slider.</param>
        /// <param name="decimals">Decimal places to display in numeric fields.</param>
        public MinMaxSliderAttribute(float minLimit, float maxLimit, bool showFields = true, int decimals = 2)
        {
            IsInt = false;
            MinLimitFloat = minLimit;
            MaxLimitFloat = maxLimit;
            ShowFields = showFields;
            Decimals = Mathf.Clamp(decimals, 0, 6);
        }

        /// <summary>
        /// Creates a min-max slider for integer ranges (Vector2Int fields).
        /// </summary>
        /// <param name="minLimit">Minimum allowed integer value for the slider.</param>
        /// <param name="maxLimit">Maximum allowed integer value for the slider.</param>
        /// <param name="showFields">Show numeric input fields alongside the slider.</param>
        public MinMaxSliderAttribute(int minLimit, int maxLimit, bool showFields = true)
        {
            IsInt = true;
            MinLimitInt = minLimit;
            MaxLimitInt = maxLimit;
            ShowFields = showFields;
            Decimals = 0;
        }
    }
}
