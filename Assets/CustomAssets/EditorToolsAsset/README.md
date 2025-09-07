# Editor Tools Asset

A collection of Unity editor tools that enhance the Inspector experience and provide reusable functionality for game development.

## Tools Included

### 1. ReadOnly Attribute
A Unity editor tool that provides a `[ReadOnly]` attribute for making fields read-only in the Unity Inspector.

### 2. Serializable Dictionary
A comprehensive serializable dictionary system with Inspector integration for editing key-value pairs directly in Unity.

### 3. TabGroup Attribute
Group fields into toolbar-style tabs similar to Odin Inspector's TabGroup, with clean buttons (no foldout arrows).

### 4. Button Attribute
Create custom buttons in the Inspector that execute methods directly, with extensive customization options.

### 5. ShowIf/HideIf Attributes
Conditionally show or hide fields in the Inspector based on other field values, with support for various data types and conditions.

### 6. Title Attribute
Create colored titles and headers in the Inspector with custom styling, font sizes, and optional separators.

### 7. Min Max Slider Attribute
A custom attribute that allows you to display and edit a float or int value as a min-max slider in the Unity Inspector. This attribute lets you define a range (minimum and maximum limits) and exposes two fields (min and max) as a draggable slider, making it easy to select value ranges for things like spawn areas, stat ranges, or configurable thresholds. The MinMaxSlider attribute supports both float and int types, and provides a clear, user-friendly interface for range selection directly in the Inspector.

---

## ReadOnly Attribute

### Features

- **Read-Only Fields**: Fields marked with `[ReadOnly]` appear grayed out in the Inspector
- **Tooltip Support**: Optional tooltip parameter for additional information
- **Programmatic Updates**: Fields can still be modified through code
- **Multiple Data Types**: Works with all Unity serializable types

### Usage

#### Basic Usage

```csharp
using CustomAssets.EditorTools;

public class PlayerController : MonoBehaviour
{
    [ReadOnly]
    public float health = 100f;
    
    [ReadOnly]
    public string playerName = "Player";
}
```

#### With Tooltip

```csharp
[ReadOnly("This field is calculated automatically")]
public int score = 0;

[ReadOnly("This value represents the player's spawn position")]
public Vector3 spawnPosition = Vector3.zero;
```

#### Programmatic Updates

```csharp
public class GameManager : MonoBehaviour
{
    [ReadOnly("Current game time")]
    public float gameTime = 0f;
    
    void Update()
    {
        // You can still modify ReadOnly fields in code
        gameTime += Time.deltaTime;
    }
}
```

---

## Serializable Dictionary

### Features

- **Generic Support**: `SerializableDictionary<TKey, TValue>` for any serializable types
- **Inspector Integration**: Custom property drawer for easy editing
- **Common Types**: Pre-defined dictionary types for frequent use cases
- **Validation**: Duplicate key detection and error handling
- **Performance**: Optimized for Unity serialization system

### Usage

#### Basic Usage

```csharp
using CustomAssets.EditorTools;

public class GameConfig : MonoBehaviour
{
    // Option A: Generic usage (works at runtime and in Inspector via [SerializeReference])
    [SerializeReference]
    public SerializableDictionary<string, string> itemNames = new SerializableDictionary<string, string>();

    // Option B: Concrete wrapper types (no [SerializeReference] needed)
    public StringIntDictionary itemPrices = new StringIntDictionary();
}
```

#### Generic Usage (Inspector + Runtime)

```csharp
using CustomAssets.EditorTools;

public class Localization : MonoBehaviour
{
    // Managed reference allows Unity to serialize an open generic instance
    [SerializeReference]
    public SerializableDictionary<string, string> localizedTexts = new SerializableDictionary<string, string>();
}
```

Notes:
- Add the attribute `[SerializeReference]` to expose a generic `SerializableDictionary<TKey, TValue>` in the Inspector.
- Our drawer will instantiate the managed reference automatically if it's null.
- For AOT/platforms without managed reference support, prefer concrete wrappers.

#### Common Dictionary Types

```csharp
// String-based dictionaries
public StringStringDictionary translations = new StringStringDictionary();
public StringIntDictionary itemPrices = new StringIntDictionary();
public StringFloatDictionary itemWeights = new StringFloatDictionary();
public StringBoolDictionary itemFlags = new StringBoolDictionary();

// Integer-based dictionaries
public IntStringDictionary levelNames = new IntStringDictionary();
public IntIntDictionary levelScores = new IntIntDictionary();
public IntFloatDictionary levelDifficulty = new IntFloatDictionary();

// Unity object dictionaries
public StringGameObjectDictionary prefabs = new StringGameObjectDictionary();
public StringSpriteDictionary icons = new StringSpriteDictionary();
public StringAudioClipDictionary sounds = new StringAudioClipDictionary();
```

#### Game Configuration Example

```csharp
public class GameSettings : MonoBehaviour
{
    [Header("Item Configuration")]
    public StringStringDictionary itemNames = new StringStringDictionary();
    public StringIntDictionary itemPrices = new StringIntDictionary();
    public StringFloatDictionary itemWeights = new StringFloatDictionary();
    
    [Header("Level Configuration")]
    public IntStringDictionary levelNames = new IntStringDictionary();
    public IntFloatDictionary levelDifficulty = new IntFloatDictionary();
}
```

#### Localization Example

```csharp
public class LocalizationManager : MonoBehaviour
{
    public StringStringDictionary englishTexts = new StringStringDictionary();
    public StringStringDictionary spanishTexts = new StringStringDictionary();
    public StringStringDictionary frenchTexts = new StringStringDictionary();
    
    public string GetText(string key, string language = "english")
    {
        switch (language.ToLower())
        {
            case "spanish":
                return spanishTexts.ContainsKey(key) ? spanishTexts[key] : key;
            case "french":
                return frenchTexts.ContainsKey(key) ? frenchTexts[key] : key;
            default:
                return englishTexts.ContainsKey(key) ? englishTexts[key] : key;
        }
    }
}
```

### Inspector Features

- **Adding Entries**: Click the "Add" button to add new key-value pairs
- **Editing Entries**: Click on key or value fields to edit them
- **Removing Entries**: Click the "×" button next to any entry to remove it
- **Validation**: Duplicate keys are detected and highlighted with warnings

---

## TabGroup Attribute

### Features

- **Toolbar Tabs**: Renders clean toolbar buttons, not foldouts
- **Grouped Fields**: Fields with the same group name share one tab bar
- **Multiple Tabs**: Show only fields that belong to the selected tab
- **Order Control**: Optional `order` decides which field draws the shared toolbar

### Usage

```csharp
using CustomAssets.EditorTools;

public class TabbedSettings : MonoBehaviour
{
    [Header("Stats")]
    [TabGroup("Stats", "Base")] public int health = 100;
    [TabGroup("Stats", "Base")] public int stamina = 50;
    [TabGroup("Stats", "Advanced")] public AnimationCurve regenCurve = AnimationCurve.Linear(0,0,1,1);

    [Header("Inventory")]
    [TabGroup("Inventory", "Weapons")] public string primaryWeapon;
    [TabGroup("Inventory", "Armor")] public string chestArmor;
}
```

### Notes

- Tabs are tracked per-object instance and group name
- Tabs within a group are ordered alphabetically for now
- Use the `order` parameter to influence which field draws the toolbar if needed

## MinMaxSlider Attribute

### Features

- **Range Selection**: Allows you to select a minimum and maximum value using a single slider in the Inspector.
- **Customizable Range**: Specify the allowed minimum and maximum values for the slider.
- **Supports Vector2/Vector2Int**: Works with `Vector2`, `Vector2Int`, and fields with two float/int values representing min and max.
- **Validation**: Ensures that the minimum value does not exceed the maximum.

### Usage
```csharp
using CustomAssets.EditorTools;

public class MinMaxSliderExample : MonoBehaviour
    {
        [Header("Float Range [0..1]")]
        [MinMaxSlider(0f, 1f)]
        public Vector2 spawnTimeRange = new Vector2(0.2f, 0.8f);

        [Header("Int Range [0..100]")]
        [MinMaxSlider(0, 100)]
        public Vector2Int levelRange = new Vector2Int(10, 40);

        [Header("Float Range [10..50] with precise fields hidden")]
        [MinMaxSlider(10f, 50f, showFields: false)]
        public Vector2 speedLimits = new Vector2(12f, 30f);
    }
```

---

## Installation

1. Copy all files to your project maintaining the folder structure
2. Ensure editor scripts are in `Editor` folders
3. The tools will be available immediately

## File Structure

```
Assets/
├── CustomAssets/
│   └── EditorToolsAsset/
│       ├── ReadOnlyAttribute.cs                    # ReadOnly attribute class
│       ├── ShowIfAttribute.cs                      # ShowIf attribute class
│       ├── HideIfAttribute.cs                      # HideIf attribute class
│       ├── ConditionalAttribute.cs                 # Base class for conditional attributes
│       ├── TitleAttribute.cs                       # Title attribute class
│       ├── SerializableDictionary.cs               # Base dictionary class
│       ├── CommonDictionaryTypes.cs                 # Pre-defined dictionary types
│       ├── TabGroupAttribute.cs                    # TabGroup attribute class
│       ├── ButtonAttribute.cs                      # Button attribute class
│       ├── Editor/
│       │   ├── ReadOnlyPropertyDrawer.cs           # ReadOnly property drawer
│       │   ├── ConditionalPropertyDrawer.cs        # ShowIf/HideIf property drawer
│       │   ├── TitlePropertyDrawer.cs              # Title property drawer
│       │   ├── SerializableDictionaryPropertyDrawer.cs # Dictionary property drawer
│       │   ├── TabGroupPropertyDrawer.cs           # TabGroup property drawer
│       │   └── ButtonEditor.cs                     # Button editor
│       ├── Examples/
│       │   ├── ReadOnlyExample.cs                   # ReadOnly usage examples
│       │   ├── ConditionalExample.cs               # ShowIf/HideIf usage examples
│       │   ├── SerializableDictionaryExample.cs   # Dictionary usage examples
│       │   ├── TabGroupExample.cs                  # TabGroup usage examples
│       │   └── ButtonExample.cs                   # Button usage examples
│       └── README.md                               # This documentation
```

## Supported Types

Both tools work with all Unity serializable types:

- **Primitive Types**: `int`, `float`, `bool`, `string`, etc.
- **Unity Types**: `Vector3`, `Vector2`, `Color`, `Transform`, etc.
- **Arrays**: Arrays of any serializable type
- **Lists**: Lists of any serializable type
- **Custom Classes**: Any class marked with `[System.Serializable]`

## Best Practices

### ReadOnly Attribute
1. **Use for Calculated Values**: Mark fields that are calculated or derived from other values
2. **Debug Information**: Use for displaying debug information that shouldn't be edited
3. **Runtime Values**: Mark fields that change during runtime but shouldn't be manually edited
4. **Documentation**: Use tooltips to explain why a field is read-only

### Serializable Dictionary
1. **Use Appropriate Types**: Use specific types for better performance
2. **Initialize with Default Values**: Set up initial data in Start() or Awake()
3. **Validate Data**: Check for required keys before accessing values
4. **Use Constants for Keys**: Define key constants to avoid typos

---

## Button Attribute

### Features

- **Method Execution**: Execute any public method with no parameters directly from the Inspector
- **Customizable Appearance**: Set button text, size, colors, and order
- **Mode Restrictions**: Enable buttons only in play mode or edit mode
- **Confirmation Dialogs**: Add confirmation prompts for dangerous actions
- **Multiple Constructors**: Various constructor overloads for different configurations
- **Error Handling**: Graceful error handling with detailed logging
- **Undo Support**: Automatic undo recording for method execution

### Usage

#### Basic Usage

```csharp
using CustomAssets.EditorTools;

public class GameManager : MonoBehaviour
{
    [Button]
    public void StartGame()
    {
        Debug.Log("Game started!");
    }
    
    [Button("Reset Level")]
    public void ResetLevel()
    {
        Debug.Log("Level reset!");
    }
}
```

#### Advanced Configuration

```csharp
public class AdvancedExample : MonoBehaviour
{
    [Button("Custom Button", 30f, 150f, 5)]
    public void CustomButton()
    {
        Debug.Log("Custom button clicked!");
    }
    
    [Button("Play Mode Only", true)]
    public void PlayModeOnlyMethod()
    {
        Debug.Log("This only works in play mode!");
    }
    
    [Button("Edit Mode Only", false, true)]
    public void EditModeOnlyMethod()
    {
        Debug.Log("This only works in edit mode!");
    }
    
    [Button("Colored Button", "#FFFF00", "#FF0000")]
    public void ColoredButton()
    {
        Debug.Log("Colored button clicked!");
    }
    
    [Button("Dangerous Action", true, "Are you sure?")]
    public void DangerousAction()
    {
        Debug.Log("Dangerous action executed!");
    }
}
```

#### Constructor Parameters

```csharp
[Button("Button Text", 25f, 0f, 5)]
public void CustomButton() { }

[Button("Play Mode Button", true)]
public void PlayModeButton() { }

[Button("Edit Mode Button", false, true)]
public void EditModeButton() { }

[Button("Confirmation Button", true, "Are you sure?")]
public void ConfirmationButton() { }

[Button("Colored Button", "#FFFF00", "#FF0000")]
public void ColoredButton() { }
```

### Constructor Parameters

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `Button()` | None | Default button with method name |
| `Button(string)` | `buttonText` | Button with custom text |
| `Button(string, float)` | `buttonText`, `buttonHeight` | Button with custom text and height |
| `Button(string, float, float)` | `buttonText`, `buttonHeight`, `buttonWidth` | Button with custom text, height, and width |
| `Button(string, float, float, int)` | `buttonText`, `buttonHeight`, `buttonWidth`, `order` | Button with custom text, height, width, and order |
| `Button(string, bool)` | `buttonText`, `playModeOnly` | Button that only works in play mode |
| `Button(string, bool, bool)` | `buttonText`, `playModeOnly`, `editModeOnly` | Button with mode restrictions |
| `Button(string, bool, string)` | `buttonText`, `showConfirmation`, `confirmationMessage` | Button with confirmation dialog |
| `Button(string, string, string)` | `buttonText`, `textColorHex`, `backgroundColorHex` | Button with custom colors (hex strings) |

### Requirements

- **Method Signature**: Methods must be `public` and have no parameters
- **Return Type**: Methods must return `void`
- **Target Types**: Works with `MonoBehaviour` and `ScriptableObject` classes
- **Editor Folder**: Button editor scripts must be in an `Editor` folder

### Examples

#### Game Management

```csharp
public class GameController : MonoBehaviour
{
    [Button("Start Game")]
    public void StartGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Game started!");
    }
    
    [Button("Pause Game")]
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Game paused!");
    }
    
    [Button("Reset Game").SetConfirmation()]
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
```

#### Component Management

```csharp
public class ComponentManager : MonoBehaviour
{
    [Button("Add Rigidbody")]
    public void AddRigidbody()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
    }
    
    [Button("Remove Rigidbody")]
    public void RemoveRigidbody()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            DestroyImmediate(rb);
        }
    }
}
```

#### Debugging Tools

```csharp
public class DebugTools : MonoBehaviour
{
    [Button("Log Transform Info")]
    public void LogTransformInfo()
    {
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Rotation: {transform.rotation}");
        Debug.Log($"Scale: {transform.localScale}");
    }
    
    [Button("Randomize Position")]
    public void RandomizePosition()
    {
        transform.position = new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );
    }
}
```

---

## ShowIf/HideIf Attributes

### Features

- **Conditional Visibility**: Show or hide fields based on other field values
- **Multiple Data Types**: Support for bool, int, float, string, enum, and Unity Object types
- **Flexible Conditions**: Check for truthy values or exact value matches
- **Tooltip Support**: Optional tooltip parameter for additional information
- **Performance Optimized**: Efficient field evaluation with reflection caching
- **Error Handling**: Graceful fallback when referenced fields are not found

### Usage

#### Basic Boolean Conditions

```csharp
using CustomAssets.EditorTools;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public bool showAdvancedSettings = false;

    [ShowIf("showAdvancedSettings")]
    [Tooltip("This field only appears when showAdvancedSettings is true")]
    public float advancedValue = 1.0f;

    [HideIf("showAdvancedSettings")]
    [Tooltip("This field is hidden when showAdvancedSettings is true")]
    public string basicMessage = "Basic settings are visible";
}
```

#### Numeric Value Conditions

```csharp
public class HealthSystem : MonoBehaviour
{
    [Range(0, 100)]
    public float health = 100f;

    [ShowIf("health", 100f)]
    [Tooltip("Only shows when health is exactly 100")]
    public string maxHealthMessage = "At full health!";

    [ShowIf("health", 0f)]
    [Tooltip("Only shows when health is exactly 0")]
    public string deathMessage = "Player is dead!";

    [HideIf("health", 0f)]
    [Tooltip("Hidden when health is 0")]
    public string aliveMessage = "Player is alive!";
}
```

#### Enum Conditions

```csharp
public enum PlayerType
{
    Guest,
    Player,
    Moderator,
    Admin
}

public class PlayerManager : MonoBehaviour
{
    public PlayerType playerType = PlayerType.Player;

    [ShowIf("playerType", PlayerType.Admin)]
    [Tooltip("Only visible for admin players")]
    public bool adminOnlySetting = true;

    [HideIf("playerType", PlayerType.Guest)]
    [Tooltip("Hidden for guest players")]
    public bool loggedInOnlySetting = true;
}
```

#### String Conditions

```csharp
public class UserInterface : MonoBehaviour
{
    public string playerName = "";

    [ShowIf("playerName")]
    [Tooltip("Only shows when playerName is not empty")]
    public string welcomeMessage = "Welcome, ";

    [HideIf("playerName")]
    [Tooltip("Hidden when playerName is not empty")]
    public string enterNameMessage = "Please enter your name";
}
```

#### Complex Multi-Conditional Examples

```csharp
public class GameSettings : MonoBehaviour
{
    [Header("Mode Settings")]
    public bool enableComplexMode = false;
    public int complexityLevel = 1;

    [ShowIf("enableComplexMode")]
    [Tooltip("Shows when complex mode is enabled")]
    public string complexModeMessage = "Complex mode is active";

    [ShowIf("complexityLevel", 3)]
    [Tooltip("Only shows when complexity level is 3")]
    public string maxComplexityMessage = "Maximum complexity reached!";

    [HideIf("enableComplexMode")]
    [Tooltip("Hidden when complex mode is enabled")]
    public string simpleModeMessage = "Simple mode is active";

    [Header("Weapon Settings")]
    public bool hasWeapon = false;
    public WeaponType weaponType = WeaponType.None;

    [ShowIf("hasWeapon")]
    public float weaponDamage = 10f;

    [ShowIf("weaponType", WeaponType.Ranged)]
    public float weaponRange = 50f;

    [ShowIf("weaponType", WeaponType.Melee)]
    public float weaponSpeed = 1.5f;
}
```

### Constructor Parameters

#### ShowIf Attribute

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `ShowIf(string)` | `fieldName` | Show field when the specified field is truthy |
| `ShowIf(string, string)` | `fieldName`, `tooltip` | Show field when truthy with tooltip |
| `ShowIf(string, object)` | `fieldName`, `expectedValue` | Show field when field equals expected value |
| `ShowIf(string, object, string)` | `fieldName`, `expectedValue`, `tooltip` | Show field when equal with tooltip |

#### HideIf Attribute

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `HideIf(string)` | `fieldName` | Hide field when the specified field is truthy |
| `HideIf(string, string)` | `fieldName`, `tooltip` | Hide field when truthy with tooltip |
| `HideIf(string, object)` | `fieldName`, `expectedValue` | Hide field when field equals expected value |
| `HideIf(string, object, string)` | `fieldName`, `expectedValue`, `tooltip` | Hide field when equal with tooltip |

### Supported Data Types

- **Boolean**: `true`/`false` values
- **Integers**: `int` values with exact matching
- **Floats**: `float` values with exact matching (using approximate equality for floating point)
- **Strings**: Non-empty strings are considered truthy
- **Enums**: Exact enum value matching
- **Unity Objects**: Non-null objects are considered truthy
- **Arrays/Lists**: Non-null and non-empty collections are considered truthy

### Truthy Value Evaluation

The following values are considered "truthy" when no expected value is specified:

- **Boolean**: `true`
- **Numeric**: Non-zero values (`int != 0`, `float != 0.0f`)
- **String**: Non-null and non-empty strings
- **Unity Objects**: Non-null objects
- **Collections**: Non-null and non-empty arrays/lists

### Best Practices

1. **Use Descriptive Field Names**: Choose clear, descriptive names for conditional fields
2. **Group Related Fields**: Use headers to organize conditional fields logically
3. **Provide Tooltips**: Add helpful tooltips to explain conditional behavior
4. **Test Edge Cases**: Verify behavior with boundary values (0, null, empty strings)
5. **Avoid Circular Dependencies**: Don't create fields that depend on each other
6. **Use Enums for States**: Prefer enums over magic numbers for better maintainability

### Performance Considerations

- **Field Lookup**: Uses reflection to find fields, but caches results for efficiency
- **Evaluation Frequency**: Conditions are evaluated every time the Inspector redraws
- **Complex Conditions**: Avoid deeply nested or complex conditional logic
- **Large Objects**: Performance impact is minimal even with many conditional fields

### Error Handling

- **Missing Fields**: Shows warning in console and displays field by default
- **Type Mismatches**: Handles type conversion gracefully where possible
- **Null References**: Safely handles null field values
- **Invalid References**: Logs warnings for debugging but doesn't break functionality

---

## Title Attribute

### Features

- **Colored Titles**: Create visually appealing titles with custom colors
- **Hex Color Support**: Use hex color strings (e.g., "#FF6B6B") for easy color specification
- **Font Size Control**: Customize the font size of titles
- **Separator Lines**: Optional separator lines below titles
- **Custom Separator Colors**: Control the color of separator lines
- **Multiple Constructors**: Various constructor overloads for different styling needs

### Usage

#### Basic Usage

```csharp
using CustomAssets.EditorTools;

public class GameSettings : MonoBehaviour
{
    [Title("Basic Settings")]
    public float speed = 5f;
    
    [Title("Advanced Settings")]
    public bool enableAdvancedMode = false;
}
```

#### With Custom Colors

```csharp
public class PlayerController : MonoBehaviour
{
    [Title("Player Stats", Color.blue)]
    public int health = 100;
    
    [Title("Weapon Configuration", "#4ECDC4")]
    public string weaponName = "Sword";
    
    [Title("Inventory", "#FF6B6B")]
    public int gold = 0;
}
```

#### With Font Size

```csharp
public class GameManager : MonoBehaviour
{
    [Title("Game Configuration", "#45B7D1", 16)]
    public bool isPaused = false;
    
    [Title("Player Data", Color.green, 14)]
    public string playerName = "Player";
}
```

#### Advanced Styling

```csharp
public class AdvancedExample : MonoBehaviour
{
    [Title("Settings", Color.white, 12, true, Color.gray)]
    public float volume = 1.0f;
    
    [Title("Debug Info", "#FFEAA7", 10, false)]
    public bool showDebugInfo = false;
}
```

### Constructor Parameters

| Constructor | Parameters | Description |
|-------------|------------|-------------|
| `Title(string)` | `title` | Basic title with default white color |
| `Title(string, Color)` | `title`, `color` | Title with custom color |
| `Title(string, string)` | `title`, `hexColor` | Title with hex color string |
| `Title(string, Color, int)` | `title`, `color`, `fontSize` | Title with color and font size |
| `Title(string, string, int)` | `title`, `hexColor`, `fontSize` | Title with hex color and font size |
| `Title(string, Color, int, bool, Color)` | `title`, `color`, `fontSize`, `showSeparator`, `separatorColor` | Full customization |

### Color Options

#### Unity Colors
```csharp
[Title("Red Title", Color.red)]
[Title("Blue Title", Color.blue)]
[Title("Green Title", Color.green)]
[Title("Yellow Title", Color.yellow)]
```

#### Hex Colors
```csharp
[Title("Coral Title", "#FF6B6B")]
[Title("Teal Title", "#4ECDC4")]
[Title("Blue Title", "#45B7D1")]
[Title("Green Title", "#96CEB4")]
[Title("Yellow Title", "#FFEAA7")]
[Title("Purple Title", "#DDA0DD")]
```

### Examples

#### Game Configuration

```csharp
public class GameConfig : MonoBehaviour
{
    [Title("Player Settings", "#4ECDC4")]
    public float playerSpeed = 5f;
    public int playerHealth = 100;
    
    [Title("Weapon Settings", "#FF6B6B")]
    public float weaponDamage = 10f;
    public float weaponRange = 50f;
    
    [Title("UI Settings", "#45B7D1")]
    public bool showHealthBar = true;
    public bool showMinimap = true;
}
```

#### Character Stats

```csharp
public class CharacterStats : MonoBehaviour
{
    [Title("Basic Stats", Color.white, 14)]
    public int strength = 10;
    public int dexterity = 10;
    public int intelligence = 10;
    
    [Title("Combat Stats", "#FF6B6B", 12)]
    public float attackPower = 15f;
    public float defense = 8f;
    public float criticalChance = 0.1f;
    
    [Title("Magic Stats", "#DDA0DD", 12)]
    public float mana = 100f;
    public float manaRegen = 5f;
    public float spellPower = 20f;
}
```

#### Debug Information

```csharp
public class DebugManager : MonoBehaviour
{
    [Title("Debug Settings", "#FFEAA7", 10)]
    public bool enableDebugLogs = false;
    public bool showFPS = false;
    public bool showMemoryUsage = false;
    
    [Title("Performance", "#96CEB4", 10)]
    public int targetFPS = 60;
    public bool enableVSync = true;
    public int qualityLevel = 2;
}
```

### Best Practices

1. **Use Consistent Colors**: Choose a color scheme and stick to it throughout your project
2. **Meaningful Titles**: Use descriptive titles that clearly indicate the section's purpose
3. **Appropriate Font Sizes**: Use larger fonts (14-16) for main sections, smaller (10-12) for subsections
4. **Color Coding**: Use different colors to categorize different types of settings
5. **Separator Usage**: Use separators to clearly divide sections, disable for compact layouts

### Color Schemes

#### Professional Blue Theme
```csharp
[Title("Main Settings", "#2E86AB")]
[Title("Player Data", "#A23B72")]
[Title("Gameplay", "#F18F01")]
[Title("Audio", "#C73E1D")]
```

#### Nature Green Theme
```csharp
[Title("Environment", "#2D5016")]
[Title("Creatures", "#4A7C59")]
[Title("Resources", "#8FBC8F")]
[Title("Weather", "#98FB98")]
```

#### Warm Orange Theme
```csharp
[Title("Core Systems", "#FF6B35")]
[Title("User Interface", "#F7931E")]
[Title("Audio", "#FFD23F")]
[Title("Graphics", "#06FFA5")]
```

---

## Troubleshooting

### ReadOnly Attribute Issues
- **Field Still Editable**: Ensure the `ReadOnlyPropertyDrawer.cs` is in an `Editor` folder
- **Compilation Errors**: Make sure you're using the correct namespace: `CustomAssets.EditorTools`

### ShowIf/HideIf Attribute Issues
- **Fields Not Showing/Hiding**: Ensure the field name is spelled correctly and exists on the same object
- **Compilation Errors**: Make sure `ConditionalPropertyDrawer.cs` is in an `Editor` folder
- **Performance Issues**: Avoid referencing fields in deeply nested objects or complex inheritance chains
- **Circular Dependencies**: Don't create fields that depend on each other in a loop
- **Type Mismatches**: Ensure the expected value type matches the referenced field type
- **Missing Field Warnings**: Check console for warnings about missing fields and fix field names

### Title Attribute Issues
- **Title Not Showing**: Ensure `TitlePropertyDrawer.cs` is in an `Editor` folder
- **Colors Not Working**: Check that hex colors are in correct format (#RRGGBB)
- **Font Size Issues**: Font size should be between 8-20 for best results
- **Separator Not Showing**: Ensure `ShowSeparator` is set to true in constructor
- **Compilation Errors**: Make sure you're using the correct namespace: `CustomAssets.EditorTools`

### Button Attribute Issues
- **Button Not Appearing**: Ensure the method is `public` and has no parameters
- **Method Not Executing**: Check that the method returns `void` and has no parameters
- **Compilation Errors**: Ensure `ButtonEditor.cs` is in an `Editor` folder
- **Confirmation Not Showing**: Check that the correct constructor is used for confirmation

## MinMaxSlider Attribute

### Features

- Min–Max slider for ranges backed by Vector2 (float) and Vector2Int (int)
- Optional numeric fields for precise input on both ends
- Enforces limits (clamp to min/max) and ordering (max >= min)
- Lightweight and consistent with Unity’s built-in MinMaxSlider UX

### Usage

- Float ranges (Vector2)

```csharp
using CustomAssets.EditorTools;

public class SpawnSettings : MonoBehaviour
{
    // Allows picking a range between 0 and 1
    [MinMaxSlider(0f, 1f)]
    public Vector2 spawnTimeRange = new Vector2(0.2f, 0.8f);
}
```

- Integer ranges (Vector2Int)

```csharp
using CustomAssets.EditorTools;

public class LevelSettings : MonoBehaviour
{
    // Allows picking an integer range between 0 and 100
    [MinMaxSlider(0, 100)]
    public Vector2Int levelRange = new Vector2Int(10, 40);
}
```

- Hide numeric fields and control decimals for floats

```csharp
using CustomAssets.EditorTools;

public class MovementSettings : MonoBehaviour
{
    // No numeric entry fields, decimals parameter controls float precision when fields are shown
    [MinMaxSlider(10f, 50f, showFields: false, decimals: 2)]
    public Vector2 speedLimits = new Vector2(12f, 30f);
}
```

### Supported types
- Vector2 for float ranges (X = min, Y = max)
- Vector2Int for integer ranges (X = min, Y = max)

### Notes
- Demonstrated in Assets/CustomAssets/EditorToolsAsset/Examples/MinMaxSliderExample.cs
- Also represented in the EditorToolsExampleScene.unity under Examples/Scenes

## License

This tool is part of the Custom Assets collection and follows the project's licensing terms.
