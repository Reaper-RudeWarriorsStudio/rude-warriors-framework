# ⚙️ How to Use the Rude Warriors Framework

The **Rude Warriors Framework (RWF)** is a lightweight C# library for Unity.  
It handles the architecture — you handle the gameplay.  
Think of it as *Bootstrap for code*: a library that gives structure, not prefab clutter.

---

## 🌿 1. Install

Add the package to your Unity project’s `manifest.json`:

```json
{
  "dependencies": {
    "com.rudewarriors.framework": "https://github.com/Reaper-RudeWarriorsStudio/rude-warriors-framework.git#v1.0.0-core"
  }
}
```
Or clone the repo into your project’s Packages folder manually.
# 🚀 2. Quick Start

Create a simple startup script in your first scene:
```csharp
using UnityEngine;
using RudeWarriors.Framework.Core;

public class Startup : MonoBehaviour
{
    public UIService ui;
    public AudioService audio;

    private void Awake()
    {
        Framework.Init()
            .UseEventBus()
            .UseDataService()
            .UseTimeService()
            .UseSaveProfiles()
            .UseUI(ui)
            .UseAudio(audio)
            .UseDebugConsole()
            .Build();
    }
}
```
-💡 Only what you call with .UseX() gets built.
      No hidden singletons, no background managers.

If you prefer a prefab workflow, drop the FrameworkBootstrapper into your scene and use the inspector to wire optional services — it simply delegates to the same builder above.

# 🧠 3. Core Concepts
## Service Locator

Global registry for all systems.
Retrieve any registered service anywhere:
```csharp
var audio = ServiceLocator.Get<IAudioService>();
audio.PlaySFX(clickSound);
```
-ServiceLocator methods:

  -Register<T>(T instance) — register service

  -Get<T>() — fetch registered service (throws if missing)

  -TryGet<T>(out T instance) — safe fetch

  -Clear() — remove all registrations (used by SceneContext when unloading)

## Event Bus

Loose, decoupled communication between systems:
```csharp
var bus = ServiceLocator.Get<IEventBus>();
bus.Publish(new PlayerDied());
bus.Subscribe<PlayerDied>(OnPlayerDied);
```
Message types are plain classes/structs. Subscribe and publish by type to avoid stringly-typed nonsense.

## Data & Profiles

Simple JSON save/load and player profiles:
```csharp
var data = ServiceLocator.Get<IDataService>();
data.SavePersistent("settings", settingsObject);
data.TryLoadPersistent("settings", out SettingsData settings);

var saves = ServiceLocator.Get<ISaveProfileManager>();
saves.CreateProfile("Player1");
saves.Save("PlayerStats", stats);
var loaded = saves.Load<PlayerStats>("PlayerStats");
```
IDataService exposes persistent and runtime stores. SaveProfileManager builds on top of that for multi-slot behavior.

## UI & Audio
Centralized control for UI and audio:
```csharp
ServiceLocator.Get<IUIService>().Show("PauseMenu");
ServiceLocator.Get<IAudioService>().PlayMusic(mainTheme);
```
Register GameObjects or implement your own services and register them via ServiceLocator — flexibility is the point.

## Time Control

Pause, resume, and smooth slow-motion:
```csharp
var time = ServiceLocator.Get<ITimeService>();
time.Pause();
time.SmoothTimeScale(0.3f, 0.5f);
time.Resume();
```
TimeService exposes DeltaTime, TimeScale, and IsPaused for systems that prefer injecting time semantics.

## Debug & Logs

Use RWDebug for consistent, tagged logging across the framework:
```csharp
RWDebug.Log("Level Loaded", "Scene");
RWDebug.Warn("Low FPS Detected");
RWDebug.Error("Missing Asset", "Data");
```
RWDebug publishes RWDebugEvent to EventBus, so the in-game DebugConsole can echo logs.

Open the Debug Console at runtime with the tilde/backquote key (~). The console supports commands like:

-help

-services

-clear

-publish <EventName>

-quit

# 🧱 4. Scene Workflow

1.) Create an empty GameObject named Startup in your startup scene.

2.) Attach your Startup script (example above) or add the FrameworkBootstrapper prefab.

3.) (Optional) Drag in references for AudioService or UIService if you use prefabs.

4.) Press Play — the framework links services and logs the initialization.

To inspect what’s registered at runtime, open Rude Warriors → Framework Explorer from the Unity top menu (Editor-only). It displays all services in the ServiceLocator.
SceneContext will scan the scene for any [AutoRegister] MonoBehaviours and register their first implemented interface automatically. Use that for scene-local systems.

# 🧰 5. Extending RWF
Make new systems by implementing an interface and registering them.

Example: Inventory service
```csharp
public interface IInventoryService { void Add(string item); }

public class InventoryService : IInventoryService
{
    public void Add(string item) => RWDebug.Log($"Added {item}", "Inventory");
}
```
Register:
```csharp
ServiceLocator.Register<IInventoryService>(new InventoryService());
```
Or register via the builder in startup (if your service is a MonoBehaviour instance):
```csharp
Framework.Init()
    .UseEventBus()
    .UseDataService()
    .Build();

ServiceLocator.Register<IInventoryService>(new InventoryService());
```
Design rules:

-Core should never depend on Gameplay. Keep Core minimal and stable.

-Prefer interfaces and composition. Make systems replaceable.

-Avoid FindObjectOfType in game code; use the ServiceLocator instead.

#🪶 6. Recommended Folder Layout
Assets/
 ├─ Scripts/
 │   ├─ Startup/
 │   ├─ Gameplay/
 │   ├─ UI/
 │   └─ Systems/
 └─ Packages/
     └─ com.rudewarriors.framework/
         ├─ Runtime/
         │  └─ Core/
         └─ Editor/
If you include the repo as a UPM package, keep samples in Samples~/ so Unity shows convenient import samples in Package Manager.

#🧾 7. Best Practices

-Keep gameplay code outside Runtime/Core/. Core is stable plumbing.

-Always access global services via ServiceLocator.

-Use EventBus for cross-system communication; prefer typed events over strings.

-Use RWDebug for logs so logs are consistent and can be routed to in-game consoles.

-Initialize once during startup — avoid rebuilding the framework mid-run unless you know what you’re doing.

-Provide clear interfaces for any public services you add so other devs can replace them.

# 🏁 8. Summary & Cheatsheet
| Task                 | One-Liner                                                  |
| -------------------- | ---------------------------------------------------------- |
| Initialize Framework | `Framework.Init().UseEventBus().UseDataService().Build();` |
| Register a Service   | `ServiceLocator.Register<IMyService>(new MyService());`    |
| Get a Service        | `var s = ServiceLocator.Get<IMyService>();`                |
| Publish Event        | `ServiceLocator.Get<IEventBus>().Publish(new MyEvent());`  |
| Log                  | `RWDebug.Log("msg", "Category");`                          |
| Pause                | `ServiceLocator.Get<ITimeService>().Pause();`              |

### Rude Warriors Framework v1.0.0-core
Lightweight • Modular • Developer-Controlled
MIT © Rude Warrior’s Studio
