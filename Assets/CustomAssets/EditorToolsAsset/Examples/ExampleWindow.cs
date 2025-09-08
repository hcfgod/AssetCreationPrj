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
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {
                    Repaint();
                }
            },
            drawRight: () =>
            {
                if (GUILayout.Button("Help", EditorStyles.toolbarButton))
                {
                    EditorUtility.DisplayDialog("Help", "This showcases all the new KEditorWindow IMGUI helpers!", "OK");
                }
            },
            includeSearch: true,
            searchPlaceholder: "Filter items..."
        );

        // Main content using tabs
        TabGroup("main", new[] { "Controls", "Layout", "Visuals", "About" }, new System.Action<int>[]
        {
            DrawControlsTab,
            DrawLayoutTab,
            DrawVisualsTab,
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
}
#endif