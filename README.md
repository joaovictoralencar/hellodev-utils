# HelloDev Utils

Foundation utilities for HelloDev packages.

## Features

- **RuntimeScriptableObject** - Abstract base class that auto-resets ScriptableObject state between play sessions
- **UnityEvent Extensions** - Safe event handling (`SafeInvoke`, `SafeSubscribe`, `SafeUnsubscribe`) with support for 0-4 parameters
- **Transform Extensions** - Transform and GameObject utilities (`DestroyAllChildren` with optional condition filter)

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
