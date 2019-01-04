using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Helpers
{
    public interface ILogsRepository:ILogger
    {
        void Add(LogModel log);
    }
}
