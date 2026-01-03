using System.Collections.Generic;
using UnityEngine;

namespace HelloDev.Logging
{
    /// <summary>
    /// System ID constants for centralized logging.
    /// Use these with Logger methods: Logger.LogWarning(LogSystems.Save, "message")
    /// </summary>
    public static class LogSystems
    {
        // Core HelloDev systems
        public const string Bootstrap = "Bootstrap";
        public const string Save = "Save";
        public const string SaveSetup = "SaveSetup";
        public const string Tween = "Tween";
        public const string UI = "UI";
        public const string WorldFlags = "WorldFlags";
        public const string Conditions = "Conditions";
    }

    /// <summary>
    /// Centralized logging system for all HelloDev packages.
    /// Supports system registration, per-system enable/disable, and semantic logging.
    ///
    /// Usage: Logger.LogWarning(LogSystems.Save, "No provider configured.");
    ///
    /// Each package should create its own logger helper that self-registers its systems.
    /// See QuestLogger for an example pattern.
    /// </summary>
    public static class Logger
    {
        #region State

        private static readonly Dictionary<string, LogSystemConfig> _systems = new();
        private static readonly HashSet<string> _disabledSystems = new();

        // Note: Systems are now configured via LoggerSettings_SO and registered by LoggerInitializer.
        // The static constructor is removed to allow external configuration.

        #endregion

        #region Global Toggles

        /// <summary>Master toggle for all logging. When false, no logs are output.</summary>
        public static bool IsEnabled { get; set; } = true;

        /// <summary>Verbose logging toggle. When false, LogVerbose calls are skipped.</summary>
        public static bool IsVerboseEnabled { get; set; } = true;

        #endregion

        #region Icons (Unicode)

        private const string IconUpdate = "\u2022";     // Bullet (standard log)
        private const string IconStart = "\u25B6";      // Play
        private const string IconComplete = "\u2713";   // Check
        private const string IconFail = "\u2717";       // Cross
        private const string IconTransition = "\u2192"; // Arrow
        private const string IconWarning = "!";
        private const string IconError = "X";

        #endregion

        #region System Registration

        /// <summary>
        /// Registers a logging system with the Logger.
        /// Call this once per system, typically in a static constructor or EnsureRegistered pattern.
        /// </summary>
        /// <param name="systemId">Unique identifier for the system (e.g., "Bootstrap", "Quest").</param>
        /// <param name="color">Hex color for the tag (e.g., "#4ECDC4").</param>
        /// <param name="tagName">Optional display name. Defaults to systemId if not provided.</param>
        public static void RegisterSystem(string systemId, string color, string tagName = null)
        {
            if (string.IsNullOrEmpty(systemId)) return;

            _systems[systemId] = new LogSystemConfig(systemId, color, tagName ?? systemId);
        }

        /// <summary>
        /// Unregisters a logging system.
        /// </summary>
        /// <param name="systemId">The system ID to unregister.</param>
        public static void UnregisterSystem(string systemId)
        {
            if (string.IsNullOrEmpty(systemId)) return;

            _systems.Remove(systemId);
            _disabledSystems.Remove(systemId);
        }

        /// <summary>
        /// Checks if a system is registered.
        /// </summary>
        /// <param name="systemId">The system ID to check.</param>
        /// <returns>True if the system is registered.</returns>
        public static bool IsSystemRegistered(string systemId)
        {
            return !string.IsNullOrEmpty(systemId) && _systems.ContainsKey(systemId);
        }

        #endregion

        #region Per-System Enable/Disable

        /// <summary>
        /// Enables or disables logging for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="enabled">Whether the system should be enabled.</param>
        public static void SetSystemEnabled(string systemId, bool enabled)
        {
            if (string.IsNullOrEmpty(systemId)) return;

            if (enabled)
                _disabledSystems.Remove(systemId);
            else
                _disabledSystems.Add(systemId);
        }

        /// <summary>
        /// Checks if logging is enabled for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <returns>True if the system is enabled (or not explicitly disabled).</returns>
        public static bool IsSystemEnabled(string systemId)
        {
            if (string.IsNullOrEmpty(systemId)) return false;
            return !_disabledSystems.Contains(systemId);
        }

        /// <summary>
        /// Enables or disables all registered systems.
        /// </summary>
        /// <param name="enabled">Whether all systems should be enabled.</param>
        public static void SetAllSystemsEnabled(bool enabled)
        {
            if (enabled)
            {
                _disabledSystems.Clear();
            }
            else
            {
                foreach (var systemId in _systems.Keys)
                {
                    _disabledSystems.Add(systemId);
                }
            }
        }

        /// <summary>
        /// Gets all registered system IDs.
        /// </summary>
        /// <returns>Collection of registered system IDs.</returns>
        public static IEnumerable<string> GetRegisteredSystems()
        {
            return _systems.Keys;
        }

        /// <summary>
        /// Clears all registered systems and disabled states.
        /// Called by LoggerSettings_SO.ApplyToLogger() before registering systems.
        /// </summary>
        public static void ClearAllSystems()
        {
            _systems.Clear();
            _disabledSystems.Clear();
        }

        #endregion

        #region Standard Logging

        /// <summary>
        /// Logs an info message for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(string systemId, string message)
        {
            if (!ShouldLog(systemId)) return;
            Debug.Log(FormatMessage(systemId, IconUpdate, message));
        }

        /// <summary>
        /// Logs an info message for a specific system with Unity Object context.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="context">Unity Object context for clickable reference.</param>
        public static void Log(string systemId, string message, Object context)
        {
            if (!ShouldLog(systemId)) return;
            Debug.Log(FormatMessage(systemId, IconUpdate, message), context);
        }

        /// <summary>
        /// Logs a warning message for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string systemId, string message)
        {
            if (!ShouldLog(systemId)) return;
            Debug.LogWarning(FormatMessage(systemId, IconWarning, message));
        }

        /// <summary>
        /// Logs an error message for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        public static void LogError(string systemId, string message)
        {
            if (!ShouldLog(systemId)) return;
            Debug.LogError(FormatMessage(systemId, IconError, message));
        }

        /// <summary>
        /// Logs a verbose message (only when IsVerboseEnabled is true).
        /// Uses the configured system color but with a dimmed message.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        public static void LogVerbose(string systemId, string message)
        {
            if (!IsEnabled || !IsVerboseEnabled) return;
            if (!IsSystemEnabled(systemId)) return;

            var config = GetOrCreateConfig(systemId);
            Debug.Log($"<color={config.HexColor}>[{config.TagName}]</color> {message}");
        }

        #endregion

        #region Semantic Logging

        /// <summary>
        /// Logs a start event (entity started).
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="entityType">Type of entity (e.g., "Quest", "Task").</param>
        /// <param name="entityName">Name of the entity.</param>
        public static void LogStart(string systemId, string entityType, string entityName)
        {
            if (!ShouldLog(systemId)) return;
            Debug.Log(FormatMessage(systemId, IconStart, $"{entityType} <b>'{entityName}'</b> started"));
        }

        /// <summary>
        /// Logs a completion event (entity completed).
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="entityType">Type of entity.</param>
        /// <param name="entityName">Name of the entity.</param>
        public static void LogComplete(string systemId, string entityType, string entityName)
        {
            if (!ShouldLog(systemId)) return;
            Debug.Log(FormatMessage(systemId, IconComplete, $"{entityType} <b>'{entityName}'</b> completed"));
        }

        /// <summary>
        /// Logs a failure event (entity failed).
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="entityType">Type of entity.</param>
        /// <param name="entityName">Name of the entity.</param>
        public static void LogFail(string systemId, string entityType, string entityName)
        {
            if (!ShouldLog(systemId)) return;
            Debug.Log(FormatMessage(systemId, IconFail, $"{entityType} <b>'{entityName}'</b> failed"));
        }

        /// <summary>
        /// Logs a transition event (from one state to another).
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="from">The source state/entity.</param>
        /// <param name="to">The target state/entity.</param>
        public static void LogTransition(string systemId, string from, string to)
        {
            if (!ShouldLog(systemId)) return;
            Debug.Log(FormatMessage(systemId, IconTransition, $"<b>'{from}'</b> {IconTransition} <b>'{to}'</b>"));
        }

        #endregion

        #region Internal Helpers

        private static bool ShouldLog(string systemId)
        {
            if (!IsEnabled) return false;
            if (string.IsNullOrEmpty(systemId)) return false;
            return IsSystemEnabled(systemId);
        }

        private static string FormatMessage(string systemId, string icon, string message)
        {
            var config = GetOrCreateConfig(systemId);
            return $"<color={config.HexColor}>[{config.TagName}]</color> {icon} {message}";
        }

        private static LogSystemConfig GetOrCreateConfig(string systemId)
        {
            if (_systems.TryGetValue(systemId, out var config))
                return config;

            // Auto-register with default white color if not registered
            config = new LogSystemConfig(systemId, "#FFFFFF", systemId);
            _systems[systemId] = config;
            return config;
        }

        #endregion
    }
}
