using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Marks an array or List<T> field to be drawn as a reorderable list in the Inspector.
    /// Works with any Unity-serializable element type.
    /// </summary>
    /// <example>
    /// <code>
    /// [ReorderableList]
    /// public int[] levels;
    ///
    /// [ReorderableList("Inventory Items")]
    /// public List<string> items;
    /// </code>
    /// </example>
    public class ReorderableListAttribute : PropertyAttribute
    {
        /// <summary>
        /// Optional header label. If empty, uses the field label.
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// Enable drag handles to reorder elements (default true).
        /// </summary>
        public bool Draggable { get; }

        /// <summary>
        /// Show the add (+) button (default true).
        /// </summary>
        public bool ShowAdd { get; }

        /// <summary>
        /// Show the remove (-) button (default true).
        /// </summary>
        public bool ShowRemove { get; }

        /// <summary>
        /// Show element labels (index labels) next to each element (default true).
        /// </summary>
        public bool ShowElementLabels { get; }

        public ReorderableListAttribute()
        {
            Header = string.Empty;
            Draggable = true;
            ShowAdd = true;
            ShowRemove = true;
            ShowElementLabels = true;
        }

        public ReorderableListAttribute(string header, bool draggable = true, bool showAdd = true, bool showRemove = true, bool showElementLabels = true)
        {
            Header = header ?? string.Empty;
            Draggable = draggable;
            ShowAdd = showAdd;
            ShowRemove = showRemove;
            ShowElementLabels = showElementLabels;
        }
    }
}
