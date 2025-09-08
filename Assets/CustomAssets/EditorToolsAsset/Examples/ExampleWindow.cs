#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;

public class ExampleWindow : KEditorWindow<ExampleWindow>
{
    // Demo data
    private Color demoColor = Color.blue;
    private Vector2 demoVector2 = new Vector2(1, 2);
    private Vector3 demoVector3 = new Vector3(1, 2, 3);
    private float demoMin = 0f, demoMax = 10f;
    private bool demoToggle = false;
    private LayerMask demoLayerMask = 1;
    private Texture2D demoTexture;
    private float progressValue = 0.3f;
    private int selectedGridItem = 0;
    private string[] gridOptions = { "Option 1", "Option 2", "Option 3", "Option 4" };

    [MenuItem("Tools/Example Window")]
    public static void Open()
    {
        ShowWindow("Example Window");
    }

    private void OnGUI()
    {
        // Toolbar with search
        Toolbar(
            drawLeft: () =>
            {
                if (Button("Refresh", style: EditorStyle.ToolbarButton))
                {
                    Repaint();
                }
            },
            drawRight: () =>
            {
                if (Button("Help", style: EditorStyle.ToolbarButton))
                {
                    EditorUtility.DisplayDialog("Help", "This showcases all the new KEditorWindow IMGUI helpers!", "OK");
                }
            },
            includeSearch: true,
            searchPlaceholder: "Filter items..."
        );

        // Main content using tabs
        TabGroup("main", new[] { "Controls", "Layout", "Visuals", "Theme", "About" }, new System.Action<int>[]
        {
            DrawControlsTab,
            DrawLayoutTab,
            DrawVisualsTab,
            DrawThemeTab,
            DrawAboutTab
        });

        Footer($"Search: '{SearchQuery}' | Items: {gridOptions.Length}", System.DateTime.Now.ToShortTimeString());
    }

    private void DrawControlsTab(int tabIndex)
    {
        Header("Input Controls");
        
        Foldout("basic", "Basic Controls", () =>
        {
            demoColor = ColorPicker("Demo Color", demoColor, true, Color.red, Color.green, Color.blue, Color.yellow);
            demoVector2 = Vector2Field("Vector2", demoVector2);
            demoVector3 = Vector3Field("Vector3", demoVector3);
            MinMaxSlider("Range", ref demoMin, ref demoMax, 0f, 20f);
            demoLayerMask = LayerMaskField("Layer Mask", demoLayerMask);
        });

        Foldout("buttons", "Button Types", () =>
        {
            Horizontal(() =>
            {
                if (GUILayout.Button("Normal Button"))
                {
                    Debug.Log("Normal button clicked!");
                }
                
                demoToggle = ToggleButton("Toggle", demoToggle);
                
                ConditionalButton("Conditional", demoToggle, "Enable toggle above first");
            });
            
            EditorGUILayout.Space();
            
            Horizontal(() =>
            {
                if (AccentButton("Accent Button", EditorStyle.Button, LayoutOption.Width100))
                {
                    Debug.Log("Accent button clicked!");
                }
                
                if (ColoredButton("Red Button", Color.red, EditorStyle.MiniButton, LayoutOption.Width80))
                {
                    Debug.Log("Red button clicked!");
                }
                
                if (ColoredButton("Green Button", Color.green, EditorStyle.ToolbarButton, LayoutOption.Width80))
                {
                    Debug.Log("Green button clicked!");
                }
            });
            
            EditorGUILayout.Space();
            
            Horizontal(() =>
            {
                if (AccentIconButton("Refresh", "Refresh", EditorStyle.ToolbarButton, LayoutOption.Width120))
                {
                    Debug.Log("Refresh icon button clicked!");
                }
                
                if (IconButton("Help", "Help", Color.blue, Color.white, EditorStyle.MiniButton, LayoutOption.Width100))
                {
                    Debug.Log("Help icon button clicked!");
                }
            });
        });

        Foldout("selection", "Selection Controls", () =>
        {
            selectedGridItem = SelectionGrid(selectedGridItem, gridOptions, 2);
            EditorGUILayout.LabelField($"Selected: {gridOptions[selectedGridItem]}");
        });
    }

    private void DrawLayoutTab(int tabIndex)
    {
        Header("Layout Examples");
        
        Foldout("columns", "Two Column Layout", () =>
        {
            TwoColumns(
                leftColumn: () =>
                {
                    CenteredLabel("Left Column", true);
                    EditorGUILayout.LabelField("This is the left side");
                    EditorGUILayout.LabelField("Perfect for settings");
                },
                rightColumn: () =>
                {
                    CenteredLabel("Right Column", true);
                    EditorGUILayout.LabelField("This is the right side");
                    EditorGUILayout.LabelField("Great for previews");
                }
            );
        });

        Foldout("groups", "Layout Groups", () =>
        {
            Box(() =>
            {
                CenteredLabel("Box Group", true);
                EditorGUILayout.LabelField("This content is inside a box");
                EditorGUILayout.LabelField("With automatic padding");
            });
            
            EditorGUILayout.Space();
            
            Horizontal(() =>
            {
                EditorGUILayout.LabelField("Horizontal group:");

                GUILayout.FlexibleSpace();
                GUILayout.Button("Button 1");
                GUILayout.Button("Button 2");
            });
        });
        
        Foldout("separators", "Separator Examples", () =>
        {
            EditorGUILayout.LabelField("Default separator:");
            Separator();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Thick separator:");
            Separator(3f);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Short separator (50% width):");
            Separator(2f, 100f);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom colored separator:");
            Separator(2f, null, Color.red);
        });
        
        Foldout("layout_options", "Layout Options Examples", () =>
        {
            EditorGUILayout.LabelField("Different button sizes:");
            
            Horizontal(() =>
            {
                if (Button("Small", null, null, EditorStyle.Default, LayoutOption.Width60)) { }
                if (Button("Medium", null, null, EditorStyle.Default, LayoutOption.Width100)) { }
                if (Button("Large", null, null, EditorStyle.Default, LayoutOption.Width150)) { }
            });
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fixed height buttons:");
            
            Horizontal(() =>
            {
                if (Button("Tall", null, null, EditorStyle.Default, LayoutOption.Height30)) { }
                if (Button("Short", null, null, EditorStyle.Default, LayoutOption.Height18)) { }
            });
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Min/Max width constraints:");
            
            Horizontal(() =>
            {
                if (Button("Min 100px", null, null, EditorStyle.Default, LayoutOption.MinWidth100)) { }
                if (Button("Max 200px", null, null, EditorStyle.Default, LayoutOption.MaxWidth200)) { }
            });
        });
        
        Foldout("editor_styles", "Editor Style Examples", () =>
        {
            EditorGUILayout.LabelField("Different button styles:");
            
            Horizontal(() =>
            {
                if (Button("Default", null, null, EditorStyle.Default, LayoutOption.Width80)) { }
                if (Button("Button", null, null, EditorStyle.Button, LayoutOption.Width80)) { }
                if (Button("Mini", null, null, EditorStyle.MiniButton, LayoutOption.Width80)) { }
            });
            
            EditorGUILayout.Space();
            
            Horizontal(() =>
            {
                if (Button("Toolbar", null, null, EditorStyle.ToolbarButton, LayoutOption.Width80)) { }
                if (Button("Popup", null, null, EditorStyle.Popup, LayoutOption.Width80)) { }
                if (Button("Dropdown", null, null, EditorStyle.Popup, LayoutOption.Width80)) { }
            });
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Toggle button with different styles:");
            
            Horizontal(() =>
            {
                demoToggle = ToggleButton("Default Toggle", demoToggle, EditorStyle.Default, LayoutOption.Width100);
                demoToggle = ToggleButton("Button Toggle", demoToggle, EditorStyle.Button, LayoutOption.Width100);
            });
        });
    }

    private void DrawVisualsTab(int tabIndex)
    {
        Header("Visual Elements");
        
        Foldout("progress", "Progress Bar", () =>
        {
            ProgressBar(progressValue, "Loading Assets", true);
            Horizontal(() =>
            {
                EditorGUILayout.LabelField("Progress:", GUILayout.Width(60));
                progressValue = EditorGUILayout.Slider(progressValue, 0f, 1f);
            });
        });

        Foldout("preview", "Asset Preview", () =>
        {
            if (demoTexture == null)
            {
                EditorGUILayout.LabelField("No texture selected");
                if (GUILayout.Button("Load Default Texture"))
                {
                    demoTexture = Resources.Load<Texture2D>("unity_builtin_extra/Default-Checker");
                }
            }
            else
            {
                if (AssetPreview(demoTexture, 64f, "Demo Texture"))
                {
                    EditorGUIUtility.PingObject(demoTexture);
                }
                
                Horizontal(() =>
                {
                    EditorGUILayout.LabelField("Texture:", GUILayout.Width(60));
                    demoTexture = ObjectField<Texture2D>("", demoTexture, false);
                });
            }
        });

        Foldout("messages", "Help Messages", () =>
        {
            HelpBox("This is an info message", MessageType.Info);
            HelpBox("This is a warning message", MessageType.Warning);
            HelpBox("This is an error message", MessageType.Error);
        });
    }

    private void DrawAboutTab(int tabIndex)
    {
        CenteredLabel("KEditorWindow Demo", true, 16);
        EditorGUILayout.Space();
        
        HelpBox("This window demonstrates all the new IMGUI helper methods available in KEditorWindow<T>.", MessageType.Info);
        
        EditorGUILayout.Space();
        
        Foldout("features", "Available Features", () =>
        {
            EditorGUILayout.LabelField("✓ Persistent search with toolbar integration");
            EditorGUILayout.LabelField("✓ Tab groups with state persistence");
            EditorGUILayout.LabelField("✓ Foldout groups with state persistence");
            EditorGUILayout.LabelField("✓ Progress bars with labels and percentages");
            EditorGUILayout.LabelField("✓ Asset preview thumbnails");
            EditorGUILayout.LabelField("✓ Enhanced color picker with presets");
            EditorGUILayout.LabelField("✓ Two-column layout helper");
            EditorGUILayout.LabelField("✓ Icon buttons with Unity icons");
            EditorGUILayout.LabelField("✓ Selection grids with auto-wrapping");
            EditorGUILayout.LabelField("✓ Type-safe object fields");
            EditorGUILayout.LabelField("✓ Min-max sliders with number inputs");
            EditorGUILayout.LabelField("✓ Conditional and toggle buttons");
            EditorGUILayout.LabelField("✓ Vector2/Vector3 fields");
            EditorGUILayout.LabelField("✓ Enum popups with type safety");
            EditorGUILayout.LabelField("✓ Layer mask fields");
            EditorGUILayout.LabelField("✓ Texture display with aspect ratio");
            EditorGUILayout.LabelField("✓ Help boxes with different message types");
            EditorGUILayout.LabelField("✓ Drag-and-drop handling");
            EditorGUILayout.LabelField("✓ Change detection utilities");
        });
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Open Project Window"))
        {
            EditorApplication.ExecuteMenuItem("Window/General/Project");
        }
    }

    private void DrawThemeTab(int tabIndex)
    {
        Header("Theme Settings");
        Box(() =>
        {
            EditorGUILayout.LabelField("Adjust colors used by KEditorWindow helpers.");
        });

        Foldout("theme_colors", "Colors", () =>
        {
            var theme = (typeof(KEditorWindow<ExampleWindow>)
                .GetField("_theme", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.GetValue(this));

            // Access via protected CurrentTheme property through reflection as well
            var themeProp = typeof(KEditorWindow<ExampleWindow>)
                .GetProperty("CurrentTheme", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            var currentTheme = themeProp?.GetValue(this);
            if (currentTheme == null) return;

            // Helper to get/set fields via reflection
            System.Func<string, Color> get = (name) =>
            {
                var fi = currentTheme.GetType().GetField(name);
                return (Color)fi.GetValue(currentTheme);
            };
            System.Action<string, Color> set = (name, val) =>
            {
                var fi = currentTheme.GetType().GetField(name);
                fi.SetValue(currentTheme, val);
            };

            Color bg = get("BackgroundColor");
            Color ac = get("AccentColor");
            Color sep = get("SeparatorColor");
            Color foot = get("FooterBorderColor");
            Color tog = get("ToggleActiveColor");

            bg = ColorPicker("Background", bg);
            ac = ColorPicker("Accent", ac);
            sep = ColorPicker("Separator", sep);
            foot = ColorPicker("Footer Border", foot);
            tog = ColorPicker("Toggle Active", tog);

            set("BackgroundColor", bg);
            set("AccentColor", ac);
            set("SeparatorColor", sep);
            set("FooterBorderColor", foot);
            set("ToggleActiveColor", tog);

            // Call SaveTheme via reflection (protected method)
            var saveThemeMi = typeof(KEditorWindow<ExampleWindow>).GetMethod("SaveTheme", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            saveThemeMi?.Invoke(this, null);
        });

        EditorGUILayout.Space();
        Footer("Theme changes are saved per-window type.");
    }
}
#endif