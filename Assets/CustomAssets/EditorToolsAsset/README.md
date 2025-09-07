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
    public StringStringDictionary itemNames = new StringStringDictionary();
    public StringIntDictionary itemPrices = new StringIntDictionary();
}
```

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
│       ├── SerializableDictionary.cs               # Base dictionary class
│       ├── CommonDictionaryTypes.cs                 # Pre-defined dictionary types
│       ├── Editor/
│       │   ├── ReadOnlyPropertyDrawer.cs           # ReadOnly property drawer
│       │   └── SerializableDictionaryPropertyDrawer.cs # Dictionary property drawer
│       ├── Examples/
│       │   ├── ReadOnlyExample.cs                   # ReadOnly usage examples
│       │   └── SerializableDictionaryExample.cs   # Dictionary usage examples
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

## Troubleshooting

### ReadOnly Attribute Issues
- **Field Still Editable**: Ensure the `ReadOnlyPropertyDrawer.cs` is in an `Editor` folder
- **Compilation Errors**: Make sure you're using the correct namespace: `CustomAssets.EditorTools`

### Button Attribute Issues
- **Button Not Appearing**: Ensure the method is `public` and has no parameters
- **Method Not Executing**: Check that the method returns `void` and has no parameters
- **Compilation Errors**: Ensure `ButtonEditor.cs` is in an `Editor` folder
- **Confirmation Not Showing**: Check that the correct constructor is used for confirmation

## License

This tool is part of the Custom Assets collection and follows the project's licensing terms.
