using System;
using UnityEngine;

namespace HelloDev.Logging
{
    /// <summary>
    /// Configuration data for a registered logging system.
    /// Serializable for inspector configuration in LoggerSettings_SO.
    /// </summary>
    [Serializable]
    public class LogSystemConfig
    {
        [SerializeField] private string systemId;
        [SerializeField] private Color color = Color.white;
        [SerializeField] private string tagName;
        [SerializeField] private bool enabled = true;

        /// <summary>Unique identifier for this system (e.g., "Bootstrap", "Quest").</summary>
        public string SystemId => systemId;

        /// <summary>Hex color for the tag (e.g., "#FF6B6B").</summary>
        public string HexColor => $"#{ColorUtility.ToHtmlStringRGB(color)}";

        /// <summary>Display name for the tag in log output. Defaults to SystemId if empty.</summary>
        public string TagName => string.IsNullOrEmpty(tagName) ? systemId : tagName;

        /// <summary>Whether logging is enabled for this system.</summary>
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        /// <summary>Default constructor for serialization.</summary>
        public LogSystemConfig() { }

        /// <summary>
        /// Creates a new LogSystemConfig with a Color.
        /// </summary>
        /// <param name="systemId">Unique identifier for this system.</param>
        /// <param name="color">Color for the tag.</param>
        /// <param name="tagName">Display name. Defaults to systemId if null.</param>
        /// <param name="enabled">Whether logging is enabled.</param>
        public LogSystemConfig(string systemId, Color color, string tagName = null, bool enabled = true)
        {
            this.systemId = systemId;
            this.color = color;
            this.tagName = tagName ?? systemId;
            this.enabled = enabled;
        }

        /// <summary>
        /// Creates a new LogSystemConfig with a hex color string.
        /// Used internally by Logger.RegisterSystem for programmatic registration.
        /// </summary>
        /// <param name="systemId">Unique identifier for this system.</param>
        /// <param name="hexColor">Hex color string (e.g., "#FF6B6B").</param>
        /// <param name="tagName">Display name. Defaults to systemId if null.</param>
        public LogSystemConfig(string systemId, string hexColor, string tagName)
        {
            this.systemId = systemId;
            ColorUtility.TryParseHtmlString(hexColor, out this.color);
            this.tagName = tagName ?? systemId;
            this.enabled = true;
        }
    }
}
