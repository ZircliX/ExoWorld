# AGENTS.md - OverBangStudio GameName Project Guide

## Project Overview
This is a **Unity multiplayer game project** (TemaLeMultiLupeni) built with a modular architecture using Assembly Definition files (.asmdef). The project uses **ExoWorld** as the core framework namespace and implements a phase-based game state machine.

---

## Architecture Overview

### Core Systems

#### 1. **Game Controller & Global State** (`Assets/Scripts/Core/`)
- **Singleton Access**: `GameController` static class manages global game state (not a MonoBehaviour)
- **Key Properties**:
  - `CurrentGameMode`: Active IGameMode implementation
  - `GameDatabase`: Lazy-loaded ScriptableObject from `Resources/GameDatabase`
  - `Metrics`: Lazy-loaded `GameMetrics` from `Resources/GameMetrics`
  - `SessionManager`: Manages player sessions
  - `CursorLockModePriority` / `CursorVisibleStatePriority`: Priority-based property system for input control
- **Initialization**: `RuntimeInitializeOnLoadMethod(BeforeSceneLoad)` at game start
- **Pattern**: Use `GameController.GetOrCreateGameMode<T>()` to manage game modes

#### 2. **Phase System (State Machine)**
- **Base Interface**: `IPhase` defines lifecycle: `OnBegin()` → `Execute()` → `OnEnd()` (all async)
- **GameMode Pattern**: `IGameMode` extends `IPhase` 
- **Concrete Phases**:
  - `GameplayPhase` (abstract base)
  - `HostGameplayPhase` / `ClientGameplayPhase` (networked variants)
  - `HubPhase` (menu/lobby state)
- **Phase Listeners**: Classes implement `IPhaseListener<T>` to hook into phase transitions
- **Usage**: Phases are set via `GameController.SetGameMode(phaseInstance)`

#### 3. **Level Management** (`LevelManager` - Singleton)
- **Lifecycle States**: `None` → `Initializing` → `Ready` → `Running` → `Disposed`
- **Key Responsibilities**:
  - Initialize gameplay (map, player, enemies, pooling, UI)
  - Manage game timer (`CurrentGameTime`, `OnTimerTick`, `OnTimerEnd` events)
  - Spawn enemies via `EnemySpawnerManager`
  - Handle cursor lock/visibility via `GameController` priorities
- **Access**: `LevelManager.Instance`
- **Setup Flow**: `Initialize(phase)` → `StartLevel()` → continuous `Update()` → `Dispose()`

---

## Modular Assembly Structure

### Critical Dependencies Chain
```
Core Layer:
  OverBang.ExoWorld.Core (base)
  ├── ZTools.Core, ZTools.Logger.Core
  ├── OverBang.Pooling.Runtime
  └── External libraries (DOTween, BroAudio, etc.)

Gameplay Layer:
  OverBang.ExoWorld.Gameplay (depends on Core)
  ├── Phase systems
  ├── Abilities, Enemies, Health, Upgrade
  ├── Quests, Rewards, Objectives
  └── Movement, Targeting, UI components

Editor & Debug Layers:
  OverBang.ExoWorld.Editor (Editor only)
  OverBang.ExoWorld.Debugging (Runtime debug tools)
```

**Assembly References**: Found in `.asmdef` files. Use **Rider/Visual Studio** to navigate references if lost.

---

## Gameplay Patterns

### 1. **Upgrade System** (`Assets/Scripts/Gameplay/Upgrade/`)
- `UpgradeManager`: Central manager (static methods)
- `UpgradeTable`: Interaction point for UI/gameplay
- Pattern: Upgrades modify player stats via `GameMode.Players` manager
- Events: `UpgradeListener` listens for phase changes and metrics updates

### 2. **Quest/Objective System** 
- **Core**: `ObjectivesManager.AddObjective()` / `RemoveObjective()` (static)
- **Event Dispatch**: `ObjectivesManager.DispatchGameEvent(IGameEvent)`
- **Reward Processing**: `RewardManager.ProcessReward(RewardData)` handles quest completion rewards
- **Custom Data**: Create `RewardData` subclasses (e.g., `TrinititeRewardData`) + `MonoRewardProcessor<T>`

### 3. **Object Pooling** (`OverBang.Pooling.*`)
- **Manager**: `PoolManager.Instance` (singleton)
- **Setup**: `PoolUtils.SetupPooling(PoolType.All)` in `LevelManager`
- **Access**: Pool configurations via `IPoolConfig` or `PoolResource` ScriptableObjects
- **Cleanup**: `PoolManager.Instance.ClearPools()` in `LevelManager.Dispose()`

### 4. **Interaction System**
- **Interface**: `IInteractible` with `OnInteract(PlayerInteraction)` method
- **Player Side**: `PlayerInteraction` component handles collision/interaction
- **Interaction Types**: Defined in `InteractionType` enum
- **UI**: `PlayerInteractionUI` displays available interactions

---

## Data Flow & Communication

### Player/Session Management
- **Player Model**: `LocalGamePlayer` / `NetworkGamePlayer` (in `GameMode.Players`)
- **Properties**: `UpdatePlayerProperty(propertyID, value)` for persistent data
- **Access**: `SessionManager.Global.CurrentPlayer` in GameController

### Event System
- **Phase Listeners**: Hook into phase begin/end
- **Delegates**: Use C# events (e.g., `OnStateChanged`, `OnTimerTick`)
- **Game Events**: Dispatch via `ObjectivesManager.DispatchGameEvent()`
- **Priority System**: `Priority<T>` for competing state changes (cursors, UI layers)

### Resource Loading
- **ScriptableObjects**: Placed in `Assets/Resources/` (lazy-loaded via `Resources.Load<T>()`)
- **Prefabs**: In `Assets/Project/Prefabs/` or loaded via pools
- **Scenes**: Managed by `SceneManager.LoadSceneAsync()` with callbacks

---

## Key Developer Workflows

### Adding a New Gameplay Feature
1. Create namespace under `OverBang.ExoWorld.Gameplay.*` (or `.Core.*` if fundamental)
2. Reference appropriate `.asmdef` for dependencies
3. Hook into phase lifecycle via `IPhaseListener<GameplayPhase>` if needed
4. Use `GameController` / `LevelManager.Instance` for global access
5. Expose static methods in manager classes for convenient access

### Debugging
- **Editor Tools**: `Assets/Scripts/Editor/`
  - `SceneManagerWindow`: Load scenes without running game
  - `DispatchGameEventEditor`: Test quest events manually
- **Debug Namespace**: `OverBang.ExoWorld.Debugging` has runtime debug utilities
- **Logging**: `ZTools.Logger.Core` for structured logging

### Testing
- **Phases**: Manually transition via `GameController.SetGameMode()`
- **Events**: Use `DispatchGameEventEditor` window or call `ObjectivesManager.DispatchGameEvent()` directly
- **Player State**: Check `SessionManager.Global.CurrentPlayer` properties in Debugger

---

## Common Pitfalls & Conventions

### Naming & Organization
- **Classes**: `Manager`, `Handler`, `Processor` suffixes for systems
- **Interfaces**: `I` prefix (e.g., `IGameMode`, `IPhase`)
- **Namespaces**: Follow folder structure: `OverBang.ExoWorld.Gameplay.Upgrade` for `Assets/Scripts/Gameplay/Upgrade/`

### Async Patterns
- All phase methods return `Awaitable` (not `Task`)
- Use `async Awaitable` / `await` for phase code
- Call `result.Run()` to execute returned Awaitables (see `GameController.SetGameMode()`)

### Dependency Injection
- Use constructor parameters for major dependencies
- Static accessors (`GameController`, `LevelManager.Instance`) for global services only
- Prefer instance methods over statics in gameplay classes

### Resource Management
- Always call `LevelManager.Dispose()` before scene unload
- Use `PoolManager.Instance.ClearPools()` to reset object pools between levels
- Load ScriptableObjects via `Resources.Load<T>()` with proper error handling

---

## Critical File Locations

```
Assets/
├── Scripts/
│   ├── Core/                    # Base systems
│   │   ├── GameController.cs    # Global entry point
│   │   ├── GameMode/            # IGameMode & phase system
│   │   ├── Phases/              # IPhase interface & lifecycle
│   │   └── Database/            # GameDatabase, GameMetrics
│   ├── Gameplay/                # Feature modules
│   │   ├── Phase/               # Concrete phase implementations
│   │   ├── Level/LevelManager.cs
│   │   ├── Upgrade/
│   │   ├── Quests/
│   │   └── ...
│   └── Editor/                  # Editor-only tools
├── Resources/
│   ├── GameDatabase.asset
│   ├── GameMetrics.asset
│   └── SpawnScenario/
└── Project/
    ├── Prefabs/                 # Reusable game objects
    └── ScriptableObjects/       # Game data configs
```

---

## External Dependencies (Key Ones)
- **DOTween**: Animation library (tweening system)
- **BroAudio**: Audio management
- **Odin Inspector**: Editor extensions (Sirenix)
- **Unity Input System**: InputSystem_Actions.inputactions
- **Networking**: Multi-assembly support for Host/Client phases

---

## Next Steps for New Developers
1. Read `GameController.cs` to understand initialization
2. Explore `LevelManager.cs` for level lifecycle
3. Study `GameplayPhase.cs` to understand phase flow
4. Check `Assets/Scripts/Gameplay/Upgrade/UpgradeManager.cs` for a complete feature example
5. Use the Phase Listener pattern to hook into game state

