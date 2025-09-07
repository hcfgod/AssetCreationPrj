using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class AssetPreviewExample : MonoBehaviour
    {
        [Title("Sprite Preview", "#96CEB4")]
        [AssetPreview(96, 96, showObjectField: true, allowSceneObjects: false, backgroundHex: "#222222", tintHex: "#FFFFFF", keepAspect: true)]
        public Sprite sprite;

        [Title("Texture Preview", "#45B7D1")]
        [AssetPreview(128, 72, showObjectField: true, allowSceneObjects: false, backgroundHex: "#1E1E1E", tintHex: "#4ECDC4", keepAspect: false)]
        public Texture2D texture;

        [Title("Material Preview", "#FFEAA7")]
        [AssetPreview(128, 128)]
        public Material material;

        [Title("Prefab Preview", "#FF6B6B")]
        [AssetPreview(128, 96)]
        public GameObject prefab;

        [Title("Preview from Path (string)", "#A0A0A0")]
        [AssetPreview(96, 96, showObjectField: true)]
        public string assetPath;
    }
}

