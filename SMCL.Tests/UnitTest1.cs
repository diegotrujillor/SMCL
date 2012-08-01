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
    public class UnitTest1
    {
        public UnitTest1()
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
        public void CanCreateAreaAndAppliance()
        {
            IRepository<Area> repoA = new AreaRepository();
            Area area = new Area();
            area.Name = "PruebaArea";
            area.Description = "Prueba descriptiva area";

            IRepository<Appliance> repoB = new ApplianceRepository();
            Appliance appliance = new Appliance();
            appliance.NameAppliance = "PruebaAppliance";
            appliance.Description = "Prueba descriptiva appliance";
            appliance.Area = area;

            area.Appliances.Add(appliance);

            repoA.Save(area);
        }
    }
}
