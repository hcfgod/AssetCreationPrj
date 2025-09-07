using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class ReorderableListExample : MonoBehaviour
    {
        [Serializable]
        public class Loot
        {
            public string id;
            public int quantity = 1;
        }

        [Title("This is an Array of ints", "#4ECDC4")]
        [ReorderableList]
        public int[] levels = new[] { 1, 2, 3 };

        [Title("List of strings")]
        [ReorderableList("Inventory Items")]
        public List<string> items = new List<string> { "Sword", "Potion" };

        [Title("List of complex objects")]
        [ReorderableList("Loot Table")]
        public List<Loot> loot = new List<Loot> { new Loot { id = "gold", quantity = 10 } };
    }
}
