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
    public class UnitTest6
    {

        private static readonly float SIGNAL_APPLIANCE_TOLERANCE = 5.2f;
        private static readonly float SIGNAL_APPLIANCE_SET_POINT = 10.0f;

        [TestMethod]
        [DeploymentItem("hibernate.cfg.xml")]
        public void CanCreateSignalWithTolerance()
        {
            IRepository<Area> repoT = new AreaRepository();
            Area area = new Area();
            area = repoT.GetById(1);

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

            IRepository<SignalAppliance> repoC = new SignalApplianceRepository();
            SignalAppliance signalAppliance = new SignalAppliance();
            signalAppliance.Signal = signal;
            signalAppliance.Appliance = appliance;
            signalAppliance.Tolerance = SIGNAL_APPLIANCE_TOLERANCE;

            appliance.Signals.Add(signalAppliance);

            repoB.Save(appliance);

            SignalAppliance createdSignalAppliance = repoC.GetById(signalAppliance.Id);

            Assert.IsNotNull(createdSignalAppliance);

            Assert.AreEqual(signalAppliance, createdSignalAppliance);

            repoC.Delete(signalAppliance.Id);

            repoB.Delete(appliance.Id);

            repoA.Delete(signal.Id);
        }

        [TestMethod]
        [DeploymentItem("hibernate.cfg.xml")]
        public void CanCreateSignalWithToleranceAndAssociatedSignalApplianceValues()
        {
            IRepository<Area> repoT = new AreaRepository();
            Area area = new Area();
            area = repoT.GetById(1);
            IRepository<AlarmType> repoAT = new AlarmTypeRepository();
            AlarmType normalAlarm = new AlarmType();
            normalAlarm = repoAT.GetById(1);
            AlarmType highAlarm = new AlarmType();
            highAlarm = repoAT.GetById(2);
            AlarmType lowAlarm = new AlarmType();
            lowAlarm = repoAT.GetById(3);

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

            IRepository<SignalAppliance> repoSAppl = new SignalApplianceRepository();
            SignalAppliance signalAppliance = new SignalAppliance();
            signalAppliance.Signal = signal;
            signalAppliance.Appliance = appliance;
            signalAppliance.Tolerance = SIGNAL_APPLIANCE_TOLERANCE;

            appliance.Signals.Add(signalAppliance);

            IRepository<SignalApplianceValue> repoSApplV = new SignalApplianceValueRepository();
            SignalApplianceValue normalValue = new SignalApplianceValue();
            normalValue.AlarmType = normalAlarm;
            normalValue.SignalAppliance = signalAppliance;
            normalValue.Value = SIGNAL_APPLIANCE_SET_POINT;

            SignalApplianceValue highValue = new SignalApplianceValue();
            highValue.AlarmType = highAlarm;
            highValue.SignalAppliance = signalAppliance;
            highValue.Value = SIGNAL_APPLIANCE_SET_POINT + SIGNAL_APPLIANCE_TOLERANCE;

            SignalApplianceValue lowValue = new SignalApplianceValue();
            lowValue.AlarmType = lowAlarm;
            lowValue.SignalAppliance = signalAppliance;
            lowValue.Value = SIGNAL_APPLIANCE_SET_POINT - SIGNAL_APPLIANCE_TOLERANCE;

            signalAppliance.SignalApplianceValues.Add(normalValue);
            signalAppliance.SignalApplianceValues.Add(highValue);
            signalAppliance.SignalApplianceValues.Add(lowValue);

            repoB.Save(appliance);

            SignalApplianceValue createdNormalValue = repoSApplV.GetById(normalValue.Id);
            Assert.AreEqual(normalValue.Value, createdNormalValue.Value);

            SignalApplianceValue createdHighValue = repoSApplV.GetById(highValue.Id);
            Assert.AreEqual(highValue.Value, createdHighValue.Value);

            SignalApplianceValue createdLowValue = repoSApplV.GetById(lowValue.Id);
            Assert.AreEqual(lowValue.Value, createdLowValue.Value);

            repoSApplV.Delete(normalValue.Id);
            repoSApplV.Delete(highValue.Id);
            repoSApplV.Delete(lowValue.Id);

            repoSAppl.Delete(signalAppliance.Id);

            repoB.Delete(appliance.Id);

            repoA.Delete(signal.Id);
        }

        [TestMethod]
        [DeploymentItem("hibernate.cfg.xml")]
        public void ListSignalAppliancesWithAlarmTypesAndValuesInOneRecord()
        {
            IRepository<SignalAppliance> dbSA = new SignalApplianceRepository();
            IRepository<Signal> dbS = new SignalRepository();
            IRepository<Appliance> dbApp = new ApplianceRepository();
            IRepository<SignalApplianceValue> dbSAppV = new SignalApplianceValueRepository();
            IRepository<AlarmType> dbA = new AlarmTypeRepository();

            var signalAppliances = from sa in dbSA.GetAll()
                                   join s in dbS.GetAll() on sa.Signal.Id equals s.Id
                                   join app in dbApp.GetAll() on sa.Appliance.Id equals app.Id
                                   join sapv in dbSAppV.GetAll() on sa.Id equals sapv.SignalAppliance.Id
                                   join a in dbA.GetAll() on sapv.AlarmType.Id equals a.Id
                                   where sa.Id.ToString().Equals("37d8e6dd-94a9-4ce8-9956-a0a100ffa9fb")
                                   select new { sa.Id, app.NameAppliance, s.Name, sa.Tolerance, a.NameAlarmType, sapv.Value};

            ObjectDumper.Write(signalAppliances);

            Console.WriteLine("*******************");
        }
    }
}
