namespace Common.Interfaces.Helpers
{
    /// <summary>
    /// Interface providing the logging opration.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Write the specified message to the log.
        /// </summary>
        void Log(string message);
    }
}
