using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ILogsRepository
    {
        /// <summary>
        /// Adds a log to the database.
        /// </summary>
        /// <param name="log"></param>
        void Add(LogModel log);
    }
}
