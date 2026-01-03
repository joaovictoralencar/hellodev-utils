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

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only: Adds all HelloDev systems for initial setup.
        /// </summary>
        [ContextMenu("Add All HelloDev Systems")]
        private void AddAllSystems()
        {
            systems.Clear();

            // Core HelloDev systems
            systems.Add(new LogSystemConfig("Bootstrap", HexToColor("#FF5252"), "Bootstrap"));
            systems.Add(new LogSystemConfig("Save", HexToColor("#2196F3"), "Save"));
            systems.Add(new LogSystemConfig("SaveSetup", HexToColor("#03A9F4"), "SaveSetup"));
            systems.Add(new LogSystemConfig("Tween", HexToColor("#2ECC71"), "Tween"));
            systems.Add(new LogSystemConfig("UI", HexToColor("#9C27B0"), "UI"));
            systems.Add(new LogSystemConfig("WorldFlags", HexToColor("#AB47BC"), "WorldFlags"));
            systems.Add(new LogSystemConfig("Conditions", HexToColor("#FFCA28"), "Conditions"));

            // Quest system
            systems.Add(new LogSystemConfig("Quest.Manager", HexToColor("#00BCD4"), "Manager"));
            systems.Add(new LogSystemConfig("Quest.Quest", HexToColor("#FFC107"), "Quest"));
            systems.Add(new LogSystemConfig("Quest.Task", HexToColor("#4DB6AC"), "Task"));
            systems.Add(new LogSystemConfig("Quest.TaskGroup", HexToColor("#4DB6AC"), "TaskGroup"));
            systems.Add(new LogSystemConfig("Quest.Stage", HexToColor("#EF5350"), "Stage"));
            systems.Add(new LogSystemConfig("Quest.Group", HexToColor("#7E57C2"), "Group"));
            systems.Add(new LogSystemConfig("Quest.QuestLine", HexToColor("#EC407A"), "QuestLine"));
            systems.Add(new LogSystemConfig("Quest.Save", HexToColor("#2196F3"), "Save"));
            systems.Add(new LogSystemConfig("Quest.SaveManager", HexToColor("#00CED1"), "SaveManager"));
            systems.Add(new LogSystemConfig("Quest.Choice", HexToColor("#E91E63"), "Choice"));
            systems.Add(new LogSystemConfig("Quest.UI", HexToColor("#9C27B0"), "UI"));

            UnityEditor.EditorUtility.SetDirty(this);
        }

        private static Color HexToColor(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }
#endif
    }
}
