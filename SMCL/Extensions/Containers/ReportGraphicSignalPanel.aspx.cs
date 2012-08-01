using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using NHibernate;
using NHibernate.Criterion;
using SMCL.Enums;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;
using System.Web.UI.HtmlControls;

namespace SMCL.Extensions
{
    public partial class ReportGraphicSignalPanel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HtmlInputText startDate = (HtmlInputText)FindControl("startDate");
            HtmlInputText finalDate = (HtmlInputText)FindControl("finalDate");

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

                this.SetParametersValues(ddlAppliancesG.SelectedItem.Value);
            }
        }

        protected void ddlAreasG_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = (DropDownList)sender;
            this.InitializeDropDownList(ddlAppliancesG);

            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList appliances = session.CreateCriteria(typeof(Appliance)).Add(Restrictions.Eq("Area.Id", int.Parse(index.SelectedValue)))
                                                                            .AddOrder(Order.Asc("NameAppliance"))
                                                                            .List();
                foreach (Appliance item in appliances)
                {
                    ddlAppliancesG.Items.Add(new ListItem(item.NameAppliance, item.Id.ToString()));
                }
            }
        }

        protected void ddlAppliancesG_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer2.LocalReport.Refresh();
            this.SetParametersValues(ddlAppliancesG.SelectedItem.Value);
        }

        protected void ddlAlarmsG_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer2.LocalReport.Refresh();
        }

        protected void ddlSignalsG_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReportViewer2.LocalReport.Refresh();
        }

        protected void ddlAreasG_Init(object sender, EventArgs e)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList areas = session.CreateCriteria(typeof(Area)).AddOrder(Order.Asc("Name")).List();
                foreach (Area item in areas)
                {
                    ddlAreasG.Items.Add(new ListItem(item.Name, item.Id.ToString()));
                }
            }
        }
        
        protected void ddlAlarmsG_Init(object sender, EventArgs e)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList alarms = session.CreateCriteria(typeof(AlarmType)).AddOrder(Order.Asc("NameAlarmType")).List();
                foreach (AlarmType item in alarms)
                {
                    ddlAlarmsG.Items.Add(new ListItem(item.NameAlarmType, item.Id.ToString()));
                }
            }
        }

        protected void ddlSignalsG_Init(object sender, EventArgs e)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                IList signals = session.CreateCriteria(typeof(Signal)).AddOrder(Order.Asc("Name")).List();
                foreach (Signal item in signals)
                {
                    ddlSignalsG.Items.Add(new ListItem(item.Name, item.Id.ToString()));
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
                    IList<SignalApplianceValue> signalApplianceValueList = dbSAV.GetByProperties(propertiesSA);

                    foreach (var signalApplianceValue in signalApplianceValueList.OrderByDescending(o => o.Value))
                    {
                        if (signalAppliance.Signal.Id == SMCLSignals.DifferentialPressure)
                        {
                            arrayDiffPressureParam.Add(signalApplianceValue.Value);
                        }
                        if (signalAppliance.Signal.Id == SMCLSignals.Temperature)
                        {
                            arrayTemperatureParam.Add(signalApplianceValue.Value);
                        }
                        if (signalAppliance.Signal.Id == SMCLSignals.RH)
                        {
                            arrayRHParam.Add(signalApplianceValue.Value);
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

            ReportViewer2.LocalReport.SetParameters(parameters);
        }
    }
}