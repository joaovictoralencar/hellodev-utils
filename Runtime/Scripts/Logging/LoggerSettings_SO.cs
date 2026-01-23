using System.Collections.Generic;
using UnityEngine;

namespace HelloDev.Logging
{
    /// <summary>
    /// ScriptableObject containing all logger settings and system configurations.
    /// Configure this in the inspector and assign to LoggerInitializer.
    /// </summary>
    [CreateAssetMenu(fileName = "LoggerSettings", menuName = "HelloDev/Settings/Logger Settings")]
    public class LoggerSettings_SO : ScriptableObject
    {
        [Header("Global Settings")]
        [SerializeField]
        [Tooltip("Master toggle for all logging. When false, no logs are output.")]
        private bool isEnabled = true;

        [SerializeField]
        [Tooltip("Verbose logging toggle. When false, LogVerbose calls are skipped.")]
        private bool isVerboseEnabled = true;

        [Header("Log Systems")]
        [SerializeField]
        [Tooltip("List of logging systems to register. Each system has an ID, color, and enabled state.")]
        private List<LogSystemConfig> systems = new();

        /// <summary>Master toggle for all logging.</summary>
        public bool IsEnabled => isEnabled;

        /// <summary>Verbose logging toggle.</summary>
        public bool IsVerboseEnabled => isVerboseEnabled;

        /// <summary>Read-only access to configured systems.</summary>
        public IReadOnlyList<LogSystemConfig> Systems => systems;

        /// <summary>
        /// Applies all settings to the static Logger.
        /// Called by LoggerInitializer on Awake.
        /// </summary>
        public void ApplyToLogger()
        {
            Logger.ClearAllSystems();
            Logger.IsEnabled = isEnabled;
            Logger.IsVerboseEnabled = isVerboseEnabled;

            foreach (var config in systems)
            {
                if (string.IsNullOrEmpty(config.SystemId)) continue;

                Logger.RegisterSystem(config.SystemId, config.HexColor, config.TagName);
                Logger.SetSystemEnabled(config.SystemId, config.Enabled);
            }
        }
    }
}
