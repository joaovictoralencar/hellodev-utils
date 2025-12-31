# HelloDev Utils

Foundation utilities for HelloDev packages. This is the base package that other HelloDev packages depend on.

## Features

- **RuntimeScriptableObject** - Abstract base class that auto-resets ScriptableObject state between play sessions
- **UnityEvent Extensions** - Safe event handling (`SafeInvoke`, `SafeSubscribe`, `SafeUnsubscribe`) with support for 0-4 parameters
- **Transform Extensions** - Transform and GameObject utilities (`DestroyAllChildren` with optional condition filter)
- **Bootstrap Support** - `IBootstrapInitializable` interface for controlled system initialization
- **Locator Pattern** - `LocatorBase_SO` base class for locator ScriptableObjects
- **Tweening Abstractions** - `ITweenProvider`, `ITweenHandle`, `EaseType` for animation abstraction

## Getting Started

### 1. Install the Package

**Via Package Manager (Local):**
1. Open Unity Package Manager (Window > Package Manager)
2. Click "+" > "Add package from disk"
3. Navigate to this folder and select `package.json`

### 2. Basic Usage

The most common use case is creating ScriptableObjects that need runtime state:

```csharp
using HelloDev.Utils;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Stats")]
public class PlayerStats_SO : RuntimeScriptableObject
{
    [SerializeField] private int maxHealth = 100;

    // Runtime state (not serialized, resets each play session)
    private int _currentHealth;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;

    protected override void OnScriptableObjectReset()
    {
        // Called automatically when entering play mode
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Max(0, _currentHealth - amount);
    }
}
```

### 3. Using Safe Event Extensions

```csharp
using HelloDev.Utils;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityEvent<int> onScoreChanged;

    void Start()
    {
        // Safe subscribe - prevents duplicate subscriptions
        onScoreChanged.SafeSubscribe(HandleScoreChanged);
    }

    void OnDestroy()
    {
        // Null-safe unsubscribe
        onScoreChanged.SafeUnsubscribe(HandleScoreChanged);
    }

    void AddScore(int points)
    {
        // Null-safe invoke
        onScoreChanged.SafeInvoke(points);
    }

    void HandleScoreChanged(int newScore) { }
}
```

## Installation

### Via Package Manager (Local)
1. Open Unity Package Manager
2. Click "+" > "Add package from disk"
3. Navigate to this folder and select `package.json`

## Usage

### RuntimeScriptableObject

Base class for ScriptableObjects that need state reset when entering play mode:

```csharp
using HelloDev.Utils;

public class GameState_SO : RuntimeScriptableObject
{
    private int cachedScore;

    protected override void OnScriptableObjectReset()
    {
        cachedScore = 0; // Reset on play mode enter
    }
}

// Access the description field
string desc = myScriptableObject.Description;
```

### Safe Event Extensions

Null-safe event operations that prevent duplicate subscriptions:

```csharp
using HelloDev.Utils;
using UnityEngine.Events;

// Works with UnityEvent, UnityEvent<T>, up to UnityEvent<T0,T1,T2,T3>

// Null-safe invoke
myEvent.SafeInvoke();
myIntEvent.SafeInvoke(42);
myTwoParamEvent.SafeInvoke(arg1, arg2);
myFourParamEvent.SafeInvoke(a, b, c, d);

// Subscribe (removes old subscription first, prevents duplicates)
myEvent.SafeSubscribe(MyHandler);

// Unsubscribe (null-safe)
myEvent.SafeUnsubscribe(MyHandler);
```

### Transform Extensions

```csharp
using HelloDev.Utils;

// Destroy all children of a transform (zero allocation, uses reverse iteration)
transform.DestroyAllChildren();

// For editor scripts (uses DestroyImmediate)
transform.DestroyAllChildren(immediate: true);

// Destroy only children matching a condition
transform.DestroyAllChildren(child => child.CompareTag("Enemy"));
transform.DestroyAllChildren(child => child.name.StartsWith("Temp_"), immediate: true);

// Also works on GameObjects directly
gameObject.DestroyAllChildren();
gameObject.DestroyAllChildren(child => child.gameObject.activeSelf == false);
```

### Bootstrap Integration

For controlled initialization order across multiple systems:

```csharp
using HelloDev.Utils;
using System.Threading.Tasks;

public class MyManager : MonoBehaviour, IBootstrapInitializable
{
    [SerializeField] private bool selfInitialize = true;

    private bool _isInitialized;

    // IBootstrapInitializable implementation
    public bool SelfInitialize => selfInitialize;
    public int InitializationPriority => 150; // Game systems layer
    public bool IsInitialized => _isInitialized;

    void OnEnable()
    {
        if (selfInitialize && !_isInitialized)
            _ = InitializeAsync();
    }

    public Task InitializeAsync()
    {
        if (_isInitialized) return Task.CompletedTask;

        // Initialize your system here
        _isInitialized = true;
        return Task.CompletedTask;
    }

    public void Shutdown()
    {
        _isInitialized = false;
    }
}
```

**Priority Ranges:**
| Range | Category | Examples |
|-------|----------|----------|
| 0-99 | Core Services | Logging, Analytics, Input |
| 100-149 | Data Layer | WorldFlags, EventSystem |
| 150-199 | Game Systems | Quests, Inventory |
| 200-249 | Persistence | SaveManager |
| 250-299 | Data Loading | Load saves, restore state |
| 300+ | Gameplay | UI, Audio |

### Locator Base

Base class for locator ScriptableObjects that provide decoupled access to managers:

```csharp
using HelloDev.Utils;
using UnityEngine;

[CreateAssetMenu(menuName = "HelloDev/Locators/My Locator")]
public class MyLocator_SO : LocatorBase_SO
{
    private MyManager _manager;

    public override string LocatorId => "HelloDev.MySystem";
    public override bool IsAvailable => _manager != null;

    public void Register(MyManager manager) => _manager = manager;
    public void Unregister(MyManager manager)
    {
        if (_manager == manager) _manager = null;
    }

    // Locator methods
    public void DoSomething()
    {
        if (!IsAvailable) return;
        _manager.DoSomething();
    }
}
```

## Dependencies

None - this is the foundation package.

## API Reference

### RuntimeScriptableObject
| Member | Description |
|--------|-------------|
| `Description` | Read-only property exposing the inspector description field |
| `OnScriptableObjectReset()` | Abstract method called before first scene loads; override to reset runtime state |

### TransformExtensions
| Method | Description |
|--------|-------------|
| `DestroyAllChildren(bool immediate)` | Destroys all children using zero-allocation reverse iteration |
| `DestroyAllChildren(Predicate<Transform>, bool immediate)` | Destroys children matching condition |

### UnityEventExtensions
| Method | Description |
|--------|-------------|
| `SafeInvoke(...)` | Null-safe event invocation (0-4 parameters) |
| `SafeSubscribe(...)` | Subscribe with duplicate prevention |
| `SafeUnsubscribe(...)` | Null-safe unsubscribe |

### IBootstrapInitializable
| Member | Description |
|--------|-------------|
| `SelfInitialize` | True = self-init in Unity lifecycle, False = wait for bootstrap |
| `InitializationPriority` | Sort order for initialization (lower = earlier) |
| `IsInitialized` | Whether initialization is complete |
| `InitializeAsync()` | Async initialization method |
| `Shutdown()` | Cleanup method |

### LocatorBase_SO
| Member | Description |
|--------|-------------|
| `LocatorId` | Unique identifier (e.g., "HelloDev.MySystem") |
| `IsAvailable` | Whether the locator is ready to use |
| `PrepareForBootstrap()` | Called before bootstrap begins |
| `OnBootstrapComplete()` | Called after all systems initialize |
| `OnBootstrapShutdown()` | Called during shutdown |

## Changelog

### v1.1.0 (2025-12-21)
**Performance:**
- `DestroyAllChildren` now uses zero-allocation reverse iteration instead of creating a temporary List
- `RuntimeScriptableObject.Instances` changed from `List` to `HashSet` for O(1) add/remove

**Robustness:**
- Added null check in `ResetInstances()` to handle destroyed ScriptableObjects

**API Improvements:**
- Renamed `Unsubscribe` to `SafeUnsubscribe` for consistency with other Safe* methods
- Added 4-parameter `UnityEvent<T0,T1,T2,T3>` support
- Added conditional `DestroyAllChildren(Predicate<Transform>)` overload
- Added `Description` property to `RuntimeScriptableObject`

**Documentation:**
- Added XML documentation to all public classes and methods
- Removed unnecessary `#region` wrappers

**Package:**
- Updated Unity version requirement to 6000.3

### v1.0.0
- Initial release

## License

MIT License
