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
