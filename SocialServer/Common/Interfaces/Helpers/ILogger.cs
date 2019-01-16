using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
