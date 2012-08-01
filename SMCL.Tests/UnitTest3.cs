using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;
using SMCLCore;

namespace SMCL.Tests
{
    [TestClass]
    public class UnitTest3
    {
        public UnitTest3()
        {
        }

        private TestContext context;

        public TestContext TestContext
        {
            get { return context; }
            set { context = value; }
        }

        [TestMethod]
        [DeploymentItem("hibernate.cfg.xml")]
        public void CanCreateUserAndRole()
        {
            IRepository<Role> repoA = new RoleRepository();
            Role role1 = new Role();
            role1.Name = "PruebaRol1";
            role1.Description = "Prueba descriptiva rol 1";
            Role role2 = new Role();
            role2.Name = "PruebaRol2";
            role2.Description = "Prueba descriptiva rol 2";

            repoA.Save(role1);
            repoA.Save(role2);

            IRepository<User> repoB = new UserRepository();
            User user1 = new User();
            user1.DocumentId = 11111;
            user1.LoginEmail = "usuario@prueba1.com";
            user1.FirstName = "Usuario prueba nombre 1";
            user1.MiddleName = "Usuario prueba nombre 2";
            user1.LastName1 = "Usuario prueba apellido 1";
            user1.LastName2 = "Usuario prueba apellido 2";
            user1.PhoneNumber = "457-4334(90)";
            user1.IsActive = true;
            user1.Password = "6d071901727aec1ba6d8e2497ef5b709";

            UserRole userRole1 = new UserRole();
            userRole1.Role = role1;
            userRole1.User = user1;
            UserRole userRole2 = new UserRole();
            userRole2.Role = role2;
            userRole2.User = user1;

            user1.Roles.Add(userRole1);
            user1.Roles.Add(userRole2);

            repoB.Save(user1);
        }
    }
}
