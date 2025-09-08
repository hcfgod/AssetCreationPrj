#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace CustomAssets.EditorTools
{
    public abstract class KEditorWindow<T> : EditorWindow where T : KEditorWindow<T>
    {
        private static T _instance;

        // Persisted state
        private string _prefsPrefix;
        private string _searchQuery;
        protected Vector2 scrollPos;

        // Repaint scheduling
        private bool _autoRepaintEnabled;
        private double _nextRepaintTime;

        // Hooks
        private bool _selectionHookActive;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetWindow<T>();
                return _instance;
            }
        }

        /// <summary>
        /// Override to return true to prevent assembly reloads while this window is open.
        /// Useful for wrapper-style windows that would lose state on reload. Defaults to false.
        /// </summary>
        protected virtual bool PreventAssemblyReloadWhileOpen => false;

        /// <summary>
        /// Override to repaint this window periodically via EditorApplication.update.
        /// </summary>
        protected virtual bool RepaintOnUpdate => false;

        /// <summary>
        /// Target repaint frequency when RepaintOnUpdate is enabled.
        /// </summary>
        protected virtual double RepaintTargetFps => 20.0; // 20 FPS by default

        /// <summary>
        /// Override to repaint this window when the editor selection changes.
        /// </summary>
        protected virtual bool RepaintOnSelectionChanges => false;

        /// <summary>
        /// Current search query, persisted per-window type.
        /// </summary>
        protected string SearchQuery
        {
            get => _searchQuery ?? string.Empty;
            set
            {
                string newValue = value ?? string.Empty;
                if (_searchQuery == newValue) return;
                _searchQuery = newValue;
                EditorPrefs.SetString(GetPrefsKey("search"), _searchQuery);
                try { OnSearchChanged(_searchQuery); } catch { }
                Repaint();
            }
        }

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

            // Setup prefs prefix once
            if (string.IsNullOrEmpty(_prefsPrefix))
                _prefsPrefix = $"KEditorWindow.{typeof(T).FullName}.";

            // Load persisted state
            _searchQuery = EditorPrefs.GetString(GetPrefsKey("search"), string.Empty);

            // Apply hooks
            if (PreventAssemblyReloadWhileOpen)
            {
                try { EditorApplication.LockReloadAssemblies(); } catch { }
            }

            if (RepaintOnUpdate)
            {
                EnableAutoRepaint(RepaintTargetFps);
            }

            if (RepaintOnSelectionChanges)
            {
                EnableRepaintOnSelectionChange();
            }
        }

        protected virtual void OnDisable()
        {
            // Persist on close
            try { EditorPrefs.SetString(GetPrefsKey("search"), _searchQuery ?? string.Empty); } catch { }

            // Unhook
            if (PreventAssemblyReloadWhileOpen)
            {
                try { EditorApplication.UnlockReloadAssemblies(); } catch { }
            }

            DisableAutoRepaint();
            DisableRepaintOnSelectionChange();

            if (ReferenceEquals(_instance, this)) _instance = null;
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

        // =============== Helpful IMGUI helpers ===============

        /// <summary>
        /// Draw a toolbar row. Left and right areas are actions you provide. Optionally include a built-in search field.
        /// Returns true if the search query changed this frame.
        /// </summary>
        protected bool Toolbar(System.Action drawLeft = null, System.Action drawRight = null, bool includeSearch = false, string searchPlaceholder = "Search...")
        {
            bool changed = false;
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                drawLeft?.Invoke();
                GUILayout.FlexibleSpace();

                if (includeSearch)
                {
                    changed |= ToolbarSearch(ref _searchQuery, searchPlaceholder);
                }

                drawRight?.Invoke();
            }
            EditorGUILayout.EndHorizontal();
            if (changed)
            {
                // persist and notify
                SearchQuery = _searchQuery;
            }
            return changed;
        }

        /// <summary>
        /// Draws a toolbar-styled search field with a clear button. Returns true if value changed.
        /// </summary>
        protected bool ToolbarSearch(ref string query, string placeholder = "Search...")
        {
            if (query == null) query = string.Empty;
            Rect r = GUILayoutUtility.GetRect(200f, 200f, 18f, 18f, EditorStyles.toolbarSearchField, GUILayout.MinWidth(120));
            EditorGUI.BeginChangeCheck();
            string newVal = EditorGUI.TextField(r, query, EditorStyles.toolbarSearchField);
            bool changed = EditorGUI.EndChangeCheck();

            // Clear button overlay (fallback style for broad Unity version support)
            Rect clearRect = r;
            clearRect.width = 18f;
            clearRect.x = r.xMax - clearRect.width;
            if (!string.IsNullOrEmpty(newVal) && GUI.Button(clearRect, new GUIContent("x"), EditorStyles.miniButton))
            {
                newVal = string.Empty;
                GUIUtility.keyboardControl = 0;
                changed = true;
            }

            if (changed)
            {
                query = newVal;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws a subtle status/footer bar.
        /// </summary>
        protected void Footer(string left, string right = null)
        {
            Rect r = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.helpBox.Draw(r, GUIContent.none, false, false, false, false);
            }
            r.x += 6; r.width -= 12;
            var leftRect = new Rect(r.x, r.y, r.width * 0.6f, r.height);
            var rightRect = new Rect(r.x + r.width * 0.6f, r.y, r.width * 0.4f, r.height);
            EditorGUI.LabelField(leftRect, left ?? string.Empty, EditorStyles.miniLabel);
            if (!string.IsNullOrEmpty(right))
            {
                GUIStyle alignRight = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
                EditorGUI.LabelField(rightRect, right, alignRight);
            }
        }

        /// <summary>
        /// Horizontal group helper.
        /// </summary>
        protected void Horizontal(System.Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Vertical group helper.
        /// </summary>
        protected void Vertical(System.Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a box group with padding around the provided content.
        /// </summary>
        protected void Box(System.Action content)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2);
            content?.Invoke();
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Utility to safely perform a section of GUI with change detection.
        /// </summary>
        protected bool WithChangeCheck(System.Action content)
        {
            EditorGUI.BeginChangeCheck();
            content?.Invoke();
            return EditorGUI.EndChangeCheck();
        }

        /// <summary>
        /// Handle generic drag-and-drop onto a given Rect; invokes onDrop with valid objects.
        /// </summary>
        protected void HandleDragAndDrop(Rect dropArea, System.Func<Object, bool> accept, System.Action<Object[]> onDrop)
        {
            var evt = Event.current;
            if (!dropArea.Contains(evt.mousePosition)) return;

            if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            {
                bool any = false;
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (accept == null || accept(obj))
                    {
                        any = true; break;
                    }
                }
                DragAndDrop.visualMode = any ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                if (evt.type == EventType.DragPerform && any)
                {
                    DragAndDrop.AcceptDrag();
                    var accepted = System.Array.FindAll(DragAndDrop.objectReferences, o => accept == null || accept(o));
                    onDrop?.Invoke(accepted);
                }
                evt.Use();
            }
        }

        /// <summary>
        /// Persist and retrieve window-specific editor preferences with automatic namespacing.
        /// </summary>
        protected string GetPrefsKey(string subKey) => _prefsPrefix + (subKey ?? string.Empty);

        /// <summary>
        /// Hook called when SearchQuery value changes.
        /// </summary>
        protected virtual void OnSearchChanged(string newQuery) { }

        // =============== Hooks management ===============

        /// <summary>
        /// Enables periodic repaint at a given target FPS.
        /// </summary>
        protected void EnableAutoRepaint(double targetFps)
        {
            if (_autoRepaintEnabled) return;
            _autoRepaintEnabled = true;
            _nextRepaintTime = EditorApplication.timeSinceStartup;
            EditorApplication.update -= UpdateRepaint;
            EditorApplication.update += UpdateRepaint;
        }

        protected void DisableAutoRepaint()
        {
            if (!_autoRepaintEnabled) return;
            _autoRepaintEnabled = false;
            EditorApplication.update -= UpdateRepaint;
        }

        private void UpdateRepaint()
        {
            if (!_autoRepaintEnabled) return;
            double interval = RepaintTargetFps > 0.0 ? 1.0 / RepaintTargetFps : 0.05;
            double now = EditorApplication.timeSinceStartup;
            if (now >= _nextRepaintTime)
            {
                _nextRepaintTime = now + interval;
                try { Repaint(); } catch { }
            }
        }

        protected void EnableRepaintOnSelectionChange()
        {
            if (_selectionHookActive) return;
            _selectionHookActive = true;
            Selection.selectionChanged -= Repaint;
            Selection.selectionChanged += Repaint;
        }

        protected void DisableRepaintOnSelectionChange()
        {
            if (!_selectionHookActive) return;
            _selectionHookActive = false;
            Selection.selectionChanged -= Repaint;
        }
    }
}
#endif