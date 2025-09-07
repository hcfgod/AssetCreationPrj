using UnityEngine;

namespace CustomAssets.EditorTools.Examples
{
    public class SceneReferenceExample : MonoBehaviour
    {
        [Title("By Name (Build Scenes Only)")]
        [SceneReference] public string mainMenuScene;

        [Title("By Path (Build Scenes Only)")]
        [SceneReference(saveAsPath: true)] public string gameplayScenePath;

        [Title("By Name (All Project Scenes)")]
        [SceneReference(saveAsPath: false, onlyBuildScenes: false)] public string anySceneName;

        [Title("SceneAsset (Object Reference)")]
        #if UNITY_EDITOR
        public UnityEditor.SceneAsset mainMenuSceneAsset;
        #endif
    }
}