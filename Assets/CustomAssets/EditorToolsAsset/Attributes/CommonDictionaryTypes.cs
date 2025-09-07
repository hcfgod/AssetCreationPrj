using UnityEngine;
using System;

namespace CustomAssets.EditorTools
{
    /// <summary>
    /// Common serializable dictionary types for Unity development.
    /// These provide convenient aliases for frequently used dictionary combinations.
    /// </summary>
    
    // ===== STRING-BASED DICTIONARIES =====
    
    [System.Serializable]
    public class StringStringDictionary : SerializableDictionary<string, string> { }

    [System.Serializable]
    public class StringIntDictionary : SerializableDictionary<string, int> { }

    [System.Serializable]
    public class StringFloatDictionary : SerializableDictionary<string, float> { }

    [System.Serializable]
    public class StringBoolDictionary : SerializableDictionary<string, bool> { }

    [System.Serializable]
    public class StringDoubleDictionary : SerializableDictionary<string, double> { }

    [System.Serializable]
    public class StringLongDictionary : SerializableDictionary<string, long> { }

    [System.Serializable]
    public class StringShortDictionary : SerializableDictionary<string, short> { }

    [System.Serializable]
    public class StringByteDictionary : SerializableDictionary<string, byte> { }

    [System.Serializable]
    public class StringCharDictionary : SerializableDictionary<string, char> { }

    [System.Serializable]
    public class StringDecimalDictionary : SerializableDictionary<string, decimal> { }

    // ===== INTEGER-BASED DICTIONARIES =====
    
    [System.Serializable]
    public class IntStringDictionary : SerializableDictionary<int, string> { }

    [System.Serializable]
    public class IntIntDictionary : SerializableDictionary<int, int> { }

    [System.Serializable]
    public class IntFloatDictionary : SerializableDictionary<int, float> { }

    [System.Serializable]
    public class IntBoolDictionary : SerializableDictionary<int, bool> { }

    [System.Serializable]
    public class IntDoubleDictionary : SerializableDictionary<int, double> { }

    [System.Serializable]
    public class IntLongDictionary : SerializableDictionary<int, long> { }

    [System.Serializable]
    public class IntShortDictionary : SerializableDictionary<int, short> { }

    [System.Serializable]
    public class IntByteDictionary : SerializableDictionary<int, byte> { }

    [System.Serializable]
    public class IntCharDictionary : SerializableDictionary<int, char> { }

    [System.Serializable]
    public class IntDecimalDictionary : SerializableDictionary<int, decimal> { }

    // ===== FLOAT-BASED DICTIONARIES =====
    
    [System.Serializable]
    public class FloatStringDictionary : SerializableDictionary<float, string> { }

    [System.Serializable]
    public class FloatIntDictionary : SerializableDictionary<float, int> { }

    [System.Serializable]
    public class FloatFloatDictionary : SerializableDictionary<float, float> { }

    [System.Serializable]
    public class FloatBoolDictionary : SerializableDictionary<float, bool> { }

    // ===== UNITY MATH TYPES =====
    
    [System.Serializable]
    public class StringVector3Dictionary : SerializableDictionary<string, Vector3> { }

    [System.Serializable]
    public class StringVector2Dictionary : SerializableDictionary<string, Vector2> { }

    [System.Serializable]
    public class StringVector4Dictionary : SerializableDictionary<string, Vector4> { }

    [System.Serializable]
    public class StringQuaternionDictionary : SerializableDictionary<string, Quaternion> { }

    [System.Serializable]
    public class StringColorDictionary : SerializableDictionary<string, Color> { }

    [System.Serializable]
    public class StringColor32Dictionary : SerializableDictionary<string, Color32> { }

    [System.Serializable]
    public class StringRectDictionary : SerializableDictionary<string, Rect> { }

    [System.Serializable]
    public class StringRectIntDictionary : SerializableDictionary<string, RectInt> { }

    [System.Serializable]
    public class StringBoundsDictionary : SerializableDictionary<string, Bounds> { }

    [System.Serializable]
    public class StringBoundsIntDictionary : SerializableDictionary<string, BoundsInt> { }

    [System.Serializable]
    public class StringMatrix4x4Dictionary : SerializableDictionary<string, Matrix4x4> { }

    [System.Serializable]
    public class StringPlaneDictionary : SerializableDictionary<string, Plane> { }

    [System.Serializable]
    public class StringRayDictionary : SerializableDictionary<string, Ray> { }

    [System.Serializable]
    public class StringRay2DDictionary : SerializableDictionary<string, Ray2D> { }

    // ===== UNITY OBJECT TYPES =====
    
    [System.Serializable]
    public class StringGameObjectDictionary : SerializableDictionary<string, GameObject> { }

    [System.Serializable]
    public class StringTransformDictionary : SerializableDictionary<string, Transform> { }

    [System.Serializable]
    public class StringComponentDictionary : SerializableDictionary<string, Component> { }

    [System.Serializable]
    public class StringMonoBehaviourDictionary : SerializableDictionary<string, MonoBehaviour> { }

    [System.Serializable]
    public class StringScriptableObjectDictionary : SerializableDictionary<string, ScriptableObject> { }

    // ===== UNITY ASSET TYPES =====
    
    [System.Serializable]
    public class StringSpriteDictionary : SerializableDictionary<string, Sprite> { }

    [System.Serializable]
    public class StringTextureDictionary : SerializableDictionary<string, Texture> { }

    [System.Serializable]
    public class StringTexture2DDictionary : SerializableDictionary<string, Texture2D> { }

    [System.Serializable]
    public class StringTexture3DDictionary : SerializableDictionary<string, Texture3D> { }

    [System.Serializable]
    public class StringCubemapDictionary : SerializableDictionary<string, Cubemap> { }

    [System.Serializable]
    public class StringRenderTextureDictionary : SerializableDictionary<string, RenderTexture> { }

    [System.Serializable]
    public class StringMaterialDictionary : SerializableDictionary<string, Material> { }

    [System.Serializable]
    public class StringMeshDictionary : SerializableDictionary<string, Mesh> { }

    [System.Serializable]
    public class StringFontDictionary : SerializableDictionary<string, Font> { }

    [System.Serializable]
    public class StringAudioClipDictionary : SerializableDictionary<string, AudioClip> { }

    [System.Serializable]
    public class StringAnimationClipDictionary : SerializableDictionary<string, AnimationClip> { }

    [System.Serializable]
    public class StringRuntimeAnimatorControllerDictionary : SerializableDictionary<string, RuntimeAnimatorController> { }

    [System.Serializable]
    public class StringPhysicMaterialDictionary : SerializableDictionary<string, PhysicsMaterial> { }

    [System.Serializable]
    public class StringPhysicsMaterial2DDictionary : SerializableDictionary<string, PhysicsMaterial2D> { }

    // ===== UNITY UI TYPES =====
    
    [System.Serializable]
    public class StringCanvasDictionary : SerializableDictionary<string, Canvas> { }

    [System.Serializable]
    public class StringCanvasGroupDictionary : SerializableDictionary<string, CanvasGroup> { }

    [System.Serializable]
    public class StringGraphicDictionary : SerializableDictionary<string, UnityEngine.UI.Graphic> { }

    [System.Serializable]
    public class StringImageDictionary : SerializableDictionary<string, UnityEngine.UI.Image> { }

    [System.Serializable]
    public class StringTextDictionary : SerializableDictionary<string, UnityEngine.UI.Text> { }

    [System.Serializable]
    public class StringButtonDictionary : SerializableDictionary<string, UnityEngine.UI.Button> { }

    [System.Serializable]
    public class StringToggleDictionary : SerializableDictionary<string, UnityEngine.UI.Toggle> { }

    [System.Serializable]
    public class StringSliderDictionary : SerializableDictionary<string, UnityEngine.UI.Slider> { }

    [System.Serializable]
    public class StringScrollbarDictionary : SerializableDictionary<string, UnityEngine.UI.Scrollbar> { }

    [System.Serializable]
    public class StringDropdownDictionary : SerializableDictionary<string, UnityEngine.UI.Dropdown> { }

    [System.Serializable]
    public class StringInputFieldDictionary : SerializableDictionary<string, UnityEngine.UI.InputField> { }

    [System.Serializable]
    public class StringScrollRectDictionary : SerializableDictionary<string, UnityEngine.UI.ScrollRect> { }

    [System.Serializable]
    public class StringLayoutElementDictionary : SerializableDictionary<string, UnityEngine.UI.LayoutElement> { }

    [System.Serializable]
    public class StringContentSizeFitterDictionary : SerializableDictionary<string, UnityEngine.UI.ContentSizeFitter> { }

    [System.Serializable]
    public class StringAspectRatioFitterDictionary : SerializableDictionary<string, UnityEngine.UI.AspectRatioFitter> { }

    // ===== UNITY PHYSICS TYPES =====
    
    [System.Serializable]
    public class StringRigidbodyDictionary : SerializableDictionary<string, Rigidbody> { }

    [System.Serializable]
    public class StringRigidbody2DDictionary : SerializableDictionary<string, Rigidbody2D> { }

    [System.Serializable]
    public class StringColliderDictionary : SerializableDictionary<string, Collider> { }

    [System.Serializable]
    public class StringCollider2DDictionary : SerializableDictionary<string, Collider2D> { }

    [System.Serializable]
    public class StringJointDictionary : SerializableDictionary<string, Joint> { }

    [System.Serializable]
    public class StringJoint2DDictionary : SerializableDictionary<string, Joint2D> { }

    [System.Serializable]
    public class StringCharacterControllerDictionary : SerializableDictionary<string, CharacterController> { }

    [System.Serializable]
    public class StringCapsuleColliderDictionary : SerializableDictionary<string, CapsuleCollider> { }

    [System.Serializable]
    public class StringBoxColliderDictionary : SerializableDictionary<string, BoxCollider> { }

    [System.Serializable]
    public class StringSphereColliderDictionary : SerializableDictionary<string, SphereCollider> { }

    [System.Serializable]
    public class StringMeshColliderDictionary : SerializableDictionary<string, MeshCollider> { }

    [System.Serializable]
    public class StringWheelColliderDictionary : SerializableDictionary<string, WheelCollider> { }

    [System.Serializable]
    public class StringTerrainColliderDictionary : SerializableDictionary<string, TerrainCollider> { }

    // ===== UNITY RENDERING TYPES =====
    
    [System.Serializable]
    public class StringCameraDictionary : SerializableDictionary<string, Camera> { }

    [System.Serializable]
    public class StringLightDictionary : SerializableDictionary<string, Light> { }

    [System.Serializable]
    public class StringRendererDictionary : SerializableDictionary<string, Renderer> { }

    [System.Serializable]
    public class StringMeshRendererDictionary : SerializableDictionary<string, MeshRenderer> { }

    [System.Serializable]
    public class StringSkinnedMeshRendererDictionary : SerializableDictionary<string, SkinnedMeshRenderer> { }

    [System.Serializable]
    public class StringParticleSystemDictionary : SerializableDictionary<string, ParticleSystem> { }

    [System.Serializable]
    public class StringTrailRendererDictionary : SerializableDictionary<string, TrailRenderer> { }

    [System.Serializable]
    public class StringLineRendererDictionary : SerializableDictionary<string, LineRenderer> { }

    [System.Serializable]
    public class StringCanvasRendererDictionary : SerializableDictionary<string, CanvasRenderer> { }

    // ===== UNITY ANIMATION TYPES =====
    
    [System.Serializable]
    public class StringAnimatorDictionary : SerializableDictionary<string, Animator> { }

    [System.Serializable]
    public class StringAnimationDictionary : SerializableDictionary<string, Animation> { }

    // Note: AnimatorStateMachine and AnimatorController are not serializable by default

    // ===== UNITY AUDIO TYPES =====
    
    [System.Serializable]
    public class StringAudioSourceDictionary : SerializableDictionary<string, AudioSource> { }

    [System.Serializable]
    public class StringAudioListenerDictionary : SerializableDictionary<string, AudioListener> { }

    [System.Serializable]
    public class StringAudioReverbZoneDictionary : SerializableDictionary<string, AudioReverbZone> { }

    [System.Serializable]
    public class StringAudioLowPassFilterDictionary : SerializableDictionary<string, AudioLowPassFilter> { }

    [System.Serializable]
    public class StringAudioHighPassFilterDictionary : SerializableDictionary<string, AudioHighPassFilter> { }

    [System.Serializable]
    public class StringAudioEchoFilterDictionary : SerializableDictionary<string, AudioEchoFilter> { }

    [System.Serializable]
    public class StringAudioDistortionFilterDictionary : SerializableDictionary<string, AudioDistortionFilter> { }

    [System.Serializable]
    public class StringAudioChorusFilterDictionary : SerializableDictionary<string, AudioChorusFilter> { }

    [System.Serializable]
    public class StringAudioReverbFilterDictionary : SerializableDictionary<string, AudioReverbFilter> { }

    // ===== UNITY INPUT TYPES =====
    
    [System.Serializable]
    public class StringKeyCodeDictionary : SerializableDictionary<string, KeyCode> { }

    [System.Serializable]
    public class StringTouchPhaseDictionary : SerializableDictionary<string, TouchPhase> { }

    // ===== UNITY SCENE MANAGEMENT TYPES =====
    
    [System.Serializable]
    public class StringSceneDictionary : SerializableDictionary<string, UnityEngine.SceneManagement.Scene> { }

    // ===== UNITY NETWORKING TYPES =====
    // Note: Unity Networking types are deprecated in newer Unity versions

    // ===== UNITY XR TYPES =====
    // Note: XRDevice is static and cannot be used as a generic type parameter

    // ===== CUSTOM COMMON TYPES =====
    
    /// <summary>
    /// Dictionary for storing enum values by string key.
    /// Use with any enum type: StringEnumDictionary&lt;MyEnum&gt;
    /// </summary>
    [System.Serializable]
    public class StringEnumDictionary<T> : SerializableDictionary<string, T> where T : Enum { }

    /// <summary>
    /// Dictionary for storing ScriptableObject instances by string key.
    /// Use with any ScriptableObject type: StringScriptableObjectDictionary&lt;MyScriptableObject&gt;
    /// </summary>
    [System.Serializable]
    public class StringScriptableObjectDictionary<T> : SerializableDictionary<string, T> where T : ScriptableObject { }

    /// <summary>
    /// Dictionary for storing MonoBehaviour instances by string key.
    /// Use with any MonoBehaviour type: StringMonoBehaviourDictionary&lt;MyMonoBehaviour&gt;
    /// </summary>
    [System.Serializable]
    public class StringMonoBehaviourDictionary<T> : SerializableDictionary<string, T> where T : MonoBehaviour { }

    /// <summary>
    /// Dictionary for storing Component instances by string key.
    /// Use with any Component type: StringComponentDictionary&lt;MyComponent&gt;
    /// </summary>
    [System.Serializable]
    public class StringComponentDictionary<T> : SerializableDictionary<string, T> where T : Component { }

    // ===== COMMON GAME DEVELOPMENT PATTERNS =====
    
    /// <summary>
    /// Dictionary for storing item data by item ID.
    /// Commonly used for inventory systems, item databases, etc.
    /// </summary>
    [System.Serializable]
    public class ItemDataDictionary : SerializableDictionary<string, ItemData> { }

    /// <summary>
    /// Dictionary for storing level data by level ID.
    /// Commonly used for level management systems.
    /// </summary>
    [System.Serializable]
    public class LevelDataDictionary : SerializableDictionary<int, LevelData> { }

    /// <summary>
    /// Dictionary for storing player stats by stat name.
    /// Commonly used for character progression systems.
    /// </summary>
    [System.Serializable]
    public class PlayerStatsDictionary : SerializableDictionary<string, PlayerStatData> { }

    /// <summary>
    /// Dictionary for storing dialogue by dialogue ID.
    /// Commonly used for dialogue systems.
    /// </summary>
    [System.Serializable]
    public class DialogueDictionary : SerializableDictionary<string, DialogueData> { }

    /// <summary>
    /// Dictionary for storing achievement data by achievement ID.
    /// Commonly used for achievement systems.
    /// </summary>
    [System.Serializable]
    public class AchievementDictionary : SerializableDictionary<string, AchievementData> { }

    /// <summary>
    /// Dictionary for storing quest data by quest ID.
    /// Commonly used for quest systems.
    /// </summary>
    [System.Serializable]
    public class QuestDictionary : SerializableDictionary<string, QuestData> { }

    /// <summary>
    /// Dictionary for storing skill data by skill ID.
    /// Commonly used for skill trees and progression systems.
    /// </summary>
    [System.Serializable]
    public class SkillDictionary : SerializableDictionary<string, SkillData> { }

    /// <summary>
    /// Dictionary for storing weapon data by weapon ID.
    /// Commonly used for weapon systems.
    /// </summary>
    [System.Serializable]
    public class WeaponDictionary : SerializableDictionary<string, WeaponData> { }

    /// <summary>
    /// Dictionary for storing enemy data by enemy ID.
    /// Commonly used for enemy management systems.
    /// </summary>
    [System.Serializable]
    public class EnemyDictionary : SerializableDictionary<string, EnemyData> { }

    /// <summary>
    /// Dictionary for storing building data by building ID.
    /// Commonly used for city builders and strategy games.
    /// </summary>
    [System.Serializable]
    public class BuildingDictionary : SerializableDictionary<string, BuildingData> { }

    // ===== EXAMPLE CUSTOM DATA CLASSES =====
    
    /// <summary>
    /// Example item data structure for game items.
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        public string name;
        public string description;
        public int value;
        public float weight;
        public Sprite icon;
        public bool stackable = true;
        public int maxStackSize = 99;
    }

    /// <summary>
    /// Example level data structure for game levels.
    /// </summary>
    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public string description;
        public int requiredLevel;
        public float difficulty;
        public Vector3 spawnPoint;
        public string sceneName;
    }

    /// <summary>
    /// Example player stat data structure.
    /// </summary>
    [System.Serializable]
    public class PlayerStatData
    {
        public int baseValue;
        public int currentValue;
        public int maxValue;
        public float multiplier = 1.0f;
        public string description;
    }

    /// <summary>
    /// Example dialogue data structure.
    /// </summary>
    [System.Serializable]
    public class DialogueData
    {
        public string speaker;
        public string text;
        public AudioClip voiceClip;
        public float displayTime = 3.0f;
        public string[] choices;
    }

    /// <summary>
    /// Example achievement data structure.
    /// </summary>
    [System.Serializable]
    public class AchievementData
    {
        public string title;
        public string description;
        public Sprite icon;
        public int points;
        public bool isHidden;
        public string[] requirements;
    }

    /// <summary>
    /// Example quest data structure.
    /// </summary>
    [System.Serializable]
    public class QuestData
    {
        public string title;
        public string description;
        public string[] objectives;
        public int experienceReward;
        public string[] itemRewards;
        public bool isRepeatable;
    }

    /// <summary>
    /// Example skill data structure.
    /// </summary>
    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public string description;
        public int level;
        public int maxLevel;
        public float experience;
        public string[] prerequisites;
    }

    /// <summary>
    /// Example weapon data structure.
    /// </summary>
    [System.Serializable]
    public class WeaponData
    {
        public string weaponName;
        public int damage;
        public float range;
        public float fireRate;
        public int ammoCapacity;
        public Sprite weaponIcon;
    }

    /// <summary>
    /// Example enemy data structure.
    /// </summary>
    [System.Serializable]
    public class EnemyData
    {
        public string enemyName;
        public int health;
        public int damage;
        public float speed;
        public int experienceReward;
        public string[] lootTable;
    }

    /// <summary>
    /// Example building data structure.
    /// </summary>
    [System.Serializable]
    public class BuildingData
    {
        public string buildingName;
        public string description;
        public int cost;
        public float buildTime;
        public Vector2 size;
        public string[] requirements;
    }
}
