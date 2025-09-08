#if UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

namespace CustomAssets.EditorTools.Editor
{
    internal static class EditorGlobalShortcuts
    {
        private static EditorWindow _tempProjectPopup;
        private static ToolbarSearchField _searchFieldRef;

        // Static init to handle assembly reloads cleanly
        static EditorGlobalShortcuts()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        // Ctrl+Space on Windows, Cmd+Space on macOS (Action modifier maps accordingly)
        [Shortcut("EditorTools/Show Project Window (Popup)", KeyCode.Space, ShortcutModifiers.Action)]
        private static void ShowProjectWindowPopup()
        {
            // Toggle: if already open, close it
            if (_tempProjectPopup != null)
            {
                try { _tempProjectPopup.Close(); } catch { }
                _tempProjectPopup = null;
                CleanupCallbacks();
                return;
            }

            var projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser, UnityEditor");
            if (projectBrowserType == null)
            {
                // Fallback to regular menu if type not found
                EditorApplication.ExecuteMenuItem("Window/General/Project");
                return;
            }

            _tempProjectPopup = ScriptableObject.CreateInstance(projectBrowserType) as EditorWindow;
            if (_tempProjectPopup == null)
            {
                EditorApplication.ExecuteMenuItem("Window/General/Project");
                return;
            }

            _tempProjectPopup.titleContent = new GUIContent("Project");

            Vector2 size = new Vector2(720f, 420f);
            Rect baseRect;

            // Try to position at the OS mouse cursor (Windows); otherwise center on focused window
            Vector2 mouse = GetMouseScreenPositionFallback();
            if (mouse.x >= 0 && mouse.y >= 0)
            {
                float x = Mathf.Round(mouse.x - size.x * 0.5f);
                float y = Mathf.Round(mouse.y - size.y * 0.5f);
                baseRect = new Rect(x, y, size.x, size.y);
            }
            else
            {
                var focused = EditorWindow.focusedWindow;
                if (focused != null)
                {
                    var pos = focused.position;
                    float x = Mathf.Round(pos.x + (pos.width - size.x) * 0.5f);
                    float y = Mathf.Round(pos.y + 60f);
                    baseRect = new Rect(x, y, size.x, size.y);
                }
                else
                {
                    baseRect = new Rect(200f, 200f, size.x, size.y);
                }
            }

            _tempProjectPopup.position = baseRect;

            // Show as a borderless popup; we'll add a custom header and make it draggable
            _tempProjectPopup.ShowPopup();

            // Inject a custom header bar for drag + styling
            TryInstallHeaderBar(_tempProjectPopup);

            // Safety: auto-close if focus is elsewhere on editor update
            EditorApplication.update -= AutoCloseIfLostFocus;
            EditorApplication.update += AutoCloseIfLostFocus;
        }

        private static void AutoCloseIfLostFocus()
        {
            if (_tempProjectPopup == null)
            {
                EditorApplication.update -= AutoCloseIfLostFocus;
                return;
            }
            if (EditorWindow.focusedWindow != _tempProjectPopup)
            {
                try { _tempProjectPopup.Close(); } catch { }
                _tempProjectPopup = null;
                CleanupCallbacks();
            }
            else
            {
                // Ensure header exists (domain reloads can rebuild UI)
                TryInstallHeaderBar(_tempProjectPopup);
            }
        }

        private static bool _isDragging;
        private static EditorWindow _dragWindow;
        private static Vector2 _dragOffsetScreen; // mouse - windowTopLeft
        private static void TryInstallHeaderBar(EditorWindow w)
        {
            try
            {
                var root = w.rootVisualElement;
                if (root == null) return;

                // Avoid duplicating header if already installed
                if (root.Q("ProjectPopupHeader") != null) return;

                var header = new VisualElement { name = "ProjectPopupHeader" };
                header.style.height = 22;
                header.style.flexShrink = 0;
                header.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
                header.style.borderBottomWidth = 1;
                header.style.borderBottomColor = new Color(0f, 0f, 0f, 0.35f);
                header.style.paddingLeft = 6;
                header.style.paddingRight = 6;
                header.style.alignItems = Align.Center;
                header.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                // Ensure header captures pointer events and sits above underlying content
                header.pickingMode = PickingMode.Position;
                // Block tooltip/mouse-move from passing to underlying controls

                var title = new Label("Project");
                title.style.color = Color.white;
                title.style.unityFontStyleAndWeight = FontStyle.Bold;
                title.style.marginLeft = 0;
                title.style.marginRight = 8;
                title.style.flexShrink = 0;

                var search = new ToolbarSearchField();
                _searchFieldRef = search;
                search.style.flexGrow = 1;
                search.style.flexShrink = 1;
                search.style.minWidth = 0; // allow shrinking within row
                search.style.marginLeft = 0;
                search.style.marginRight = 6;
                search.tooltip = "Search assets";

                var headerRow = new VisualElement();
                headerRow.style.flexDirection = FlexDirection.Row;
                headerRow.style.flexGrow = 1;
                headerRow.style.alignItems = Align.Center;
                headerRow.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                headerRow.pickingMode = PickingMode.Position;

                var closeBtn = new Button(() =>
                {
                    if (_tempProjectPopup != null)
                    {
                        try { _tempProjectPopup.Close(); } catch { }
                        _tempProjectPopup = null;
                    }
                }) { text = "\u00D7" }; // multiplication sign as close symbol
                closeBtn.style.width = 22;
                closeBtn.style.height = 18;
                closeBtn.style.marginLeft = 6;
                closeBtn.style.flexShrink = 0;
                closeBtn.tooltip = "Close";

                headerRow.Add(title);
                headerRow.Add(search);
                headerRow.Add(closeBtn);
                header.Add(headerRow);

                // Insert header at top so it pushes content down
                root.Insert(0, header);



                // Hook search change -> select matching assets in Project
                search.RegisterValueChangedCallback(evt =>
                {
                    UpdateProjectSearchText(evt.newValue);
                });

                // Keyboard: Ctrl/Cmd+Enter to select/ping; ESC to close
                search.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Escape)
                    {
                        if (_tempProjectPopup != null) { _tempProjectPopup.Close(); _tempProjectPopup = null; }
                        evt.StopImmediatePropagation();
                    }
                    else if ((evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter) && (evt.actionKey || evt.ctrlKey))
                    {
                        var q = search.value;
                        ApplyProjectSearch(q, ping: true);
                        // Return focus to search field after selection/ping effects
                        EditorApplication.delayCall += () => { if (_searchFieldRef != null) _searchFieldRef.Focus(); };
                        evt.StopImmediatePropagation();
                    }
                });

                header.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (evt.button != 0) return;
                    _isDragging = true;
                    _dragWindow = w;
                    Vector2 mouse = GetMouseScreenPositionFallback();
                    if (mouse.x < 0 || mouse.y < 0)
                    {
                        // Fallback compute using window pos + local
                        _dragOffsetScreen = (w.position.position + evt.mousePosition) - w.position.position;
                    }
                    else
                    {
                        _dragOffsetScreen = mouse - w.position.position;
                    }
                    header.CaptureMouse();
                    // Subscribe update loop for smooth dragging
                    EditorApplication.update -= DragUpdate;
                    EditorApplication.update += DragUpdate;
                    evt.StopImmediatePropagation();
                });
                header.RegisterCallback<MouseUpEvent>(evt =>
                {
                    if (evt.button != 0) return;
                    _isDragging = false;
                    header.ReleaseMouse();
                    EditorApplication.update -= DragUpdate;
                    evt.StopImmediatePropagation();
                });
                header.RegisterCallback<MouseCaptureOutEvent>(evt =>
                {
                    _isDragging = false;
                    EditorApplication.update -= DragUpdate;
                });
            }
            catch { }
        }


        private static void DragUpdate()
        {
            if (!_isDragging || _dragWindow == null)
            {
                // stop if window disappeared
                EditorApplication.update -= DragUpdate;
                return;
            }
            Vector2 mouse = GetMouseScreenPositionFallback();
            if (mouse.x < 0 || mouse.y < 0) return;
            Vector2 newPos = mouse - _dragOffsetScreen;
            // Round to whole pixels to avoid blur
            newPos.x = Mathf.Round(newPos.x);
            newPos.y = Mathf.Round(newPos.y);
            var r = _dragWindow.position;
            if (r.position != newPos)
            {
                r.position = newPos;
                _dragWindow.position = r;
                _dragWindow.Repaint();
            }
        }

        private static void ApplyProjectSearch(string query, bool ping)
        {
            if (query == null) query = string.Empty;
            query = query.Trim();
            if (string.IsNullOrEmpty(query))
            {
                // Clear selection on empty search
                Selection.objects = Array.Empty<UnityEngine.Object>();
                return;
            }
            try
            {
                var guids = AssetDatabase.FindAssets(query, new[] { "Assets" });
                var objs = guids.Take(200)
                                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                                .Where(p => !string.IsNullOrEmpty(p))
                                .Select(p => AssetDatabase.LoadMainAssetAtPath(p))
                                .Where(o => o != null)
                                .ToArray();
                Selection.objects = objs;
                if (ping && objs.Length > 0)
                {
                    EditorGUIUtility.PingObject(objs[0]);
                }
            }
            catch { }
        }

        private static void UpdateProjectSearchText(string query)
        {
            try
            {
                if (_tempProjectPopup == null) return;
                var w = _tempProjectPopup;
                var t = w.GetType();
                // Try reflection method first (SetSearch)
                var mi = t.GetMethod("SetSearch", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
                if (mi != null)
                {
                    mi.Invoke(w, new object[] { query });
                    w.Repaint();
                    return;
                }
                // Try property 'searchString'
                var pi = t.GetProperty("searchString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (pi != null && pi.CanWrite)
                {
                    pi.SetValue(w, query);
                    w.Repaint();
                    return;
                }
                // Try to find a UIElements search field inside the content (skip our header at index 0)
                var root = w.rootVisualElement;
                if (root != null)
                {
                    VisualElement content = root.childCount > 1 ? root[1] : root;
                    var innerSearch = content.Q<ToolbarSearchField>();
                    if (innerSearch != null)
                    {
                        innerSearch.value = query;
                        w.Repaint();
                    }
                }
            }
            catch { }
            finally
            {
                // Keep focus on our search bar to avoid intrusive focus changes
                EditorApplication.delayCall += () => { if (_searchFieldRef != null) _searchFieldRef.Focus(); };
            }
        }

        private static void OnBeforeAssemblyReload()
        {
            // Close any of our popups to avoid stranded windows after reload
            AutoCloseIfLostFocus();
            CloseOurProjectPopups();
            CleanupCallbacks();
            _tempProjectPopup = null;
            _dragWindow = null;
            _isDragging = false;
        }

        private static void OnAfterAssemblyReload()
        {
            // Ensure no orphaned popup persists without our header
            AutoCloseIfLostFocus();
            CloseOurProjectPopups();
            _tempProjectPopup = null;
            _dragWindow = null;
            _isDragging = false;
        }

        private static void CleanupCallbacks()
        {
            EditorApplication.update -= AutoCloseIfLostFocus;
            EditorApplication.update -= DragUpdate;
        }

        private static void CloseOurProjectPopups()
        {
            try
            {
                var projectBrowserType = Type.GetType("UnityEditor.ProjectBrowser, UnityEditor");
                if (projectBrowserType == null) return;
                var objs = Resources.FindObjectsOfTypeAll(projectBrowserType);
                foreach (var o in objs)
                {
                    var w = o as EditorWindow;
                    if (w == null) continue;
                    var root = w.rootVisualElement;
                    if (root != null && root.Q("ProjectPopupHeader") != null)
                    {
                        try { w.Close(); } catch { }
                    }
                }
            }
            catch { }
        }

#if UNITY_EDITOR_WIN
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }
        [DllImport("user32.dll")] private static extern bool GetCursorPos(out POINT lpPoint);
#endif
        private static Vector2 GetMouseScreenPositionFallback()
        {
#if UNITY_EDITOR_WIN
            try
            {
                if (GetCursorPos(out POINT p))
                    return new Vector2(p.X, p.Y);
            }
            catch { }
#endif
            // Fallback: indicate invalid so we use focused window centering
            return new Vector2(-1f, -1f);
        }
    }
}
#endif

