using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMCLCore.Domain.Repositories;
using SMCLCore;
using SMCLCore.Domain.Model;

namespace SMCL.Tests
{
    [TestClass]
    public class UnitTest4
    {
        public UnitTest4()
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
        public void CanCreateSignalApplianceAndValues()
        {
            IRepository<Area> repoT = new AreaRepository();
            Area area = new Area();
            area = repoT.GetById(1);
            IRepository<AlarmType> repoAT = new AlarmTypeRepository();
            AlarmType alarm = new AlarmType();
            alarm = repoAT.GetById(1);

            IRepository<Signal> repoA = new SignalRepository();
            Signal signal = new Signal();
            signal.Name = "PruebaSenal";
            signal.Description = "Prueba descriptiva senal";

            repoA.Save(signal);

            IRepository<Appliance> repoB = new ApplianceRepository();
            Appliance appliance = new Appliance();
            appliance.NameAppliance = "PruebaAppliance";
            appliance.Description = "Prueba descriptiva appliance";
            appliance.Area = area;

            SignalAppliance signalAppliance = new SignalAppliance();
            signalAppliance.Signal = signal;
            signalAppliance.Appliance = appliance;

            appliance.Signals.Add(signalAppliance);

            repoB.Save(appliance);


            IRepository<SignalApplianceValue> repoSAV = new SignalApplianceValueRepository();
            SignalApplianceValue signalApplianceValue = new SignalApplianceValue();
            signalApplianceValue.Value = 40;
            signalApplianceValue.SignalAppliance = signalAppliance;
            signalApplianceValue.AlarmType = alarm;

            repoSAV.Save(signalApplianceValue);
        }
    }
}
