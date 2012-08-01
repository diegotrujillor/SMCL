using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;

namespace SMCL.Tests
{
    [TestClass]
    public class UnitTest2
    {
        public UnitTest2()
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
        public void CanCreateAlarmTypeAndLog()
        {
            IRepository<AlarmType> repoA = new AlarmTypeRepository();
            AlarmType alarm = new AlarmType();
            alarm.NameAlarmType = "PruebaAlarma";
            alarm.Description = "Prueba descriptiva alarma";

            repoA.Save(alarm);

            IRepository<User> repoB = new UserRepository();
            User user = new User();
            user = repoB.GetById(1);
            IRepository<Event> repoC = new EventRepository();
            Event eventt = new Event();
            eventt = repoC.GetById(2);

            IRepository<Log> repoD = new LogRepository();
            Log log = new Log();
            log.DateTime = DateTime.Now;
            log.Text = "Prueba descriptiva log";
            log.Event = eventt;
            log.User = user;

            repoD.Save(log);
        }
    }
}
