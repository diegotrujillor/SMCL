using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SMCL.Enums
{
    public enum EventTypes
    {
        None,
        Create = 1,
        Edit = 2,
        Delete = 3,
        Report = 4,
        Login = 5,
        Logout = 6,
        Account = 7,
        Password = 8,
        Notify = 9
        //Other future events of system, i.e: create users, delete sensitive data, etc.
    }
}