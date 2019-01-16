using Common.Interfaces;
using Common.Interfaces.Helpers;
using System;

namespace Common.Loggers
{
    /// <summary>
    /// Class providing loggers creation operation.
    /// </summary>
    public class LoggerFactory
    {

        private static LoggerFactory instance = null;

        private ILogger _dbLogger;
        private ILogger _serverFileLogger;
        private ILogger _DBAndServerFileLogger;

        /// <summary>
        /// initilazes factory instance
        /// </summary>
        /// <param name="logsRepository"></param>
        /// <param name="logFile"></param>
        public static void Init(ILogsRepository logsRepository, string logFile)
        {
            instance = new LoggerFactory(logsRepository, logFile);
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        private LoggerFactory(ILogsRepository logsRepository, string logFile)
        {
            _dbLogger = new DBLogger(logsRepository);
            _serverFileLogger = new FileLogger(logFile);
            _DBAndServerFileLogger = new MultiLogger(_dbLogger, _serverFileLogger);
        }


        /// <summary>
        /// Gets the single instance of the class.
        /// </summary>
        public static LoggerFactory GetInstance()
        {
            if (instance != null)
                return instance;
            else
            {
                throw new ArgumentException("logger factory hasnt been initilaized");
            }
        }


        /// <summary>
        /// Gets the db logger.
        /// </summary>
        public ILogger GetDbLogger()
        {
            return _dbLogger;
        }


        /// <summary>
        /// Gets the file logger.
        /// </summary>
        /// <returns></returns>
        public ILogger GetFileLogger()
        {
            return _serverFileLogger;
        }


        /// <summary>
        /// Gets the combo logger.
        /// </summary>
        /// <returns></returns>
        public ILogger AllLogger()
        {
            return _DBAndServerFileLogger;
        }
    }
}
