namespace HelloDev.Logging
{
    /// <summary>
    /// Configuration data for a registered logging system.
    /// </summary>
    public readonly struct LogSystemConfig
    {
        /// <summary>Unique identifier for this system.</summary>
        public string SystemId { get; }

        /// <summary>Hex color for the tag (e.g., "#4ECDC4").</summary>
        public string Color { get; }

        /// <summary>Display name for the tag in log output.</summary>
        public string TagName { get; }

        public LogSystemConfig(string systemId, string color, string tagName)
        {
            SystemId = systemId;
            Color = color;
            TagName = tagName;
        }
    }
}
