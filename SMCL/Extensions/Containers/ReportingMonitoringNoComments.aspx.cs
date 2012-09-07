using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using NHibernate;
using NHibernate.Criterion;
using SMCL.Enums;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;
using SMCL.Services.LoggingInterface;
using SMCL.Services.LoggingImplementation;
using System.Web.UI.HtmlControls;

namespace SMCL.Extensions.Containers
{
    public partial class ReportingMonitoringNoComments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlInputText startDate = (HtmlInputText)FindControl("startDate");
            HtmlInputText finalDate = (HtmlInputText)FindControl("finalDate");
            HtmlInputHidden alarmTypeExclusion = (HtmlInputHidden)FindControl("alarmTypeExclusionId");
            HtmlInputHidden alarmTypeRecover = (HtmlInputHidden)FindControl("alarmTypeRecoverId");

            if (!Page.IsPostBack)
            {
                if (String.IsNullOrEmpty(startDate.Value))
                {
                    startDate.Value = DateTime.Now.ToString("yyyy/MM/dd");
                }
                if (String.IsNullOrEmpty(finalDate.Value))
                {
                    finalDate.Value = DateTime.Now.ToString("yyyy/MM/dd");
                }

                this.SetParametersValues(ddlAppliances.SelectedItem.Value);

                alarmTypeExclusion.Value = ConfigurationManager.AppSettings["AlarmTypeExclusionId"];
                alarmTypeRecover.Value = ConfigurationManager.AppSettings["MockNormalAlarmId"];
            }
        }

        protected void ddlAreas_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = (DropDownList)sender;
            this.InitializeDropDownList(ddlAppliances);

            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList appliances = session.CreateCriteria(typeof(Appliance)).Add(Restrictions.Eq("Area.Id", int.Parse(index.SelectedValue)))
                                                                            .AddOrder(Order.Asc("NameAppliance"))
                                                                            .List();
                foreach (Appliance item in appliances)
                {
                    ddlAppliances.Items.Add(new ListItem(item.NameAppliance, item.Id.ToString()));
                }
            }
        }

        protected void ddlAppliances_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.Refresh();
            this.SetParametersValues(ddlAppliances.SelectedItem.Value);
        }

        protected void ddlAlarms_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.Refresh();
        }

        protected void ddlSignals_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.Refresh();
        }

        protected void ddlAreas_Init(object sender, EventArgs e)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList areas = session.CreateCriteria(typeof(Area)).AddOrder(Order.Asc("Name")).List();
                foreach (Area item in areas)
                {
                    ddlAreas.Items.Add(new ListItem(item.Name, item.Id.ToString()));
                }
            }
        }

        protected void ddlAlarms_Init(object sender, EventArgs e)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList alarms = session.CreateCriteria(typeof(AlarmType)).AddOrder(Order.Asc("NameAlarmType")).List();
                foreach (AlarmType item in alarms)
                {
                   if (item.Id.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["NormalAlarmId"])))
                    {
                        ddlAlarms.Items.Add(new ListItem(ConfigurationManager.AppSettings["NormalAlarmText"], item.Id.ToString()));
                    }
                    else
                    {
                        if(!item.Id.Equals(Convert.ToInt32(ConfigurationManager.AppSettings["MockNormalAlarmId"]))) {
                            ddlAlarms.Items.Add(new ListItem(item.NameAlarmType, item.Id.ToString()));
                        }
                    }
                }
            }
        }

        protected void ddlSignals_Init(object sender, EventArgs e)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList signals = session.CreateCriteria(typeof(Signal)).AddOrder(Order.Asc("Name")).List();
                foreach (Signal item in signals)
                {
                    ddlSignals.Items.Add(new ListItem(item.Name, item.Id.ToString()));
                }
            }
        }

        private void InitializeDropDownList(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("-- Seleccione --", "0"));
            ddl.Items.Add(new ListItem("-- Todos --", "-1"));
        }

        private void SetParametersValues(string applianceId)
        {
            ReportParameter applianceParam = new ReportParameter();
            ReportParameter temperatureParam = new ReportParameter();
            ReportParameter RHParam = new ReportParameter();
            ReportParameter diffPressureParam = new ReportParameter();
            IRepository<Appliance> dbA = new ApplianceRepository();
            IRepository<SignalAppliance> dbSA = new SignalApplianceRepository();
            IRepository<SignalApplianceValue> dbSAV = new SignalApplianceValueRepository();

            applianceParam = new ReportParameter("ApplianceParam", "N/A");
            temperatureParam = new ReportParameter("TemperatureParam", "0");
            RHParam = new ReportParameter("RHParam", "0");
            diffPressureParam = new ReportParameter("DiffPressureParam", "0");

            Appliance appliance = dbA.GetById(int.Parse(applianceId));

            if (appliance == null)
            {
                if (int.Parse(applianceId) < 0)
                {
                    applianceParam = new ReportParameter("ApplianceParam", "TODOS");
                    temperatureParam = new ReportParameter("TemperatureParam", "Todos");
                    RHParam = new ReportParameter("RHParam", "Todos");
                    diffPressureParam = new ReportParameter("DiffPressureParam", "Todos");
                }
            }
            else
            {
                applianceParam = new ReportParameter("ApplianceParam", appliance.NameAppliance);

                var arrayDiffPressureParam = new ArrayList();
                var arrayTemperatureParam = new ArrayList();
                var arrayRHParam = new ArrayList();
                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add("Appliance.Id", int.Parse(applianceId));
                IList<SignalAppliance> signalApplianceList = dbSA.GetByProperties(properties);

                foreach (var signalAppliance in signalApplianceList)
                {
                    Dictionary<string, object> propertiesSA = new Dictionary<string, object>();
                    propertiesSA.Add("SignalAppliance.Id", signalAppliance.Id);
                    propertiesSA.Add("AlarmType.Id", int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"]));
                    IList<SignalApplianceValue> signalApplianceValueList = dbSAV.GetByProperties(propertiesSA);

                    foreach (var signalApplianceValue in signalApplianceValueList.OrderByDescending(o => o.Value))
                    {
                        if (signalAppliance.Signal.Id == SMCLSignals.DifferentialPressure)
                        {
                            arrayDiffPressureParam.Add(signalApplianceValue.Value);
                            arrayDiffPressureParam.Add(signalAppliance.Tolerance);
                        }
                        if (signalAppliance.Signal.Id == SMCLSignals.Temperature)
                        {
                            arrayTemperatureParam.Add(signalApplianceValue.Value);
                            arrayTemperatureParam.Add(signalAppliance.Tolerance);
                        }
                        if (signalAppliance.Signal.Id == SMCLSignals.RH)
                        {
                            arrayRHParam.Add(signalApplianceValue.Value);
                            arrayRHParam.Add(signalAppliance.Tolerance);
                        }
                    }
                }

                if (arrayDiffPressureParam.Count > 0)
                {
                    diffPressureParam = new ReportParameter("DiffPressureParam", arrayDiffPressureParam[0].ToString() + " ± " + arrayDiffPressureParam[1].ToString() + " " + ConfigurationManager.AppSettings["InchesOfWater"]);
                }
                if (arrayTemperatureParam.Count > 0)
                {
                    temperatureParam = new ReportParameter("TemperatureParam", arrayTemperatureParam[0].ToString() + " ± " + arrayTemperatureParam[1].ToString() + " " + ConfigurationManager.AppSettings["DegreeCelsius"]);
                }
                if (arrayRHParam.Count > 0)
                {
                    RHParam = new ReportParameter("RHParam", arrayRHParam[0].ToString() + " ± " + arrayRHParam[1].ToString() + " " + ConfigurationManager.AppSettings["Percentage"]);
                }
            }

            ReportParameterCollection parameters = new ReportParameterCollection();

            parameters.Add(applianceParam);
            parameters.Add(temperatureParam);
            parameters.Add(RHParam);
            parameters.Add(diffPressureParam);

            ReportViewer1.LocalReport.SetParameters(parameters);
        }

        protected void odsMonitoring_DataBinding(object sender, EventArgs e)
        {
            ILoggable log = new LogSMCL();
            List<Object> logList = new List<Object>();
            logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["ReportText"] + "Texto sin comentarios", (int)EventTypes.Report, (int)Session["UserId"]));
            log.Write(logList);
        }
    }
}