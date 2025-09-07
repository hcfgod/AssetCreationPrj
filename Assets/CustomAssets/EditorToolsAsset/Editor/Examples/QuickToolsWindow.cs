#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CustomAssets.EditorTools;
using CustomAssets.EditorTools.Editor;

namespace CustomAssets.EditorTools.Examples
{
    // Example: quick tools window derived from EditorToolsWindow
    public class QuickToolsWindow : EditorToolsWindow
    {
        [Title("Quick Tools", "#4ECDC4")] [ReadOnly("Demo only")] public string info = "Use these tools to batch operations.";

        [Title("Tag Ops", "#45B7D1")]
        [TagSelector] public string tagToFind = "Untagged";

        [Title("Layer Ops", "#96CEB4")]
        [LayerMaskSelector] public int layers;

        [Title("Range", "#FFEAA7")]
        [MinMaxSlider(0f, 100f)] public Vector2 range = new Vector2(10, 40);

        [Title("Progress", "#FF6B6B")]
        [ProgressBar(0, 100, label: "Work", hexColor: "#FF6B6B", height: 18f, showValue: true, editable: true)] public int progress = 25;

        [MenuItem("Window/Editor Tools/Quick Tools")] private static void Open()
        {
            var w = GetWindow<QuickToolsWindow>(false, "Quick Tools", true);
            w.Show();
        }

        protected override void DrawToolbarRight()
        {
            if (GUILayout.Button("Run", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                Debug.Log($"Running on tag '{tagToFind}', layers {layers}, range {range}, progress {progress}");
            }
        }
    }
}
#endif

