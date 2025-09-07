#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Simple menu tree for building custom editor windows with a left-side menu.
    /// Add UnityEngine.Object pages via hierarchical paths (e.g., "Gameplay/AI/Agents").
    /// If a page implements IEditorToolsPanel, its OnGUI() is called; otherwise, a default inspector is shown.
    /// </summary>
    public abstract class EditorToolsMenuWindow : EditorWindow
    {
        internal class Node
        {
            public string Name;
            public Dictionary<string, Node> Children = new Dictionary<string, Node>();
            public UnityEngine.Object Target; // leaf only
            public bool Expanded = true;
        }

        protected class MenuTree
        {
            internal Node Root = new Node { Name = "Root" };
            public void Add(string path, UnityEngine.Object obj)
            {
                if (string.IsNullOrEmpty(path) || obj == null) return;
                var parts = path.Split('/');
                Node cur = Root;
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i].Trim();
                    if (string.IsNullOrEmpty(part)) continue;
                    if (!cur.Children.TryGetValue(part, out var next))
                    {
                        next = new Node { Name = part };
                        cur.Children.Add(part, next);
                    }
                    cur = next;
                }
                cur.Target = obj;
            }
        }

        private UnityEngine.Object _selected;
        private UnityEditor.Editor _selectedEditor;
        private Vector2 _leftScroll;
        private Vector2 _rightScroll;
        private float _leftWidth = 220f;
        private MenuTree _menu;

        protected abstract void BuildMenu(MenuTree menu);

        [MenuItem("Window/Editor Tools/Menu Window Example")]
        private static void OpenExample()
        {
            GetWindow<EditorToolsMenuWindowExample>(false, "Tools", true).Show();
        }

        protected virtual void OnEnable()
        {
            RebuildMenu();
        }

        protected void RebuildMenu()
        {
            _menu = new MenuTree();
            BuildMenu(_menu);
            if (_selected == null)
            {
                // auto-select first leaf
                _selected = FindFirstLeaf(_menu.Root);
            }
            RefreshEditor();
        }

        private UnityEngine.Object FindFirstLeaf(Node n)
        {
            if (n == null) return null;
            if (n.Target != null) return n.Target;
            foreach (var kv in n.Children)
            {
                var o = FindFirstLeaf(kv.Value);
                if (o != null) return o;
            }
            return null;
        }

        private void RefreshEditor()
        {
            if (_selectedEditor != null)
            {
                DestroyImmediate(_selectedEditor);
                _selectedEditor = null;
            }
            if (_selected != null && !(_selected is IEditorToolsPanel))
            {
                _selectedEditor = UnityEditor.Editor.CreateEditor(_selected);
            }
        }

        protected virtual void OnGUI()
        {
            DrawToolbar();

            Rect r = position;
            Rect left = new Rect(0, EditorStyles.toolbar.fixedHeight, _leftWidth, r.height - EditorStyles.toolbar.fixedHeight);
            Rect right = new Rect(left.width + 1, EditorStyles.toolbar.fixedHeight, r.width - left.width - 1, r.height - EditorStyles.toolbar.fixedHeight);

            // Left panel
            GUILayout.BeginArea(left);
            _leftScroll = EditorGUILayout.BeginScrollView(_leftScroll);
            DrawNode(_menu.Root, 0);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            // Splitter (use dedicated control ID to avoid interfering with other drags)
            Rect splitterRect = new Rect(left.xMax - 2, left.y, 5, left.height);
            EditorGUI.DrawRect(new Rect(left.xMax, left.y, 1, left.height), new Color(0,0,0,0.3f));
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);

            int splitterId = GUIUtility.GetControlID("EditorToolsMenuSplitter".GetHashCode(), FocusType.Passive);
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (splitterRect.Contains(e.mousePosition))
                    {
                        GUIUtility.hotControl = splitterId;
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == splitterId)
                    {
                        _leftWidth = Mathf.Clamp(e.mousePosition.x, 140f, position.width - 200f);
                        e.Use();
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == splitterId)
                    {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }
                    break;
            }

            // Right panel
            GUILayout.BeginArea(right);
            _rightScroll = EditorGUILayout.BeginScrollView(_rightScroll);
            DrawRightPane();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawNode(Node node, int depth)
        {
            if (node == null) return;
            foreach (var kv in node.Children)
            {
                var child = kv.Value;
                Rect rect;
                if (child.Children.Count > 0 && child.Target == null)
                {
                    rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                    child.Expanded = EditorGUI.Foldout(rect, child.Expanded, new GUIContent(child.Name), true);
                    if (child.Expanded)
                        DrawNode(child, depth+1);
                }
                else
                {
                    rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                    rect.x += 12f * depth;
                    bool selected = _selected == child.Target;
                    if (GUI.Button(rect, new GUIContent(child.Name), selected ? EditorStyles.boldLabel : EditorStyles.label))
                    {
                        _selected = child.Target;
                        RefreshEditor();
                    }
                }
            }
        }

        private void DrawRightPane()
        {
            if (_selected == null)
            {
                EditorGUILayout.HelpBox("No page selected.", MessageType.Info);
                return;
            }

            if (_selected is IEditorToolsPanel panel)
            {
                panel.OnGUI();
                return;
            }

            if (_selectedEditor == null)
            {
                _selectedEditor = UnityEditor.Editor.CreateEditor(_selected);
                if (_selectedEditor == null)
                {
                    EditorGUILayout.HelpBox("Could not create editor for selection.", MessageType.Error);
                    return;
                }
            }
            _selectedEditor.OnInspectorGUI();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    RebuildMenu();

                GUILayout.FlexibleSpace();
            }
        }
    }

    /// <summary>
    /// Implement to provide custom panel content.
    /// </summary>
    public interface IEditorToolsPanel
    {
        void OnGUI();
    }

    /// <summary>
    /// Minimal example window; override BuildMenu to register pages.
    /// </summary>
    public class EditorToolsMenuWindowExample : EditorToolsMenuWindow
    {
        protected override void BuildMenu(MenuTree menu)
        {
            // Example runtime pages
            var sample = ScriptableObject.CreateInstance<SamplePanel>();
            sample.hideFlags = HideFlags.DontSave;
            menu.Add("Examples/Sample Panel", sample);

            // You can add any UnityEngine.Object, including assets from your project, MonoBehaviours, materials, etc.
            // menu.Add("Project/Main Camera", Camera.main);
        }
    }

    public class SamplePanel : ScriptableObject, IEditorToolsPanel
    {
        [SerializeField] private string message = "Hello from Custom Panel";
        [SerializeField] private Color color = Color.cyan;

        public void OnGUI()
        {
            GUILayout.Space(6);
            EditorGUILayout.LabelField("Sample Panel", EditorStyles.boldLabel);
            message = EditorGUILayout.TextField("Message", message);
            color = EditorGUILayout.ColorField("Color", color);
            GUILayout.Space(6);
            using (new EditorGUILayout.VerticalScope("box"))
            {
                var prev = GUI.color;
                GUI.color = color;
                GUILayout.Label(message);
                GUI.color = prev;
            }
        }
    }
}
#endif

