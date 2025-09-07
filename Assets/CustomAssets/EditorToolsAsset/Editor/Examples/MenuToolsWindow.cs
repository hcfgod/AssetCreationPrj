#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using CustomAssets.EditorTools.Editor;

namespace CustomAssets.EditorTools.Examples
{
    // Example: menu-driven tools window derived from EditorToolsMenuWindow
    public class MenuToolsWindow : EditorToolsMenuWindow
    {
        [MenuItem("Window/Editor Tools/Menu Tools")] private static void Open()
        {
            GetWindow<MenuToolsWindow>(false, "Menu Tools", true).Show();
        }

        protected override void BuildMenu(MenuTree menu)
        {
            // Custom panels implementing IEditorToolsPanel (drawn via OnGUI)
            var hello = ScriptableObject.CreateInstance<HelloPanel>();
            hello.hideFlags = HideFlags.DontSave;
            menu.Add("Examples/Hello", hello);

            var ops = ScriptableObject.CreateInstance<OperationsPanel>();
            ops.hideFlags = HideFlags.DontSave;
            menu.Add("Examples/Operations", ops);

            // Plain ScriptableObject drawn with default inspector (fallback)
            var settings = ScriptableObject.CreateInstance<SimpleSettings>();
            settings.hideFlags = HideFlags.DontSave;
            menu.Add("General/Settings", settings);

            // Unity object (shows Unity's default inspector in right pane)
            var tempMat = new Material(Shader.Find("Sprites/Default")) { hideFlags = HideFlags.DontSave };
            menu.Add("Objects/Temp Material", tempMat);
        }
    }

    // A simple settings object to demonstrate inspector fallback
    public class SimpleSettings : ScriptableObject
    {
        [SerializeField] private string projectName = "My Project";
        [SerializeField] private Color accent = new Color(0.31f, 0.8f, 0.77f);
        [SerializeField] private int quality = 2;

        public string ProjectName => projectName;
        public Color Accent => accent;
        public int Quality => quality;

        public void Reset()
        {
            projectName = "My Project";
            accent = new Color(0.31f, 0.8f, 0.77f);
            quality = 2;
        }
    }

    // Custom panel rendered via IEditorToolsPanel
    public class HelloPanel : ScriptableObject, IEditorToolsPanel
    {
        [SerializeField] private string message = "Hello Menu Tools!";
        [SerializeField] private float scale = 1f;

        public void OnGUI()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField("Hello Panel", EditorStyles.boldLabel);
            message = EditorGUILayout.TextField("Message", message);
            scale = EditorGUILayout.Slider("Scale", scale, 0.5f, 3f);

            GUILayout.Space(8);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                var style = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, fontSize = Mathf.RoundToInt(12 * scale) };
                GUILayout.Label(message, style, GUILayout.Height(30 * scale));
            }
        }
    }

    public class OperationsPanel : ScriptableObject, IEditorToolsPanel
    {
        [SerializeField] private string tagToFind = "Untagged";
        [SerializeField] private LayerMask layers;

        public void OnGUI()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField("Operations", EditorStyles.boldLabel);
            tagToFind = EditorGUILayout.TagField("Tag", tagToFind);
            layers = EditorGUILayout.LayerField("Layer (single)", layers);

            GUILayout.Space(4);
            if (GUILayout.Button("Log Selection Info"))
            {
                Debug.Log($"Selected: {Selection.objects.Length} objects. Tag='{tagToFind}', Layers={layers.value}");
            }
        }
    }
}
#endif

