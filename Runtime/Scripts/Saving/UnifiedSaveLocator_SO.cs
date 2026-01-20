using System.Threading.Tasks;
using HelloDev.Utils;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace HelloDev.Saving
{
    /// <summary>
    /// ScriptableObject locator for UnifiedSaveManager.
    /// Acts as a "channel" that any asset can reference to access unified save/load functionality.
    /// The UnifiedSaveManager registers itself with this locator on enable.
    ///
    /// Usage:
    /// 1. Create a single UnifiedSaveLocator_SO asset in your project
    /// 2. Assign it to UnifiedSaveManager's "Locator" field
    /// 3. Reference the same asset anywhere you need save/load access
    /// </summary>
    [CreateAssetMenu(fileName = "UnifiedSaveLocator", menuName = "HelloDev/Locators/Unified Save Locator")]
    public class UnifiedSaveLocator_SO : LocatorBase_SO
    {
        #region LocatorBase_SO Implementation

        /// <inheritdoc/>
        public override string LocatorId => "HelloDev.Saving.Unified";

        /// <inheritdoc/>
        public override bool IsAvailable => _manager != null;

        /// <inheritdoc/>
        public override void PrepareForBootstrap()
        {
            // Manager will re-register during bootstrap
            _manager = null;
        }

        #endregion

        #region Private Fields

        private UnifiedSaveManager _manager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the registered manager instance.
        /// </summary>
        public UnifiedSaveManager Manager => _manager;

        /// <summary>
        /// Gets whether a save provider has been configured via SaveService.SetProvider().
        /// </summary>
        public bool HasProvider => SaveService.IsConfigured;

        /// <summary>
        /// Gets the save system settings from the manager.
        /// </summary>
        public SaveSystemSettings_SO Settings => _manager?.Settings;

        #endregion

        #region Events

        /// <summary>
        /// Fired when a manager registers with this locator.
        /// </summary>
        [System.NonSerialized]
        public UnityEvent OnManagerRegistered = new();

        /// <summary>
        /// Fired when a manager unregisters from this locator.
        /// </summary>
        [System.NonSerialized]
        public UnityEvent OnManagerUnregistered = new();

        /// <summary>
        /// Fired before a save operation starts.
        /// </summary>
        [System.NonSerialized]
        public UnityEvent<string> OnBeforeSave = new();

        /// <summary>
        /// Fired after a save operation completes.
        /// </summary>
        [System.NonSerialized]
        public UnityEvent<string, bool> OnAfterSave = new();

        /// <summary>
        /// Fired before a load operation starts.
        /// </summary>
        [System.NonSerialized]
        public UnityEvent<string> OnBeforeLoad = new();

        /// <summary>
        /// Fired after a load operation completes.
        /// </summary>
        [System.NonSerialized]
        public UnityEvent<string, bool> OnAfterLoad = new();

        #endregion

        #region Registration

        /// <summary>
        /// Registers a UnifiedSaveManager with this locator.
        /// Called by UnifiedSaveManager.OnEnable().
        /// </summary>
        public void Register(UnifiedSaveManager manager)
        {
            if (manager == null) return;

            if (_manager != null && _manager != manager)
            {
                Debug.LogWarning($"[UnifiedSaveLocator] Replacing existing manager. Old: {_manager.name}, New: {manager.name}");
            }

            _manager = manager;
            OnManagerRegistered?.Invoke();
        }

        /// <summary>
        /// Unregisters a UnifiedSaveManager from this locator.
        /// Called by UnifiedSaveManager.OnDisable().
        /// </summary>
        public void Unregister(UnifiedSaveManager manager)
        {
            if (_manager == manager)
            {
                _manager = null;
                OnManagerUnregistered?.Invoke();
            }
        }

        #endregion

        #region System Registration (Delegate)

        /// <summary>
        /// Registers a saveable system with the manager.
        /// </summary>
        /// <param name="system">The system to register.</param>
        public void RegisterSystem(ISaveableSystem system)
        {
            _manager?.RegisterSystem(system);
        }

        /// <summary>
        /// Unregisters a saveable system from the manager.
        /// </summary>
        /// <param name="system">The system to unregister.</param>
        public void UnregisterSystem(ISaveableSystem system)
        {
            _manager?.UnregisterSystem(system);
        }

        #endregion

        #region Save/Load Operations (Delegate)

        /// <summary>
        /// Saves all systems to the specified slot.
        /// </summary>
        /// <param name="slotKey">The save slot identifier.</param>
        /// <returns>True if save was successful.</returns>
        public async Task<bool> SaveAsync(string slotKey)
        {
            if (_manager == null)
            {
                Debug.LogWarning("[UnifiedSaveLocator] No manager registered. Cannot save.");
                return false;
            }
            return await _manager.SaveAsync(slotKey);
        }

        /// <summary>
        /// Loads all systems from the specified slot.
        /// </summary>
        /// <param name="slotKey">The save slot identifier.</param>
        /// <returns>True if load was successful.</returns>
        public async Task<bool> LoadAsync(string slotKey)
        {
            if (_manager == null)
            {
                Debug.LogWarning("[UnifiedSaveLocator] No manager registered. Cannot load.");
                return false;
            }
            return await _manager.LoadAsync(slotKey);
        }

        /// <summary>
        /// Checks if a save slot exists.
        /// </summary>
        /// <param name="slotKey">The save slot identifier.</param>
        /// <returns>True if the slot exists.</returns>
        public async Task<bool> SaveExistsAsync(string slotKey)
        {
            if (_manager == null) return false;
            return await _manager.SaveExistsAsync(slotKey);
        }

        /// <summary>
        /// Deletes a save slot.
        /// </summary>
        /// <param name="slotKey">The save slot identifier.</param>
        /// <returns>True if deletion was successful.</returns>
        public async Task<bool> DeleteSaveAsync(string slotKey)
        {
            if (_manager == null) return false;
            return await _manager.DeleteSaveAsync(slotKey);
        }

        /// <summary>
        /// Gets metadata for a save slot without loading the full snapshot.
        /// </summary>
        /// <param name="slotKey">The save slot identifier.</param>
        /// <returns>The metadata, or null if not found.</returns>
        public async Task<UnifiedSnapshotMetadata> GetMetadataAsync(string slotKey)
        {
            if (_manager == null) return null;
            return await _manager.GetMetadataAsync(slotKey);
        }

        #endregion

        #region Snapshot Operations (Delegate)

        /// <summary>
        /// Captures the current state of all systems without saving to storage.
        /// </summary>
        /// <returns>A unified snapshot.</returns>
        public UnifiedSnapshot CaptureSnapshot()
        {
            return _manager?.CaptureUnifiedSnapshot();
        }

        /// <summary>
        /// Restores all systems from a snapshot without loading from storage.
        /// </summary>
        /// <param name="snapshot">The snapshot to restore.</param>
        /// <returns>True if restoration succeeded.</returns>
        public bool RestoreSnapshot(UnifiedSnapshot snapshot)
        {
            if (_manager == null) return false;
            return _manager.RestoreUnifiedSnapshot(snapshot);
        }

        #endregion

        #region Debug

#if ODIN_INSPECTOR && UNITY_EDITOR
        [Title("Debug")]
        [ShowInInspector, ReadOnly]
        [PropertyOrder(100)]
        private bool ManagerRegistered => IsAvailable;

        [ShowInInspector, ReadOnly]
        [PropertyOrder(101)]
        private string ManagerName => _manager != null ? _manager.name : "(none)";

        [ShowInInspector, ReadOnly]
        [PropertyOrder(102)]
        private bool ProviderConfigured => SaveService.IsConfigured;

        [ShowInInspector, ReadOnly]
        [PropertyOrder(103)]
        private int RegisteredSystems => _manager?.RegisteredSystems.Count ?? 0;
#endif

        #endregion
    }
}
