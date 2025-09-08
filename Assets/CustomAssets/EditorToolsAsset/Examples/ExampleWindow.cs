#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using CustomAssets.EditorTools;

public class ExampleWindow : KEditorWindow<ExampleWindow>
{
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
                    EditorUtility.DisplayDialog("Help", "This is an example editor window using KEditorWindow helpers.", "OK");
                }
            },
            includeSearch: true,
            searchPlaceholder: "Filter items..."
        );

        Header("Contents");
        Separator();

        ScrollView(() =>
        {
            for (int i = 0; i < 50; i++)
            {
                string label = $"Item {i}";
                if (!string.IsNullOrEmpty(SearchQuery) && !label.ToLowerInvariant().Contains(SearchQuery.ToLowerInvariant()))
                    continue;
                if (GUILayout.Button(label))
                {
                    Debug.Log($"Clicked {label}");
                }
            }
        });

        Footer($"Search: '{SearchQuery}'", System.DateTime.Now.ToShortTimeString());
    }
}
#endif