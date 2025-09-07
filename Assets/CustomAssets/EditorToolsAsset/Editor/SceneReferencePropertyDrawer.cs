using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomAssets.EditorTools.Editor
{
    [CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
    public class SceneReferencePropertyDrawer : PropertyDrawer
    {
        private class SceneInfo
        {
            public string name;
            public string path;
            public bool enabledInBuild;
        }

        private static double _lastRefreshTime;
        private static List<SceneInfo> _cachedScenes;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (SceneReferenceAttribute)attribute;
            EnsureSceneCache(attr);

            // Build display names
            List<string> options = new List<string>();
            options.Add("None");
            foreach (var s in _cachedScenes)
            {
                string disp = attr.SaveAsPath ? s.path : s.name;
                if (!s.enabledInBuild && attr.OnlyBuildScenes)
                {
                    // Will not happen due to filter, but keep for completeness
                    continue;
                }
                options.Add(disp);
            }

            if (property.propertyType == SerializedPropertyType.String)
            {
                // Determine current selection for string value
                int currentIndex = 0; // None
                string current = property.stringValue ?? string.Empty;
                if (!string.IsNullOrEmpty(current))
                {
                    for (int i = 0; i < _cachedScenes.Count; i++)
                    {
                        var s = _cachedScenes[i];
                        if (attr.SaveAsPath)
                        {
                            if (string.Equals(current, s.path, System.StringComparison.OrdinalIgnoreCase))
                            {
                                currentIndex = i + 1; // +1 due to "None"
                                break;
                            }
                        }
                        else
                        {
                            if (string.Equals(current, s.name, System.StringComparison.Ordinal))
                            {
                                currentIndex = i + 1;
                                break;
                            }
                        }
                    }
                }

                int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());
                if (newIndex != currentIndex)
                {
                    if (newIndex <= 0)
                    {
                        property.stringValue = string.Empty;
                    }
                    else
                    {
                        var selected = _cachedScenes[newIndex - 1];
                        property.stringValue = attr.SaveAsPath ? selected.path : selected.name;
                    }
                }
                return;
            }
            else if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                // Support SceneAsset object reference fields
                var fieldType = fieldInfo != null ? fieldInfo.FieldType : null;
                if (fieldType == null || (fieldType != typeof(SceneAsset) && !typeof(SceneAsset).IsAssignableFrom(fieldType)))
                {
                    EditorGUI.PropertyField(position, property, label, true);
                    return;
                }

                // Determine current selection for SceneAsset
                int currentIndex = 0; // None
                string currentPath = null;
                if (property.objectReferenceValue != null)
                {
                    currentPath = AssetDatabase.GetAssetPath(property.objectReferenceValue);
                    for (int i = 0; i < _cachedScenes.Count; i++)
                    {
                        if (string.Equals(_cachedScenes[i].path, currentPath, System.StringComparison.OrdinalIgnoreCase))
                        {
                            currentIndex = i + 1;
                            break;
                        }
                    }
                }

                int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());
                if (newIndex != currentIndex)
                {
                    if (newIndex <= 0)
                    {
                        property.objectReferenceValue = null;
                    }
                    else
                    {
                        var selected = _cachedScenes[newIndex - 1];
                        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(selected.path);
                        property.objectReferenceValue = sceneAsset;
                    }
                }
                return;
            }
            else
            {
                // Fallback: draw default for unsupported types
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
        }

        private static void EnsureSceneCache(SceneReferenceAttribute attr)
        {
            // Refresh cache every few seconds to pick up project changes
            if (_cachedScenes != null && EditorApplication.timeSinceStartup - _lastRefreshTime < 5.0f)
                return;

            _lastRefreshTime = EditorApplication.timeSinceStartup;
            _cachedScenes = new List<SceneInfo>();

            if (attr.OnlyBuildScenes)
            {
                foreach (var s in EditorBuildSettings.scenes)
                {
                    string path = s.path;
                    string name = Path.GetFileNameWithoutExtension(path);
                    _cachedScenes.Add(new SceneInfo { name = name, path = path, enabledInBuild = s.enabled });
                }
            }
            else
            {
                string[] guids = AssetDatabase.FindAssets("t:Scene");
                HashSet<string> buildSet = new HashSet<string>();
                foreach (var bs in EditorBuildSettings.scenes)
                {
                    buildSet.Add(bs.path);
                }
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    string name = Path.GetFileNameWithoutExtension(path);
                    bool inBuild = buildSet.Contains(path);
                    _cachedScenes.Add(new SceneInfo { name = name, path = path, enabledInBuild = inBuild });
                }

                // Sort by name for convenience
                _cachedScenes.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
