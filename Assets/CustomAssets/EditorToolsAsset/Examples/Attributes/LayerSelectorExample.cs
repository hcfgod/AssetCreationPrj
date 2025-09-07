using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class LayerSelectorExample : MonoBehaviour
    {
        [Title("Layer Selector (int index)", "#96CEB4")]
        [LayerSelector]
        public int enemyLayerIndex = 0; // Default layer index

        [Title("Layer Selector (string name)", "#96CEB4")]
        [LayerSelector]
        public string effectsLayerName = "Default";
    }
}

