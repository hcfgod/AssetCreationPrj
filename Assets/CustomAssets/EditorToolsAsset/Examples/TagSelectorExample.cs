using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class TagSelectorExample : MonoBehaviour
    {
        [Title("Tag Selector", "#45B7D1")]
        [TagSelector]
        public string targetTag = "Untagged";
    }
}