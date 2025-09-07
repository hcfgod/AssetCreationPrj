using UnityEngine;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Shows a scene dropdown instead of a string field. Stores either scene name or path.
    /// </summary>
    public class SceneReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// If true, writes the scene path (e.g., "Assets/Scenes/Main.unity"). If false, writes the scene name (e.g., "Main").
        /// </summary>
        public bool SaveAsPath { get; }

        /// <summary>
        /// If true, only lists scenes from Build Settings. If false, lists all scenes in the project.
        /// </summary>
        public bool OnlyBuildScenes { get; }

        public SceneReferenceAttribute(bool saveAsPath = false, bool onlyBuildScenes = true)
        {
            SaveAsPath = saveAsPath;
            OnlyBuildScenes = onlyBuildScenes;
        }
    }
}
