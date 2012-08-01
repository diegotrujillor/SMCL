using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMCLCore.Domain.Model;
using System.Configuration;
using SMCLCore.Domain.Repositories;
using SMCLCore;

namespace SMCL.Utilities
{
    public static class Security
    {
        public static bool IsOptionInUserRoles(IList<UserRole> roles, string option)
        {
            IRepository<Role> repoRole = new RoleRepository();
            foreach (UserRole item in roles)
            {
                if (ConfigurationManager.AppSettings[repoRole.GetById(item.Role.Id).Name].Contains(option))
                {
                    return true;
                }
            }
            return false;
        }
    }
}