using Common.Interfaces.Helpers;
using System.IO;

namespace Common.Loggers
{
    /// <summary>
    /// Class providing logging operations to files.
    /// </summary>
    public class FileLogger : AbstractLogger, ILogger
    {
        string _file;


        /// <summary>
        /// Constructor.
        /// </summary>
        public FileLogger(string file)
        {
            _file = file;
        }


        /// <summary>
        /// Writes the messesage into the file.
        /// </summary>
        public override void Log(string message)
        {
            File.AppendAllText(_file, CreateLog(message).ToString());
        }
    }
}
