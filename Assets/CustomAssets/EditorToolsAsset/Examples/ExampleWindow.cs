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
    private Object demoAsset;
    private float progressValue = 0.3f;
    private int selectedGridItem = 0;
    private string[] gridOptions = { "Option 1", "Option 2", "Option 3", "Option 4" };

    [MenuItem("Tools/Example Window")]
    public static void Open()
    {
        ShowWindow("Example Window");
    }

    protected override void DrawWindowContents()
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
                if (Button("Normal Button"))
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
                Button("Button 1");
                Button("Button 2");
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
                if (Button("Small", null, null, EditorStyle.Button, LayoutOption.Width60)) { }
                if (Button("Medium", null, null, EditorStyle.Button, LayoutOption.Width100)) { }
                if (Button("Large", null, null, EditorStyle.Button, LayoutOption.Width150)) { }
            });
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fixed height buttons:");
            
            Horizontal(() =>
            {
                if (Button("Tall", null, null, EditorStyle.Button, LayoutOption.Height30)) { }
                if (Button("Short", null, null, EditorStyle.Button, LayoutOption.Height18)) { }
            });
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Min/Max width constraints:");
            
            Horizontal(() =>
            {
                if (Button("Min 100px", null, null, EditorStyle.Button, LayoutOption.MinWidth100)) { }
                if (Button("Max 200px", null, null, EditorStyle.Button, LayoutOption.MaxWidth200)) { }
            });
        });
        
        Foldout("editor_styles", "Editor Style Examples", () =>
        {
            EditorGUILayout.LabelField("Different button styles:");
            
            Horizontal(() =>
            {
                if (Button("Button", null, null, EditorStyle.Button, LayoutOption.Width80)) { }
                if (Button("Mini", null, null, EditorStyle.MiniButton, LayoutOption.Width80)) { }
            });
            
            EditorGUILayout.Space();
            
            Horizontal(() =>
            {
                if (Button("Toolbar", null, null, EditorStyle.ToolbarButton, LayoutOption.Width80)) { }

                DropdownButton("Popup", menu =>
                {
                    menu.AddItem(new GUIContent("Action/Do Thing"), false, () => Debug.Log("Do Thing"));
                    menu.AddItem(new GUIContent("Action/Do Other"), false, () => Debug.Log("Do Other"));
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Settings"), false, () => Debug.Log("Settings"));
                }, EditorStyle.Popup, LayoutOption.Width80);

                DropdownButton("Dropdown", menu =>
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        int captured = i;
                        menu.AddItem(new GUIContent($"Item {captured}"), false, () => Debug.Log($"Item {captured}"));
                    }
                }, EditorStyle.Popup, LayoutOption.Width80);
            });
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Toggle button with different styles:");
            
            Horizontal(() =>
            {
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
                EditorGUILayout.LabelField("Progress:", GUILayout.Width(65));
                progressValue = EditorGUILayout.Slider(progressValue, 0f, 1f);
            });
        });

        Foldout("preview", "Asset Preview", () =>
        {
            TwoColumns(
                leftColumn: () =>
                {
                    EditorGUILayout.LabelField("Asset:");
                    demoAsset = EditorGUILayout.ObjectField(demoAsset, typeof(Object), false);

                    EditorGUILayout.Space();
                    if (demoAsset != null)
                    {
                        EditorGUILayout.LabelField($"Name: {demoAsset.name}", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField($"Type: {demoAsset.GetType().Name}", EditorStyles.miniLabel);
                        Horizontal(() =>
                        {
                            if (GUILayout.Button("Ping", GUILayout.Width(60))) EditorGUIUtility.PingObject(demoAsset);
                            if (GUILayout.Button("Clear", GUILayout.Width(60))) demoAsset = null;
                        });
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Drag any asset here", EditorStyles.miniLabel);
                    }
                },
                rightColumn: () =>
                {
                    if (demoAsset != null)
                    {
                        if (AssetPreview(demoAsset, 96f, demoAsset.name))
                        {
                            EditorGUIUtility.PingObject(demoAsset);
                        }
                    }
                },
                leftWidth: 260f
            );
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

        var theme = EditorThemeManager.GetEditableThemeForWindow(typeof(ExampleWindow));

        Foldout("theme_colors", "Per-Window Colors", () =>
        {
            theme.AccentColor       = ColorPicker("Accent", theme.AccentColor);
            theme.SeparatorColor    = ColorPicker("Separator", theme.SeparatorColor);
            theme.ToggleActiveColor = ColorPicker("Toggle Active", theme.ToggleActiveColor);

            EditorGUILayout.Space();
            Header("Extended Colors", EditorStyle.BoldLabel);
            theme.TextColor            = ColorPicker("Text", theme.TextColor);
            theme.SubTextColor         = ColorPicker("Sub Text", theme.SubTextColor);
            theme.HeaderTextColor      = ColorPicker("Header Text", theme.HeaderTextColor);
            theme.PanelBackgroundColor = ColorPicker("Panel Background", theme.PanelBackgroundColor);
            theme.PanelBorderColor     = ColorPicker("Panel Border", theme.PanelBorderColor);

            EditorGUILayout.Space();
            Header("Button Colors", EditorStyle.BoldLabel);
            theme.ButtonNormalColor = ColorPicker("Button Background", theme.ButtonNormalColor);
            theme.ButtonTextColor   = ColorPicker("Button Text", theme.ButtonTextColor);
            theme.ButtonBorderColor = ColorPicker("Button Border", theme.ButtonBorderColor);

            EditorGUILayout.Space();
            Header("Progress Bar Colors", EditorStyle.BoldLabel);
            theme.ProgressBackgroundColor = ColorPicker("Progress Background", theme.ProgressBackgroundColor);
            theme.ProgressFillColor       = ColorPicker("Progress Fill", theme.ProgressFillColor);
            theme.ProgressTextColor       = ColorPicker("Progress Text", theme.ProgressTextColor);

            EditorGUILayout.Space();
            Horizontal(() =>
            {
                if (Button("Apply Light Defaults", null, null, EditorStyle.MiniButton, LayoutOption.Width150))
                {
                    var def = EditorTheme.DefaultLight();
                    theme.AccentColor       = def.AccentColor;
                    theme.SeparatorColor    = def.SeparatorColor;
                    theme.ToggleActiveColor = def.ToggleActiveColor;
                }
                if (Button("Apply Dark Defaults", null, null, EditorStyle.MiniButton, LayoutOption.Width150))
                {
                    var def = EditorTheme.DefaultDark();
                    theme.AccentColor       = def.AccentColor;
                    theme.SeparatorColor    = def.SeparatorColor;
                    theme.ToggleActiveColor = def.ToggleActiveColor;
                }
            });

            EditorGUILayout.Space();
            Horizontal(() =>
            {
                if (AccentButton("Save Window Theme", EditorStyle.Button, LayoutOption.Width150))
                {
                    EditorThemeManager.SaveThemeForWindow(typeof(ExampleWindow), theme);
                }
                if (ColoredButton("Reset To Global", Color.red, EditorStyle.Button, LayoutOption.Width150))
                {
                    EditorThemeManager.ResetThemeForWindow(typeof(ExampleWindow));
                }
                if (Button("Open Global Theme Preferences", null, null, EditorStyle.Button, LayoutOption.Width200))
                {
                    SettingsService.OpenUserPreferences("Preferences/KEditor/Theme");
                }
            });
        });

        EditorGUILayout.Space();
        Footer("Theme changes can be saved per-window or globally in Preferences.");
    }
}
#endif