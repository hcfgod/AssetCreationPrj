using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class SceneReferenceExample : MonoBehaviour
    {
        [Header("By Name (Build Scenes Only)")]
        [SceneReference] public string mainMenuScene;

        [Header("By Path (Build Scenes Only)")]
        [SceneReference(saveAsPath: true)] public string gameplayScenePath;

        [Header("By Name (All Project Scenes)")]
        [SceneReference(saveAsPath: false, onlyBuildScenes: false)] public string anySceneName;
    }
}
