using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class AssetPreviewExample : MonoBehaviour
    {
        [Title("Sprite Preview", "#96CEB4")]
        [AssetPreview(96, 96)]
        public Sprite sprite;

        [Title("Texture Preview", "#45B7D1")]
        [AssetPreview(128, 72)]
        public Texture2D texture;

        [Title("Material Preview", "#FFEAA7")]
        [AssetPreview(128, 128)]
        public Material material;

        [Title("Prefab Preview", "#FF6B6B")]
        [AssetPreview(128, 96)]
        public GameObject prefab;
    }
}

