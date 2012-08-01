using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;

namespace SMCL.Tests
{
    [TestClass]
    public class UnitTest5
    {
        public UnitTest5()
        {
        }

        [TestMethod]
        [DeploymentItem("hibernate.cfg.xml")]
        public void TestMethod1()
        {
            IRepository<Monitoring> repoM = new MonitoringRepository();
            var res = repoM.GetByProperty("CommentsOnAlarm", null);

            IRepository<Signal> repo1 = new SignalRepository();
            Signal signal = new Signal();
            signal = repo1.GetById(1);
            IRepository<Appliance> repo2 = new ApplianceRepository();
            Appliance appliance = new Appliance();
            appliance = repo2.GetById(1);
            IRepository<AlarmType> repo3 = new AlarmTypeRepository();
            AlarmType alarmType = new AlarmType();
            alarmType = repo3.GetById(1);
            IRepository<User> repo4 = new UserRepository();
            User user = new User();
            user = repo4.GetById(2);



            IRepository<MappingTag> repoMT = new MappingTagRepository();
            MappingTag tag = new MappingTag();
            tag.Tag = "PruebaTag";
            tag.Description = "Prueba descriptiva mapping tag";
            tag.Signal = signal;
            tag.Appliance = appliance;
            tag.AlarmType = alarmType;

            Monitoring monitor = new Monitoring();
            monitor.Value = float.Parse("2.400");
            monitor.DateTime = DateTime.Now;
            monitor.CommentsOnAlarm = "Prueba de comentarios sobre alarma, monitoreo";
            monitor.MappingTag = tag;
            monitor.User = user;

            tag.Monitorings.Add(monitor);

            repoMT.Save(tag);


            /*INSERT 4000 RECORDS for TEST*/
            //IRepository<Monitoring> repoM = new MonitoringRepository();
            //IRepository<User> repo4 = new UserRepository();
            //User user = new User();
            //user = repo4.GetById(6);

            //IRepository<MappingTag> repoMT = new MappingTagRepository();
            //MappingTag tag = new MappingTag();
            //tag = repoMT.GetById(103);

            //for (int i = 0; i < 4000; i++)
            //{
            //    Monitoring monitor = new Monitoring();
            //    monitor.Value = (1000 + i);
            //    monitor.DateTime = DateTime.Now;
            //    monitor.CommentsOnAlarm = null;
            //    monitor.MappingTag = tag;
            //    monitor.User = user;

            //    repoM.Save(monitor);
            //}
        }
    }
}
