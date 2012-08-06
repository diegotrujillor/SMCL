using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMCL.Models;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;
using System.Configuration;
using NHibernate.Criterion;
using SMCL.Services.LoggingInterface;
using SMCL.Services.LoggingImplementation;
using SMCL.Enums;
using NHibernate.Exceptions;

namespace SMCL.Controllers
{
    public class SignalApplianceController : Controller
    {

        ILoggable log = new LogSMCL();
        private IRepository<Signal> dbS = new SignalRepository();
        private IRepository<Appliance> dbA = new ApplianceRepository();
        private IRepository<AlarmType> dbAT = new AlarmTypeRepository();
        private IRepository<SignalApplianceValue> dbSAppV = new SignalApplianceValueRepository();
        private IRepository<SignalAppliance> dbSA = new SignalApplianceRepository();

        //
        // GET: /SignalAppliance/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /SignalAppliance/Details/5

        public ViewResult Details(Guid id)
        {
            SignalAppliance signalAppliance = dbSA.GetById(id);

            int differentialPressure = int.Parse(ConfigurationManager.AppSettings["DifferentialPressure"]);
            int temperature = int.Parse(ConfigurationManager.AppSettings["Temperature"]);
            int rh = int.Parse(ConfigurationManager.AppSettings["RH"]);

            string measureUnit = "N/A";

            if (signalAppliance.Signal.Id == differentialPressure)
            {
                measureUnit = ConfigurationManager.AppSettings["Percentage"].ToString();
            }
            else if(signalAppliance.Signal.Id == temperature)
            {
                measureUnit = ConfigurationManager.AppSettings["DegreeCelsius"].ToString();
            }
            else if(signalAppliance.Signal.Id == rh)
            {
                measureUnit = ConfigurationManager.AppSettings["InchesOfWater"].ToString();
            }

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("SignalAppliance.Id", signalAppliance.Id);

            float setPoint = 0, highValue = 0, lowValue = 0;

            foreach (SignalApplianceValue signalApplianceValue in dbSAppV.GetByProperties(properties))
            {
                if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"]))
                {
                    setPoint = signalApplianceValue.Value;
                }
                else if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["HighAlarmId"]))
                {
                    highValue = signalApplianceValue.Value;
                }
                else if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["LowAlarmId"]))
                {
                    lowValue = signalApplianceValue.Value;
                }
            }

            ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(id).Appliance.Id).NameAppliance;
            ViewBag.HighValue = highValue;
            ViewBag.LowValue = lowValue;
            ViewBag.MeasureUnit = measureUnit;
            ViewBag.SetPoint = setPoint;
            ViewBag.SignalName = dbS.GetById(dbSA.GetById(id).Signal.Id).Name;

            return View(signalAppliance);
        }

        //
        // GET: /SignalAppliance/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name");

            return View();
        }

        //
        // POST: /SignalAppliance/Create

        [HttpPost]
        public ActionResult Create(SignalAppliance signalAppliance, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form, true))
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add("Signal.Id", int.Parse(form["SignalId"]));
                properties.Add("Appliance.Id", int.Parse(form["ApplianceId"]));

                IList<SignalAppliance> signalApplianceList = dbSA.GetByProperties(properties);

                if (!signalApplianceList.Any())
                {
                    signalAppliance.Signal = dbS.GetById(int.Parse(form["SignalId"]));
                    signalAppliance.Appliance = dbA.GetById(int.Parse(form["ApplianceId"]));
                    signalAppliance.Tolerance = float.Parse(form["Tolerance"]);

                    dbSA.Save(signalAppliance);

                    updateSignalApplianceValues(signalAppliance, float.Parse(form["SetPoint"]), false);

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalAppliance.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                    log.Write(logList);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["DuplicatedRecordErrorMessage"].ToString();
                }
            }

            ViewBag.ApplianceId = !String.IsNullOrEmpty(form["ApplianceId"]) ? new SelectList(dbA.GetAll(), "Id", "NameAppliance", form["ApplianceId"]) : new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = !String.IsNullOrEmpty(form["SignalId"]) ? new SelectList(dbS.GetAll(), "Id", "Name", form["SignalId"]) : new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.SetPoint = form["SetPoint"];

            return View();
        }

        //
        // GET: /SignalAppliance/Edit/5

        public ActionResult Edit(Guid id)
        {
            SignalAppliance signalAppliance = dbSA.GetById(id);
            signalAppliance.Signal = dbS.GetById(dbSA.GetById(signalAppliance.Id).Signal.Id);
            signalAppliance.Appliance = dbA.GetById(dbSA.GetById(signalAppliance.Id).Appliance.Id);

            Dictionary<string, object> properties  = new Dictionary<string, object>();
            properties.Add("SignalAppliance.Id", signalAppliance.Id);
            properties.Add("AlarmType.Id", int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"].ToString()));

            ViewBag.SetPoint = dbSAppV.GetByProperties(properties)[0].Value;
            ViewBag.Tolerance = signalAppliance.Tolerance;
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name", dbS.GetById(dbSA.GetById(signalAppliance.Id).Signal.Id).Id);
            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance", dbA.GetById(dbSA.GetById(signalAppliance.Id).Appliance.Id).Id);
            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(signalAppliance);
        }

        //
        // POST: /SignalAppliance/Edit/5

        [HttpPost]
        public ActionResult Edit(SignalAppliance signalAppliance, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form, false))
            {
                SignalAppliance sa = dbSA.GetById(signalAppliance.Id);

                float setPoint = float.Parse(form["SetPoint"]);
                float tolerance = float.Parse(form["Tolerance"]);
                
                if (sa  != null)
                {
                    sa.Signal = dbS.GetById(int.Parse(form["SignalId"]));
                    sa.Appliance = dbA.GetById(int.Parse(form["ApplianceId"]));
                    sa.Tolerance = tolerance;

                    dbSA.Update(sa);

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalAppliance.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                    log.Write(logList);

                    updateSignalApplianceValues(sa, setPoint, true);
                }
                else
                {
                    signalAppliance = new SignalAppliance();
                    signalAppliance.Signal = dbS.GetById(int.Parse(form["SignalId"]));
                    signalAppliance.Appliance = dbA.GetById(int.Parse(form["ApplianceId"]));
                    signalAppliance.Tolerance = float.Parse(form["Tolerance"]);

                    dbSA.Save(signalAppliance);

                    updateSignalApplianceValues(sa, setPoint, false);

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalAppliance.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                    log.Write(logList);
                }

                List<Object> logL = new List<Object>();
                logL.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalAppliance.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logL);

                return RedirectToAction("Index");
            }

            ViewBag.ApplianceId = !String.IsNullOrEmpty(form["ApplianceId"]) ? new SelectList(dbA.GetAll(), "Id", "NameAppliance", form["ApplianceId"]) : new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = !String.IsNullOrEmpty(form["SignalId"]) ? new SelectList(dbS.GetAll(), "Id", "Name", form["SignalId"]) : new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.AlarmTypeId = !String.IsNullOrEmpty(form["AlarmTypeId"]) ? new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", form["AlarmTypeId"]) : new SelectList(dbAT.GetAll(), "Id", "NameAlarmType");

            return View();
        }

        //
        // GET: /SignalAppliance/Delete/5

        public ActionResult Delete(Guid id)
        {
            SignalAppliance signalAppliance = dbSA.GetById(id);

            int differentialPressure = int.Parse(ConfigurationManager.AppSettings["DifferentialPressure"]);
            int temperature = int.Parse(ConfigurationManager.AppSettings["Temperature"]);
            int rh = int.Parse(ConfigurationManager.AppSettings["RH"]);

            string measureUnit = "N/A";

            if (signalAppliance.Signal.Id == differentialPressure)
            {
                measureUnit = ConfigurationManager.AppSettings["Percentage"].ToString();
            }
            else if (signalAppliance.Signal.Id == temperature)
            {
                measureUnit = ConfigurationManager.AppSettings["DegreeCelsius"].ToString();
            }
            else if (signalAppliance.Signal.Id == rh)
            {
                measureUnit = ConfigurationManager.AppSettings["InchesOfWater"].ToString();
            }

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("SignalAppliance.Id", signalAppliance.Id);

            float setPoint = 0, highValue = 0, lowValue = 0;

            foreach (SignalApplianceValue signalApplianceValue in dbSAppV.GetByProperties(properties))
            {
                if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"]))
                {
                    setPoint = signalApplianceValue.Value;
                }
                else if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["HighAlarmId"]))
                {
                    highValue = signalApplianceValue.Value;
                }
                else if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["LowAlarmId"]))
                {
                    lowValue = signalApplianceValue.Value;
                }
            }

            ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(id).Appliance.Id).NameAppliance;
            ViewBag.HighValue = highValue;
            ViewBag.LowValue = lowValue;
            ViewBag.MeasureUnit = measureUnit;
            ViewBag.SetPoint = setPoint;
            ViewBag.SignalName = dbS.GetById(dbSA.GetById(id).Signal.Id).Name;

            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(signalAppliance);
        }

        //
        // POST: /SignalAppliance/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            try
            {
                var signalAppliancesValues = dbSAppV.GetByProperty("SignalAppliance.Id", id);

                foreach (var signalApplianceValue in signalAppliancesValues)
                {
                    dbSAppV.Delete(signalApplianceValue.Id);
                }

                dbSA.Delete(id);

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + id + ")", (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);
            }
            catch (GenericADOException ex)
            {
                ViewData["ValidationErrorMessage"] = "Imposible eliminar, registros dependientes asociados.";

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ex.InnerException.Message, (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);

                SignalAppliance signalAppliance = dbSA.GetById(id);

                int differentialPressure = int.Parse(ConfigurationManager.AppSettings["DifferentialPressure"]);
                int temperature = int.Parse(ConfigurationManager.AppSettings["Temperature"]);
                int rh = int.Parse(ConfigurationManager.AppSettings["RH"]);

                string measureUnit = "N/A";

                if (signalAppliance.Signal.Id == differentialPressure)
                {
                    measureUnit = ConfigurationManager.AppSettings["Percentage"].ToString();
                }
                else if (signalAppliance.Signal.Id == temperature)
                {
                    measureUnit = ConfigurationManager.AppSettings["DegreeCelsius"].ToString();
                }
                else if (signalAppliance.Signal.Id == rh)
                {
                    measureUnit = ConfigurationManager.AppSettings["InchesOfWater"].ToString();
                }

                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add("SignalAppliance.Id", signalAppliance.Id);

                float setPoint = 0, highValue = 0, lowValue = 0;

                foreach (SignalApplianceValue signalApplianceValue in dbSAppV.GetByProperties(properties))
                {
                    if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"]))
                    {
                        setPoint = signalApplianceValue.Value;
                    }
                    else if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["HighAlarmId"]))
                    {
                        highValue = signalApplianceValue.Value;
                    }
                    else if (signalApplianceValue.AlarmType.Id == int.Parse(ConfigurationManager.AppSettings["LowAlarmId"]))
                    {
                        lowValue = signalApplianceValue.Value;
                    }
                }

                ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(id).Appliance.Id).NameAppliance;
                ViewBag.HighValue = highValue;
                ViewBag.LowValue = lowValue;
                ViewBag.MeasureUnit = measureUnit;
                ViewBag.SetPoint = setPoint;
                ViewBag.SignalName = dbS.GetById(dbSA.GetById(id).Signal.Id).Name;

                return View(signalAppliance);
            }
            catch (Exception ex)
            {
                ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["UnknownError"];

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ex.InnerException.Message, (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);

                return View();
            }

            return RedirectToAction("Index");
        }

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = dbSA.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var signalAppliances = dbSA.GetAll().OrderBy(o => o.Id).Skip(pageIndex * pageSize);//.Take(pageSize);

            var results = from sa in signalAppliances
                          join s in dbS.GetAll() on sa.Signal.Id equals s.Id
                          join app in dbA.GetAll() on sa.Appliance.Id equals app.Id
                          join sapv in dbSAppV.GetAll() on sa.Id equals sapv.SignalAppliance.Id
                          join a in dbAT.GetAll() on sapv.AlarmType.Id equals a.Id
                         group new { a.Id, a.NameAlarmType, sapv.Value } by new { sa.Id, Appliance = app.NameAppliance, Signal = s.Name, sa.Tolerance } into g
                       orderby g.Key.Id
                        select new { signalAppliance = g.Key, SetPoint = from v in g where v.Id == int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"].ToString()) select v.Value, HighValue = from v in g where v.Id == int.Parse(ConfigurationManager.AppSettings["HighAlarmId"].ToString()) select v.Value, LowValue = from v in g where v.Id == int.Parse(ConfigurationManager.AppSettings["LowAlarmId"].ToString()) select v.Value };

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from r in results
                        select new
                        {
                            id = r.signalAppliance.Id,
                            cell = new[] { r.signalAppliance.Appliance.ToString(), r.signalAppliance.Signal.ToString(), r.SetPoint.ElementAtOrDefault(0).ToString(), r.signalAppliance.Tolerance.ToString(), r.HighValue.ElementAtOrDefault(0).ToString(), r.LowValue.ElementAtOrDefault(0).ToString() }
                        }).ToList()
            };
            return Json(jsonData);
        }

        private bool FormCollectionIsValid(FormCollection form, bool actionSave)
        {
            if (!String.IsNullOrWhiteSpace(form["SetPoint"]))
            {
                if(!String.IsNullOrWhiteSpace(form["Tolerance"]))
                {
                    if (!String.IsNullOrEmpty(form["ApplianceId"]))
                    {
                        if (!String.IsNullOrEmpty(form["SignalId"]))
                        {
                            if (!actionSave)
                            {
                                return true;
                            }

                            Dictionary<string, object> properties = new Dictionary<string, object>();
                            properties.Add("Signal.Id", int.Parse(form["SignalId"]));
                            properties.Add("Appliance.Id", int.Parse(form["ApplianceId"]));
                            IList<SignalAppliance> signalApplianceList = dbSA.GetByProperties(properties);

                            if (signalApplianceList.Any())
                            {
                                ViewData["ValidationErrorMessage"] = "Registro (Señal-Equipo): " + ConfigurationManager.AppSettings["EmptyOrDuplicatedFieldErrorMessage"].ToString();
                                return false; //Error on record already exists.
                            }
                            return true;
                        }
                        else
                        {
                            ViewData["ValidationErrorMessage"] = "Nombre señal: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                            return false;
                        }
                    }
                    else
                    {
                        ViewData["ValidationErrorMessage"] = "Nombre equipo: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                        return false;
                    }
                }
                else
                {
                    ViewData["ValidationErrorMessage"] = "Tolerancia: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                    return false;
                }
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Set Point: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                return false;
            }
        }

        private void updateSignalApplianceValues(SignalAppliance signalAppliance, float setPoint , bool exists)
        {
            int normalAlarmTypeId = int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"].ToString());
            int highAlarmTypeId = int.Parse(ConfigurationManager.AppSettings["HighAlarmId"].ToString());
            int lowAlarmTypeId = int.Parse(ConfigurationManager.AppSettings["LowAlarmId"].ToString());

            if (exists)
            {
                IList<SignalApplianceValue> saValues = dbSAppV.GetByProperty("SignalAppliance.Id", signalAppliance.Id);

                foreach (SignalApplianceValue saValue in saValues)
                {
                    if (saValue.AlarmType.Id == normalAlarmTypeId)
                    {
                        saValue.Value = setPoint;
                    }
                    else if (saValue.AlarmType.Id == highAlarmTypeId)
                    {
                        saValue.Value = setPoint + signalAppliance.Tolerance;
                    }
                    else if (saValue.AlarmType.Id == lowAlarmTypeId)
                    {
                        saValue.Value = setPoint - signalAppliance.Tolerance;
                    }

                    dbSAppV.Update(saValue);
                }
            }
            else
            {
                SignalApplianceValue normalValue = new SignalApplianceValue();
                normalValue.SignalAppliance = signalAppliance;
                normalValue.AlarmType = dbAT.GetById(normalAlarmTypeId);
                normalValue.Value = setPoint;

                SignalApplianceValue highValue = new SignalApplianceValue();
                highValue.SignalAppliance = signalAppliance;
                highValue.AlarmType = dbAT.GetById(highAlarmTypeId);
                highValue.Value = setPoint + signalAppliance.Tolerance;

                SignalApplianceValue lowValue = new SignalApplianceValue();
                lowValue.SignalAppliance = signalAppliance;
                lowValue.AlarmType = dbAT.GetById(lowAlarmTypeId);
                lowValue.Value = setPoint - signalAppliance.Tolerance;

                dbSAppV.Save(normalValue);
                dbSAppV.Save(highValue);
                dbSAppV.Save(lowValue);
            }
        }
    }
}
