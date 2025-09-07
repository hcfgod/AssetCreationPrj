using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Groups multiple fields into a shared foldout header in the Inspector, similar to Odin's FoldoutGroup.
    /// All fields with the same group name will share the same foldout state per-object instance.
    /// </summary>
    public class FoldoutGroupAttribute : PropertyAttribute
    {
        /// <summary>
        /// Logical group name that ties several fields together.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Optional ordering hint used to choose which field draws the foldout header.
        /// Lower values have higher priority. Defaults to 0.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Whether the group should be expanded by default.
        /// </summary>
        public bool ExpandedByDefault { get; }

        public FoldoutGroupAttribute(string groupName, int order = 0, bool expandedByDefault = true)
        {
            GroupName = string.IsNullOrEmpty(groupName) ? "Group" : groupName;
            Order = order;
            ExpandedByDefault = expandedByDefault;
        }
    }
}
