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
        GUILayout.Label("Hello from My Tool!", EditorStyles.boldLabel);

        if (GUILayout.Button("Do Something"))
        {
            Debug.Log("Button pressed!");
        }
    }
}