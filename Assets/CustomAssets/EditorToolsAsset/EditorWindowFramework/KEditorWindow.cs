using UnityEngine;
using UnityEditor;

namespace CustomAssets.EditorTools
{
    public abstract class KEditorWindow<T> : EditorWindow where T : KEditorWindow<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetWindow<T>();
                return _instance;
            }
        }

        protected Vector2 scrollPos;

        public static void ShowWindow(string title = null)
        {
            _instance = GetWindow<T>();
            if (!string.IsNullOrEmpty(title))
                _instance.titleContent = new GUIContent(title);
            _instance.Show();
        }

        protected virtual void OnEnable()
        {
            _instance = (T)this;
        }

        protected virtual void OnDisable()
        {
            if (Instance == this) _instance = null;
        }

        protected void ScrollView(System.Action content)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            content?.Invoke();
            EditorGUILayout.EndScrollView();
        }

        protected void Header(string label)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        protected void Separator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
        }
    }
}