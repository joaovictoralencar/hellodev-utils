using System.Collections.Generic;
using HelloDev.Logging.Editor;
using Sirenix.OdinInspector;
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

        [Header("Code Generation")]
        [SerializeField]
        [FolderPath]
        [Tooltip("Folder inside Assets where the generated constants file will be placed.")]
        private string generatedOutputFolder = "Assets/3rdParties/HelloDev/Scripts/Logging/Generated";

        [SerializeField]
        [ValidateInput("IsValidFileName", "File name cannot be empty or contain invalid characters.")]
        [Tooltip("File name for the generated constants class (without extension).")]
        private string generatedFileName = "LogIds";

        [SerializeField]
        [Tooltip("Namespace for the generated constants class.")]
        private string generatedNamespace = "HelloDev.Logging";

        /// <summary>Full output path for the generated constants file.</summary>
        public string GeneratedOutputPath
        {
            get
            {
                string folder = string.IsNullOrWhiteSpace(generatedOutputFolder)
                    ? "Assets/3rdParties/HelloDev/Scripts/Logging/Generated"
                    : generatedOutputFolder.TrimEnd('/');
                string fileName = string.IsNullOrWhiteSpace(generatedFileName)
                    ? "LogIds"
                    : generatedFileName;
                return $"{folder}/{fileName}.cs";
            }
        }

        /// <summary>Namespace for the generated constants class.</summary>
        public string GeneratedNamespace =>
            string.IsNullOrWhiteSpace(generatedNamespace) ? "HelloDev.Logging" : generatedNamespace;

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
            Logger.IsInitialized = true;

            foreach (var config in systems)
            {
                if (string.IsNullOrEmpty(config.SystemId)) continue;
                Logger.RegisterSystem(config.SystemId, config.HexColor, config.TagName);
                Logger.SetSystemEnabled(config.SystemId, config.Enabled);
            }
        }

        [Button("Generate System ID Constants", ButtonSizes.Medium)]
        [EnableIf("HasSystems")]
        private void GenerateConstants()
        {
#if UNITY_EDITOR
            LoggerIdGenerator.GenerateForSettings(this, GeneratedOutputPath, GeneratedNamespace);
#endif
        }

        private bool HasSystems => systems != null && systems.Count > 0;

#if UNITY_EDITOR
        private bool IsValidFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in value)
            {
                if (System.Array.IndexOf(invalidChars, c) >= 0) return false;
            }
            return true;
        }
        
        void SetSystemEnabled(LogSystemConfig s, bool enabled) => s.Enabled = enabled; 
        
        [Button]
        void EnableAll() => systems.ForEach(s => SetSystemEnabled(s,true));
        
        [Button]
        void DisableAll() => systems.ForEach(s => SetSystemEnabled(s,false));
#endif
    }
}
