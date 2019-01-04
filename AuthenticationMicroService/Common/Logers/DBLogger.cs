using Common.Interfaces.Helpers;

namespace Common.Loggers
{
    /// <summary>
    /// Class providing logging operations for the DB.
    /// </summary>
    public class DBLogger : AbstractLogger, ILogger
    {
        ILogsRepository _logsRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        public DBLogger(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }


        /// <summary>
        /// Create a log instance out of the message.
        /// </summary>
        public override void Log(string message)
        {
            try
            {
                _logsRepository.Add(CreateLog(message));
            }
            catch
            {
                //if db connection is the error it cant be logged to db
            }

        }
    }
}
