namespace Logfella
{
    /// <summary>
    /// Severity of the log message.
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// (0) The log entry has no assigned severity level.
        /// </summary>
        Default = 0,

        /// <summary>
        /// (100) Debug or trace information.
        /// </summary>
        Debug = 100,

        /// <summary>
        /// (200) Routine information, such as ongoing status or performance.
        /// </summary>
        Info = 200,

        /// <summary>
        /// (300) Normal but significant events, such as start up, shut down, or a configuration change.
        /// </summary>
        Notice = 300,

        /// <summary>
        /// (400) Warning events might cause problems.
        /// </summary>
        Warning = 400,

        /// <summary>
        /// (500) Error events are likely to cause problems.
        /// </summary>
        Error = 500,

        /// <summary>
        /// (600) Critical events cause more severe problems or outages.
        /// </summary>
        Critical = 600,

        /// <summary>
        /// (700) A person must take an action immediately.
        /// </summary>
        Alert = 700,

        /// <summary>
        /// (800) One or more systems are unusable.
        /// </summary>
        Emergency = 800
    }
}