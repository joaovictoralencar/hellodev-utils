using System.Collections.Generic;
using UnityEngine;

namespace HelloDev.Logging
{
    /// <summary>
    /// Centralized logging system for all HelloDev packages.
    /// Each package should create its own logger helper that self-registers its systems.
    /// </summary>
    public static class Logger
    {
        #region State

        private static readonly Dictionary<string, LogSystemConfig> _systems = new();
        
        private static bool _isEnabled;
        
        #endregion

        #region Global Toggles
        
        public static bool IsInitialized { get; set; }


        /// <summary>Master toggle for all logging. When false, no logs are output.</summary>
        public static bool IsEnabled { get => !IsInitialized || _isEnabled; set => _isEnabled = value; }

        /// <summary>Verbose logging toggle. When false, LogVerbose calls are skipped.</summary>
        public static bool IsVerboseEnabled { get; set; } = true;

        #endregion

        #region Icons (Unicode)

        private const string IconUpdate = "\u2022";
        private const string IconWarning = "\u26A0";
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
            _systems[systemId].Enabled = enabled;
        }

        /// <summary>
        /// Checks if logging is enabled for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <returns>True if the system is enabled (or not explicitly disabled).</returns>
        public static bool IsSystemEnabled(string systemId)
        {
            if (string.IsNullOrEmpty(systemId)) return false;
            if (!IsInitialized || _systems.ContainsKey(systemId)) return !IsInitialized || _systems[systemId].Enabled;
            // Fallback: auto-register with default color based on severity
            Debug.LogWarning($"[Logger] System '{systemId}' is not registered. Auto-registering with default color. " +
                             "Consider adding it to LoggerSettings_SO and regenerating constants.");
            return true;
        }

        /// <summary>
        /// Enables or disables all registered systems.
        /// </summary>
        /// <param name="enabled">Whether all systems should be enabled.</param>
        public static void SetAllSystemsEnabled(bool enabled)
        {
            foreach (var kv in _systems) { kv.Value.Enabled = enabled; }
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
            Debug.LogWarning(FormatMessage(systemId, IconWarning, message, LogSeverity.Warning));
        }

        /// <summary>
        /// Logs a warning message for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="context">Unity Object context for clickable reference.</param>
        public static void LogWarning(string systemId, string message, Object context)
        {
            if (!ShouldLog(systemId)) return;
            Debug.LogWarning(FormatMessage(systemId, IconWarning, message, LogSeverity.Warning), context);
        }

        /// <summary>
        /// Logs an error message for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        public static void LogError(string systemId, string message)
        {
            if (!ShouldLog(systemId)) return;
            Debug.LogError(FormatMessage(systemId, IconError, message, LogSeverity.Error));
        }

        /// <summary>
        /// Logs an error message for a specific system.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="context">Unity Object context for clickable reference.</param>
        public static void LogError(string systemId, string message, Object context)
        {
            if (!ShouldLog(systemId)) return;
            Debug.LogError(FormatMessage(systemId, IconError, message, LogSeverity.Error), context);
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

        /// <summary>
        /// Logs a verbose message (only when IsVerboseEnabled is true).
        /// Uses the configured system color but with a dimmed message.
        /// </summary>
        /// <param name="systemId">The system ID.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="context">Unity Object context for clickable reference.</param>
        public static void LogVerbose(string systemId, string message, Object context)
        {
            if (!IsEnabled || !IsVerboseEnabled) return;
            if (!IsSystemEnabled(systemId)) return;

            var config = GetOrCreateConfig(systemId);
            Debug.Log($"<color={config.HexColor}>[{config.TagName}]</color> {message}", context);
        }

        #endregion

        #region Internal Helpers

        private static bool ShouldLog(string systemId)
        {
            string parentId = GetParentSystemId(systemId);
            if (!IsEnabled) return false;
            if (string.IsNullOrEmpty(systemId)) return false;
            return IsSystemEnabled(string.IsNullOrEmpty(parentId) ? systemId : parentId);
        }

        private static string FormatMessage(string systemId, string icon, string message, LogSeverity severity = LogSeverity.Log)
        {
            var config = GetOrCreateConfig(systemId, severity);
            return $"<color={config.HexColor}>[{config.TagName}]</color> {icon} {message}";
        }

        private static LogSystemConfig GetOrCreateConfig(string systemId, LogSeverity severity = LogSeverity.Log)
        {
            // Try exact match first
            if (_systems.TryGetValue(systemId, out var config))
                return config;

            // Try to find parent system (e.g., "Battle.Player" -> "Battle")
            string parentId = GetParentSystemId(systemId);
            if (parentId != null && _systems.TryGetValue(parentId, out var parentConfig))
            {
                // Create child config that inherits parent's color and tag
                var childConfig = new LogSystemConfig(systemId, parentConfig.HexColor, systemId);
                _systems[systemId] = childConfig;
                return childConfig;
            }

            // Fallback: auto-register with default color based on severity
            Debug.LogWarning($"[Logger] System '{systemId}' is not registered. Auto-registering with default color. " +
                             "Consider adding it to LoggerSettings_SO and regenerating constants.");
            string color = severity switch
            {
                LogSeverity.Log => "#FFFFFF",
                LogSeverity.Warning => "#F9A825",
                LogSeverity.Error => "#D32F2F",
                _ => "#FFFFFF"
            };
            config = new LogSystemConfig(systemId, color, systemId);
            _systems[systemId] = config;
            return config;
        }

        /// <summary>
        /// Gets the parent system ID by removing the last dot-separated segment.
        /// Example: "Battle.Player" -> "Battle", "Battle.Skill.Boomguin" -> "Battle.Skill"
        /// </summary>
        /// <param name="systemId">The full system ID.</param>
        /// <returns>The parent system ID, or null if no parent exists.</returns>
        private static string GetParentSystemId(string systemId)
        {
            if (string.IsNullOrEmpty(systemId)) return null;

            int lastDotIndex = systemId.LastIndexOf('.');
            if (lastDotIndex <= 0) return null;

            return systemId.Substring(0, lastDotIndex);
        }

        private enum LogSeverity
        {
            Log,
            Warning,
            Error
        }

        #endregion
    }
}