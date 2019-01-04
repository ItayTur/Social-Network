using Common.Interfaces.Helpers;
using Common.Models;
using System;

namespace Common.Loggers
{
    /// <summary>
    /// Abstract class providing logging operations.
    /// </summary>
    public abstract class AbstractLogger : ILogger
    {
        /// <summary>
        /// Writes the message to the log.
        /// </summary>
        public abstract void Log(string message);


        /// <summary>
        /// Create a log instance out of the message.
        /// </summary>
        protected LogModel CreateLog(string message)
        {
            LogModel log = new LogModel
            {
                Message = message,
                Date = DateTime.Now
            };
            return log;
        }
    }
}
