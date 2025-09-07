# Editor Tools Asset

A collection of Unity editor tools that enhance the Inspector experience and provide reusable functionality for game development.

## Tools Included

### 1. ReadOnly Attribute
A Unity editor tool that provides a `[ReadOnly]` attribute for making fields read-only in the Unity Inspector.

### 2. Serializable Dictionary
A comprehensive serializable dictionary system with Inspector integration for editing key-value pairs directly in Unity.

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

## Troubleshooting

### ReadOnly Attribute Issues
- **Field Still Editable**: Ensure the `ReadOnlyPropertyDrawer.cs` is in an `Editor` folder
- **Compilation Errors**: Make sure you're using the correct namespace: `CustomAssets.EditorTools`

### Serializable Dictionary Issues
- **Dictionary Not Showing in Inspector**: Ensure the dictionary field is public and namespace is imported
- **Duplicate Key Warnings**: Check for duplicate keys in the Inspector
- **Serialization Errors**: Ensure all key and value types are serializable

## License

This tool is part of the Custom Assets collection and follows the project's licensing terms.
