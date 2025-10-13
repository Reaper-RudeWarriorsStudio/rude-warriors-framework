# 🧠 Rude Warriors Framework — Changelog

All notable changes to the **Rude Warriors Framework** will be documented here.  
This project follows a lightweight semantic versioning format:  
**MAJOR.MINOR.PATCH-IDENTIFIER** (example: `1.0.0-core`)

---

## [1.0.0-core] — 2025-10-13
**Status:** Core Framework Complete 🏁  
This marks the first fully stable core release of the Rude Warriors Framework.

### 🔧 Core Systems
- Added **ServiceLocator.cs** — global dependency registry.
- Added **EventBus.cs** — decoupled event broadcasting system.
- Added **DataService.cs** — runtime and persistent data handler.
- Added **SceneContext.cs** — automatic per-scene service registration.
- Added **FrameworkBootstrapper.cs** — persistent initialization chain.

### 🧩 Subsystems
- Added **AudioService.cs** — centralized music/SFX manager.
- Added **UIService.cs** — modular UI panel control.
- Added **TimeService.cs** — global pause/slow-motion control.
- Added **SaveProfileManager.cs** — multi-slot profile management.
- Added **RWDebug.cs** — unified, color-coded logging utility.

### 🧰 Tools
- Added **FrameworkExplorerWindow.cs** — in-editor service viewer.
- Added **DebugConsole.cs** — in-game runtime command console.

### ⚙️ Integration
- Bootstrapper now auto-registers all core services at startup.
- All runtime systems now output through `RWDebug` for unified logging.
- SceneContext auto-registers [AutoRegister] systems dynamically.
- Core systems persist automatically between scenes.

### 🗂 Structure
# 🧠 Rude Warriors Framework — Changelog

All notable changes to the **Rude Warriors Framework** will be documented here.  
This project follows a lightweight semantic versioning format:  
**MAJOR.MINOR.PATCH-IDENTIFIER** (example: `1.0.0-core`)

---

## [1.0.0-core] — 2025-10-13
**Status:** Core Framework Complete 🏁  
This marks the first fully stable core release of the Rude Warriors Framework.

### 🔧 Core Systems
- Added **ServiceLocator.cs** — global dependency registry.
- Added **EventBus.cs** — decoupled event broadcasting system.
- Added **DataService.cs** — runtime and persistent data handler.
- Added **SceneContext.cs** — automatic per-scene service registration.
- Added **FrameworkBootstrapper.cs** — persistent initialization chain.

### 🧩 Subsystems
- Added **AudioService.cs** — centralized music/SFX manager.
- Added **UIService.cs** — modular UI panel control.
- Added **TimeService.cs** — global pause/slow-motion control.
- Added **SaveProfileManager.cs** — multi-slot profile management.
- Added **RWDebug.cs** — unified, color-coded logging utility.

### 🧰 Tools
- Added **FrameworkExplorerWindow.cs** — in-editor service viewer.
- Added **DebugConsole.cs** — in-game runtime command console.

### ⚙️ Integration
- Bootstrapper now auto-registers all core services at startup.
- All runtime systems now output through `RWDebug` for unified logging.
- SceneContext auto-registers [AutoRegister] systems dynamically.
- Core systems persist automatically between scenes.

### 🗂 Structure
Runtime/
├─ Core/
│ ├─ AudioService.cs
│ ├─ DataService.cs
│ ├─ DebugConsole.cs
│ ├─ EventBus.cs
│ ├─ FrameworkBootstrapper.cs
│ ├─ RWDebug.cs
│ ├─ SaveProfileManager.cs
│ ├─ SceneContext.cs
│ ├─ ServiceLocator.cs
│ ├─ TimeService.cs
│ └─ UIService.cs
└─ Editor/
└─ FrameworkExplorerWindow.cs

### 🪵 Notes
- The Core layer is now feature-complete and frozen.  
- Future updates will expand into Gameplay, Systems, and Tools modules.  
- Target Unity version: **2021.3 LTS or newer.**

---

## [Unreleased]
### Planned
- `EntitySystem.cs` — base component for gameplay actors.
- `InteractionSystem.cs` — generalized interaction handler.
- `InventorySystem.cs` — item tracking and persistence.
- `LocalizationService.cs` — multi-language UI/Dialogue support.
- `NetService.cs` — optional networked event layer.

---

## [v0.2.0] — Core + Input System Integration  
**Date:** 2025-10-13  
**Type:** Major Feature Update  

### 🧠 Core Framework
- **FrameworkBootstrapper.cs**
  - Rebuilt initialization flow using `FindObjectsByType` (Unity 2023+ compliant)
  - Refined persistent registration for Audio, UI, and DebugConsole
  - Enhanced runtime safety for duplicate bootstrappers
  - Clean service initialization logging via `RWDebug.System()`

- **ServiceLocator.cs**
  - Overhauled registration and retrieval logic
  - Added `TryGet<T>(out T service)` overload
  - Added error handling and warnings via `RWDebug.Warning()`
  - Unified singleton registry for core systems

- **SceneContext.cs**
  - Modernized to auto-register scene-level services
  - Fixed `IService` dependency issues
  - Replaced deprecated `Register` overloads
  - Ensures service unregistration on scene unload

- **RWDebug.cs**
  - Added missing `Warning()` and `Error()` levels
  - Unified visual log prefixing for framework systems

- **DebugConsole.cs**
  - Fully rewritten for stability and readability
  - Fixed parsing and syntax issues
  - Added in-game UI with runtime command support  
    (`help`, `clear`, `services`, `publish`, `quit`)
  - Integrated with EventBus for real-time event firing

---

### ⚙️ Gameplay Systems
- **FirstPersonController.cs**
  - Migrated to **Unity Input System**
  - Added smooth crouch, jump, and sprint control
  - Built-in `GroundCheck` auto-creation
  - Uses framework service registration for modularity
  - Added camera rotation smoothing & cursor lock logic

- **InputSystem_Actions.cs**
  - Added as a self-contained Input System asset replacement
  - Supports:
    - Player actions: Move, Look, Jump, Crouch, Sprint, Interact, Attack
    - UI actions: Navigate, Click, Scroll, Submit, Cancel
  - Compatible with all major control schemes  
    (Keyboard & Mouse, Gamepad, XR, Joystick, Touch)

---

### 🧩 Example Systems
- **ExampleSystem.cs**
  - Linked `AutoRegister` attribute to `IService`
  - Updated to match the new ServiceLocator structure

---

### ✅ General Notes
- All warnings resolved (deprecated API migration complete)
- Fully compatible with Unity 2023.3–2025.x
- Package is verified for runtime use under `/Packages/com.rudewarriors.focusedframework/`
- Ideal for modular extensions — `Core`, `Gameplay`, and `UI` ready for expansion

---

### 🔖 Next Goals
- Add **Input Manager UI overlay** (bind remapping)
- Introduce **PlayerData Sync Service**  
- Create **Framework Editor Window v2** for live system monitoring

---
