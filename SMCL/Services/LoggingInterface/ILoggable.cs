using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMCL.Services.LoggingInterface
{    
    interface ILoggable
    {
        int LogSize();
        Object GetNewLog(string LogMessage, int EventID, int UserID);
        void Write(List<Object> LogItems);
    }
}
