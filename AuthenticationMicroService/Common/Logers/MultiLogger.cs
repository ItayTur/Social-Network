using Common.Interfaces.Helpers;

namespace Common.Loggers
{
    /// <summary>
    /// Class providing multilogger operation.
    /// </summary>
    public class MultiLogger : ILogger
    {
        ILogger[] _loggers;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="loggers"></param>
        public MultiLogger(params ILogger[] loggers)
        {
            _loggers = loggers;
        }


        /// <summary>
        /// Writes to multi logs.
        /// </summary>
        public void Log(string message)
        {
            foreach (ILogger logger in _loggers)
            {
                if (logger != null)
                {
                    logger.Log(message);
                }
            }
        }
    }
}
