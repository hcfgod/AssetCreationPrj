# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

Project type: Unity 6 (LTS) project using URP and the new Input System.
Unity version is pinned in ProjectSettings/ProjectVersion.txt: 6000.2.2f1.

Common commands (Windows, PowerShell)
- Set the Unity Editor path once per session
```powershell path=null start=null
$env:UNITY_PATH = "C:\Program Files\Unity\Hub\Editor\6000.2.2f1\Editor\Unity.exe"
```

- Build a Windows 64-bit player (non-interactive)
```powershell path=null start=null
& $env:UNITY_PATH `
  -batchmode -nographics -quit `
  -projectPath . `
  -buildWindows64Player "Builds/Windows/AssetCreationPrj/AssetCreationPrj.exe" `
  -logFile -
```
Notes:
- -logFile - streams logs to the terminal.
- If you need a different platform, use the corresponding -build...Player argument (e.g., -buildOSXUniversalPlayer, -buildLinux64Player). Output directories are up to you.

- Run all EditMode tests
```powershell path=null start=null
& $env:UNITY_PATH `
  -batchmode -nographics -quit `
  -projectPath . `
  -runTests -testPlatform editmode `
  -testResults "TestResults/EditMode.xml" `
  -logFile -
```

- Run all PlayMode tests
```powershell path=null start=null
& $env:UNITY_PATH `
  -batchmode -nographics -quit `
  -projectPath . `
  -runTests -testPlatform playmode `
  -testResults "TestResults/PlayMode.xml" `
  -logFile -
```

- Run a single test (filter by fully qualified name or method name)
```powershell path=null start=null
& $env:UNITY_PATH `
  -batchmode -nographics -quit `
  -projectPath . `
  -runTests -testPlatform editmode `
  -testFilter "Namespace.ClassName.MethodName" `
  -testResults "TestResults/Single.xml" `
  -logFile -
```
Notes:
- The Test Framework package (com.unity.test-framework) is present; this enables the CLI test runner. If there are no test assemblies yet, add Tests assemblies under Assets with an .asmdef configured for tests.

- Linting
No project-level CLI linter is configured. Use IDE analyzers (Rider/Visual Studio) for code style and inspections. If a CLI linter is needed later, adopt Roslyn analyzers or dotnet-format via a generated solution and a dedicated step.

High-level architecture
- Rendering and Input
  - URP via com.unity.render-pipelines.universal (17.2.0)
  - Input System via com.unity.inputsystem (1.14.2)

- Reusable assets contained under Assets/CustomAssets
  - Editor Tools Asset (Assets/CustomAssets/EditorToolsAsset)
    - Purpose: Improves Inspector UX with attributes and custom drawers.
    - Key runtime attributes: ReadOnly, ShowIf/HideIf, Title, Button, TabGroup, SerializableDictionary.
    - Editor-only drawers under Editor/ implement the Inspector behavior.
    - Examples/ includes sample scripts and scenes demonstrating usage.
  - KCharacterController / ECM2 (Assets/CustomAssets/KCharacterController)
    - Purpose: Kinematic character controller and movement system with multiple example scenes.
    - Assembly: KCharacterController (Assets/CustomAssets/KCharacterController/Source/KCharacterController.asmdef).
    - Source/ organization highlights:
      - Characters/: core character type(s)
      - Components/: CharacterMovement, NavMeshCharacter, PhysicsVolume, etc.
      - Common/, Helpers/, Utils/, Interfaces/: math helpers, utilities, and contracts that underpin movement and collision.
    - Editor/: Editor-only tooling (e.g., CharacterFactory menu) and example scene setup helpers.
    - Shared Assets/: Materials, animations, input maps, prefabs used by the examples.

- Project settings and packages
  - Unity version pinned to 6000.2.2f1 (ProjectSettings/ProjectVersion.txt)
  - Package highlights: Test Framework, URP, UGUI, Timeline, Visual Scripting, IDE integrations.

Important project rules (from .cursor/rules)
- Versioning and editor
  - Use the pinned Unity version from ProjectSettings/ProjectVersion.txt. Avoid mid-milestone upgrades.
  - Force Text serialization with Visible Meta Files; use Asset Pipeline v2.
- Source control
  - Git with LFS for large/binary assets. Use UnityYAMLMerge (Smart Merge) for scenes/prefabs.
  - Do not commit Library/, Temp/, Logs/, Build/, UserSettings/, DerivedDataCache/.
- Assemblies and code layout
  - Separate Runtime, Editor, and Tests via asmdef files where appropriate. Keep a dependency DAG (utilities below gameplay).
- CI/build expectations
  - Batchmode builds should work end-to-end. Tests (EditMode/PlayMode) act as a gate when present.
- Native plugins and Windows builds — Premake policy
  - For any native plugins or tooling, generate Visual Studio projects with Premake 5 and provide a bootstrap script that fetches premake and generates solution files. Do not commit generated solutions.

Notes for agents
- This repository is a Unity project; prefer Unity batchmode for build and tests. Use the provided commands and set $env:UNITY_PATH to the appropriate Unity.exe for the pinned version.
- The KCharacterController and Editor Tools assets are intended to be reusable across projects; avoid tightly coupling new features to example scenes.
