using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Renders a preview thumbnail for object reference fields (Sprite, Texture, Material, Prefab, etc.).
    /// Optionally draws the object field above the preview.
    /// </summary>
    public class AssetPreviewAttribute : PropertyAttribute
    {
        public float Width { get; }
        public float Height { get; }
        public bool ShowObjectField { get; }
        public bool AllowSceneObjects { get; }

        public AssetPreviewAttribute(float width = 64f, float height = 64f, bool showObjectField = true, bool allowSceneObjects = false)
        {
            Width = Mathf.Max(16f, width);
            Height = Mathf.Max(16f, height);
            ShowObjectField = showObjectField;
            AllowSceneObjects = allowSceneObjects;
        }
    }
}

