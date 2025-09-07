using UnityEngine;
using System.Collections.Generic;
using CustomAssets.EditorTools;

namespace CustomAssets.EditorTools.Examples
{
    public class TitleReorderableListTest : MonoBehaviour
    {
        [Header("Unity Header Works Above List")]
        [ReorderableList]
        public List<string> unityHeaderList = new List<string> { "Item 1", "Item 2" };

        [Space(20)]
        
        [Title("Custom Title Above List", "#FF6B6B")]
        [ReorderableList]
        public List<string> customTitleList = new List<string> { "Item A", "Item B" };

        [Space(20)]

        [Title("Custom Title Works On Normal Field", "#4ECDC4")]
        public string normalField = "This shows Title correctly";

        [Space(20)]

        [Header("Unity Header On Normal Field")]
        public string unityHeaderField = "Unity header also works";
    }
}
