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
            IRepository<AlarmType> dbAT = new AlarmTypeRepository();
            IRepository<Area> dbA = new AreaRepository();
            
            Area area = new Area();
            area = dbA.GetById(1);
            AlarmType normalAlarm = new AlarmType();
            normalAlarm = dbAT.GetById(1);
            AlarmType highAlarm = new AlarmType();
            highAlarm = dbAT.GetById(2);
            AlarmType lowAlarm = new AlarmType();
            lowAlarm = dbAT.GetById(3);

            Signal signal = new Signal();
            signal.Name = "PruebaSenal";
            signal.Description = "Prueba descriptiva senal";

            dbS.Save(signal);

            Appliance appliance = new Appliance();
            appliance.NameAppliance = "PruebaAppliance";
            appliance.Description = "Prueba descriptiva appliance";
            appliance.Area = area;

            SignalAppliance signalAppliance = new SignalAppliance();
            signalAppliance.Signal = signal;
            signalAppliance.Appliance = appliance;
            signalAppliance.Tolerance = SIGNAL_APPLIANCE_TOLERANCE;

            appliance.Signals.Add(signalAppliance);

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

            dbApp.Save(appliance);

            var result = from sa in dbSA.GetAll()
                                   join s in dbS.GetAll() on sa.Signal.Id equals s.Id
                                   join app in dbApp.GetAll() on sa.Appliance.Id equals app.Id
                                   join sapv in dbSAppV.GetAll() on sa.Id equals sapv.SignalAppliance.Id
                                   join a in dbAT.GetAll() on sapv.AlarmType.Id equals a.Id
                                   where sa.Id.Equals(signalAppliance.Id)
                                   group new {a.Id, a.NameAlarmType, sapv.Value } by new {sa.Id, app.NameAppliance, s.Name, sa.Tolerance} into g
                         select new { signalAppliance = g.Key, setPoint = from v in g where v.Id == 1 select v.Value, highValue = from v in g where v.Id == 2 select v.Value, lowValue = from v in g where v.Id == 3 select v.Value };

            foreach (var sa in result)
            {
                Assert.AreEqual(sa.signalAppliance.Id, signalAppliance.Id);
                ObjectDumper.Write(sa.setPoint);
                Assert.AreEqual(normalValue.Value, sa.setPoint.ElementAtOrDefault(0));
                ObjectDumper.Write(sa.highValue);
                Assert.AreEqual(highValue.Value, sa.highValue.ElementAtOrDefault(0));
                ObjectDumper.Write(sa.lowValue);
                Assert.AreEqual(lowValue.Value, sa.lowValue.ElementAtOrDefault(0));
            }

            dbSAppV.Delete(normalValue.Id);
            dbSAppV.Delete(highValue.Id);
            dbSAppV.Delete(lowValue.Id);

            dbSA.Delete(signalAppliance.Id);

            dbApp.Delete(appliance.Id);

            dbS.Delete(signal.Id);
        }

        [TestMethod]
        [DeploymentItem("hibernate.cfg.xml")]
        public void ListMonitoringRecordsForInbox()
        {
            IRepository<SignalAppliance> dbSA = new SignalApplianceRepository();
            IRepository<MappingTag> dbMapT = new MappingTagRepository();
            IRepository<Monitoring> dbMon = new MonitoringRepository();
            IRepository<Signal> dbS = new SignalRepository();
            IRepository<Appliance> dbApp = new ApplianceRepository();
            IRepository<SignalApplianceValue> dbSAppV = new SignalApplianceValueRepository();
            IRepository<AlarmType> dbAT = new AlarmTypeRepository();
            IRepository<Area> dbA = new AreaRepository();

            var result = from m in ( 
                             from r in (
                                      from mon in dbMon.GetAll()
                                      join mapp in dbMapT.GetAll() on mon.MappingTag.Id equals mapp.Id
                                     where mapp.Id.ToString().Equals("8") || mapp.Id.ToString().Equals("70") || mapp.Id.ToString().Equals("132")
                                    select new { monDatetime = mon.DateTime, appId = mapp.Appliance.Id, sigId = mapp.Signal.Id, pv =  (mapp.AlarmType.Id.Equals(Convert.ToInt32("1")) ? mon.Value: 0 ), alarm = (mon.Value == 1 && !mapp.AlarmType.Id.Equals(Convert.ToInt32("1")) ? mapp.AlarmType.Id : 0 ) ,userId = mon.User.Id})
                            group r by new {r.monDatetime, r.appId, r.sigId, r.userId} into g
                           select new { record = g.Key, monValue = g.Sum(d => d.pv), alarm = g.Sum(d => d.alarm) == 2 ? "A" : g.Sum(d => d.alarm) == 3 ? "B" : g.Sum(d => d.alarm) == 0 ? "N" : "D" })
                       select m;
            /*
                to_char(tmp.datetime, 'yyyy/mm/dd hh24:mi:ss') mon_datetime, area.are_name, sig.sig_name,
                decode(tmp.alarm, 'A', 'Alta', 'B', 'Baja', 'N', 'Normal', 'Desconodido') ala_typ_name,
                smcl_get_appl_signal_comment(tmp.datetime, tmp.app_id, tmp.sig_id, tmp.alarm) mon_comment_on_alarm,
                appl.app_name, case when tmp.sig_id = 1 then round_half_way_down(trunc(tmp.mon_value, 3), 2) else round_half_way_down(trunc(tmp.mon_value, 2), 1) end mon_value ,
                tmp.sig_id, usr.usr_first_name || ' ' || usr.usr_last_name_1 || '(' || usr.usr_login_email || ')' username,
             */
            //group new {a.Id, a.NameAlarmType, sapv.Value } by new {sa.Id, app.NameAppliance, s.Name, sa.Tolerance} into g
            //select new { signalAppliance = g.Key, setPoint = from v in g where v.Id == 1 select v.Value, highValue = from v in g where v.Id == 2 select v.Value, lowValue = from v in g where v.Id == 3 select v.Value };

            ObjectDumper.Write(result.Count());

            foreach (var v in result)
            {
                ObjectDumper.Write(v.record);
                ObjectDumper.Write(v.alarm);
                ObjectDumper.Write(v.monValue);
            }
        }
    }
}
