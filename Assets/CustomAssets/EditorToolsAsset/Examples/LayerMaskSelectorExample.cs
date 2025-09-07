using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class LayerMaskSelectorExample : MonoBehaviour
    {
        [Title("LayerMask Selector (LayerMask)", "#FFD166")]
        [LayerMaskSelector]
        public LayerMask affectedLayers;

        [Title("LayerMask Selector (int bitmask)", "#FFD166")]
        [LayerMaskSelector]
        public int affectedLayersMask;
    }
}