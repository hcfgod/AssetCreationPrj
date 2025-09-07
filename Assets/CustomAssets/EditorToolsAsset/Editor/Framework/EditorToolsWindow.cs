#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    /// <summary>
    /// Base class for rapidly creating custom editor windows.
    /// Just derive from this class and add [SerializeField] fields — they will be drawn like an inspector.
    /// All your existing attributes and drawers (Title, FoldoutGroup, MinMaxSlider, etc.) work here too.
    /// </summary>
    public abstract class EditorToolsWindow : EditorWindow
    {
        private SerializedObject _so;
        private Vector2 _scroll;

        protected virtual void OnEnable()
        {
            _so = new SerializedObject(this);
        }

        protected virtual void OnDisable()
        {
            _so = null;
        }

        protected virtual void OnGUI()
        {
            if (_so == null)
                _so = new SerializedObject(this);

            // Toolbar
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Repaint", EditorStyles.toolbarButton, GUILayout.Width(70)))
                    Repaint();
                GUILayout.FlexibleSpace();
                DrawToolbarRight();
            }

            _so.UpdateIfRequiredOrScript();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            try
            {
                var it = _so.GetIterator();
                bool enterChildren = true;
                while (it.NextVisible(enterChildren))
                {
                    enterChildren = false;
                    if (it.propertyPath == "m_Script")
                        continue;
                    EditorGUILayout.PropertyField(it, true);
                }
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }

            _so.ApplyModifiedProperties();
        }

        /// <summary>
        /// Optional right side of the toolbar.
        /// </summary>
        protected virtual void DrawToolbarRight() { }
    }
}
#endif