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

        /// <summary>
        /// Draws a progress bar with optional label and percentage display.
        /// </summary>
        protected void ProgressBar(float progress, string label = null, bool showPercentage = true)
        {
            progress = Mathf.Clamp01(progress);
            Rect rect = EditorGUILayout.GetControlRect(false, 18f);

            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(rect, new Color(0.13f, 0.13f, 0.13f));
                Rect fillRect = new Rect(rect.x + 1, rect.y + 1, (rect.width - 2) * progress, rect.height - 2);
                EditorGUI.DrawRect(fillRect, new Color(0.2f, 0.6f, 1f));
            }

            string displayText = label ?? string.Empty;
            if (showPercentage)
            {
                string percent = $"{(progress * 100):F0}%";
                displayText = string.IsNullOrEmpty(displayText) ? percent : $"{displayText} ({percent})";
            }

            if (!string.IsNullOrEmpty(displayText))
            {
                GUI.Label(rect, displayText, EditorStyles.centeredGreyMiniLabel);
            }
        }

        /// <summary>
        /// Draws an asset preview thumbnail with optional label. Returns true if clicked.
        /// </summary>
        protected bool AssetPreview(Object asset, float size = 64f, string label = null)
        {
            if (asset == null) return false;

            bool clicked = false;
            Vertical(() =>
            {
                Texture2D preview = UnityEditor.AssetPreview.GetAssetPreview(asset);
                if (preview != null)
                {
                    Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
                    if (GUI.Button(rect, preview, GUIStyle.none))
                    {
                        clicked = true;
                    }
                }
                else
                {
                    Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
                    if (GUI.Button(rect, "?", EditorStyles.miniButton))
                    {
                        clicked = true;
                    }
                }

                if (!string.IsNullOrEmpty(label))
                {
                    EditorGUILayout.LabelField(label, EditorStyles.centeredGreyMiniLabel, GUILayout.Width(size));
                }
            });

            return clicked;
        }

        /// <summary>
        /// Creates a foldout group that persists its state in EditorPrefs.
        /// </summary>
        protected bool Foldout(string key, string label, System.Action content, bool defaultExpanded = false)
        {
            bool expanded = EditorPrefs.GetBool(GetPrefsKey($"foldout.{key}"), defaultExpanded);
            bool newExpanded = EditorGUILayout.Foldout(expanded, label, true);

            if (newExpanded != expanded)
            {
                EditorPrefs.SetBool(GetPrefsKey($"foldout.{key}"), newExpanded);
            }

            if (newExpanded)
            {
                EditorGUI.indentLevel++;
                content?.Invoke();
                EditorGUI.indentLevel--;
            }

            return newExpanded;
        }

        /// <summary>
        /// Tab group that persists selected tab in EditorPrefs.
        /// </summary>
        protected int TabGroup(string key, string[] tabLabels, System.Action<int>[] tabContents, int defaultTab = 0)
        {
            if (tabLabels == null || tabLabels.Length == 0) return -1;

            int selectedTab = EditorPrefs.GetInt(GetPrefsKey($"tab.{key}"), defaultTab);
            selectedTab = Mathf.Clamp(selectedTab, 0, tabLabels.Length - 1);

            int newSelectedTab = GUILayout.Toolbar(selectedTab, tabLabels);
            if (newSelectedTab != selectedTab)
            {
                EditorPrefs.SetInt(GetPrefsKey($"tab.{key}"), newSelectedTab);
                selectedTab = newSelectedTab;
            }

            GUILayout.Space(4);

            if (tabContents != null && selectedTab < tabContents.Length)
            {
                tabContents[selectedTab]?.Invoke(selectedTab);
            }

            return selectedTab;
        }

        /// <summary>
        /// Enhanced color field with preset colors and optional alpha.
        /// </summary>
        protected Color ColorPicker(string label, Color color, bool showAlpha = true, params Color[] presets)
        {
            Color newColor = color;

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                newColor = EditorGUILayout.ColorField(color, GUILayout.ExpandWidth(false));

                if (presets != null && presets.Length > 0)
                {
                    GUILayout.Space(4);
                    foreach (var preset in presets)
                    {
                        Rect presetRect = GUILayoutUtility.GetRect(16, 16, GUILayout.Width(16), GUILayout.Height(16));
                        if (Event.current.type == EventType.Repaint)
                        {
                            EditorGUI.DrawRect(presetRect, preset);
                        }
                        if (GUI.Button(presetRect, GUIContent.none, GUIStyle.none))
                        {
                            newColor = showAlpha ? preset : new Color(preset.r, preset.g, preset.b, color.a);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return newColor;
        }

        /// <summary>
        /// Draws a help box with different message types.
        /// </summary>
        protected void HelpBox(string message, MessageType messageType = MessageType.Info)
        {
            EditorGUILayout.HelpBox(message, messageType);
        }

        /// <summary>
        /// Creates a two-column layout helper.
        /// </summary>
        protected void TwoColumns(System.Action leftColumn, System.Action rightColumn, float leftWidth = 200f)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(leftWidth));
                leftColumn?.Invoke();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                rightColumn?.Invoke();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a button with an icon and text.
        /// </summary>
        protected bool IconButton(string text, string iconName, params GUILayoutOption[] options)
        {
            Texture2D icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;
            GUIContent content = new GUIContent(text, icon);
            return GUILayout.Button(content, options);
        }

        /// <summary>
        /// Creates a selection grid with automatic wrapping.
        /// </summary>
        protected int SelectionGrid(int selected, string[] options, int maxColumns = 3, params GUILayoutOption[] options_)
        {
            if (options == null || options.Length == 0) return -1;
            int columns = Mathf.Min(maxColumns, options.Length);
            return GUILayout.SelectionGrid(selected, options, columns, options_);
        }

        /// <summary>
        /// Enhanced object field with drag-and-drop highlight.
        /// </summary>
        protected TObject ObjectField<TObject>(string label, TObject obj, bool allowSceneObjects = true) where TObject : Object
        {
            return EditorGUILayout.ObjectField(label, obj, typeof(TObject), allowSceneObjects) as TObject;
        }

        /// <summary>
        /// Creates a min-max slider.
        /// </summary>
        protected void MinMaxSlider(string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, minLimit, maxLimit);
                minValue = EditorGUILayout.FloatField(minValue, GUILayout.Width(50));
                maxValue = EditorGUILayout.FloatField(maxValue, GUILayout.Width(50));
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Creates a centered label with optional styling.
        /// </summary>
        protected void CenteredLabel(string text, bool bold = false, int fontSize = 0)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = bold ? FontStyle.Bold : FontStyle.Normal
            };

            if (fontSize > 0)
            {
                style.fontSize = fontSize;
            }

            EditorGUILayout.LabelField(text, style);
        }

        /// <summary>
        /// Creates a button that's only enabled if a condition is met.
        /// </summary>
        protected bool ConditionalButton(string text, bool condition, string disabledTooltip = null)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = condition;

            GUIContent content = new GUIContent(text, condition ? null : disabledTooltip);
            bool clicked = GUILayout.Button(content);

            GUI.enabled = wasEnabled;
            return clicked;
        }

        /// <summary>
        /// Creates a toggle button that shows its state visually.
        /// </summary>
        protected bool ToggleButton(string text, bool value, params GUILayoutOption[] options)
        {
            Color originalColor = GUI.backgroundColor;
            if (value)
            {
                GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
            }

            bool clicked = GUILayout.Button(text, options);
            GUI.backgroundColor = originalColor;

            return clicked ? !value : value;
        }

        /// <summary>
        /// Creates a property-like field for Vector2.
        /// </summary>
        protected Vector2 Vector2Field(string label, Vector2 value)
        {
            return EditorGUILayout.Vector2Field(label, value);
        }

        /// <summary>
        /// Creates a property-like field for Vector3.
        /// </summary>
        protected Vector3 Vector3Field(string label, Vector3 value)
        {
            return EditorGUILayout.Vector3Field(label, value);
        }

        /// <summary>
        /// Creates a simple enum popup.
        /// </summary>
        protected TEnum EnumPopup<TEnum>(string label, TEnum value) where TEnum : System.Enum
        {
            return (TEnum)EditorGUILayout.EnumPopup(label, value);
        }

        /// <summary>
        /// Creates a simple mask field for LayerMask.
        /// </summary>
        protected LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            return EditorGUILayout.MaskField(label, layerMask, UnityEditorInternal.InternalEditorUtility.layers);
        }

        /// <summary>
        /// Draws a texture with aspect ratio preserved.
        /// </summary>
        protected void DrawTexture(Texture2D texture, float maxWidth = 200f, float maxHeight = 200f)
        {
            if (texture == null) return;

            float aspectRatio = (float)texture.width / texture.height;
            float width = Mathf.Min(maxWidth, texture.width);
            float height = width / aspectRatio;

            if (height > maxHeight)
            {
                height = maxHeight;
                width = height * aspectRatio;
            }

            Rect rect = GUILayoutUtility.GetRect(width, height, GUILayout.Width(width), GUILayout.Height(height));
            GUI.DrawTexture(rect, texture);
        }
        
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