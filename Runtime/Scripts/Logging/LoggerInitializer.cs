using System.Threading.Tasks;
using HelloDev.Utils;
using UnityEngine;

namespace HelloDev.Logging
{
    /// <summary>
    /// Initializes the Logger from a LoggerSettings_SO asset.
    /// Runs at DefaultExecutionOrder(-2000) to initialize before GameBootstrap (-1000).
    /// </summary>
    /// <remarks>
    /// This component implements IBootstrapInitializable for consistency with the bootstrap system,
    /// but always self-initializes since the logger must be available before bootstrap runs.
    /// </remarks>
    [DefaultExecutionOrder(-2000)]
    public class LoggerInitializer : MonoBehaviour, IBootstrapInitializable
    {
        [SerializeField]
        [Tooltip("The logger settings asset containing system configurations.")]
        private LoggerSettings_SO settings;

        private bool _isInitialized;

        #region IBootstrapInitializable

        /// <summary>
        /// Always true - logger must initialize before bootstrap.
        /// </summary>
        public bool SelfInitialize => true;

        /// <summary>
        /// Priority 0 - Core Services (earliest possible).
        /// </summary>
        public int InitializationPriority => 0;

        /// <summary>
        /// Whether the logger has been initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Called by bootstrap if discovered. Already initialized in Awake.
        /// </summary>
        public Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears all registered systems on shutdown.
        /// </summary>
        public void Shutdown()
        {
            Logger.ClearAllSystems();
            _isInitialized = false;
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (_isInitialized) return;
            Initialize();
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            if (settings == null)
            {
                Debug.LogWarning("[LoggerInitializer] No LoggerSettings_SO assigned. Logger will use defaults.");
                return;
            }

            settings.ApplyToLogger();
            _isInitialized = true;
        }

        #endregion
    }
}
