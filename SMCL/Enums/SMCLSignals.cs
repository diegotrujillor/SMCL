using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SMCL.Enums
{
    public static class SMCLSignals
    {
        public readonly static int DifferentialPressure = int.Parse(ConfigurationManager.AppSettings["DifferentialPressure"]);
        public readonly static int Temperature = int.Parse(ConfigurationManager.AppSettings["Temperature"]);
        public readonly static int RH = int.Parse(ConfigurationManager.AppSettings["RH"]);
    }
}