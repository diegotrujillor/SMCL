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

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("SignalAppliance.Id", signalAppliance.Id);
            properties.Add("AlarmType.Id", dbAT.GetById(int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"])).Id);

            ViewBag.SetPoint = dbSAppV.GetByProperties(properties);
            ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(id).Appliance.Id).NameAppliance;
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
                    signalApplianceList = dbSA.GetByProperties(properties);
                }

                foreach (SignalAppliance item in signalApplianceList)
                {
                    SignalApplianceValue normalValue = new SignalApplianceValue();
                    normalValue.SignalAppliance = item;
                    normalValue.AlarmType = dbAT.GetById(int.Parse(ConfigurationManager.AppSettings["NormalAlarmId"].ToString()));
                    normalValue.Value = float.Parse(form["SetPoint"]);

                    SignalApplianceValue highValue = new SignalApplianceValue();
                    highValue.SignalAppliance = item;
                    highValue.AlarmType = dbAT.GetById(int.Parse(ConfigurationManager.AppSettings["HighAlarmId"].ToString()));
                    highValue.Value = float.Parse(form["SetPoint"]) + item.Tolerance;

                    SignalApplianceValue lowValue = new SignalApplianceValue();
                    lowValue.SignalAppliance = item;
                    lowValue.AlarmType = dbAT.GetById(int.Parse(ConfigurationManager.AppSettings["LowAlarmId"].ToString()));
                    lowValue.Value = float.Parse(form["SetPoint"]) - item.Tolerance;

                    properties = new Dictionary<string, object>();
                    properties.Add("SignalAppliance.Id", item.Id);

                    if (!dbSAppV.GetByProperties(properties).Any())
                    {
                        dbSAppV.Save(normalValue);
                        dbSAppV.Save(highValue);
                        dbSAppV.Save(lowValue);

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
            }

            ViewBag.ApplianceId = !String.IsNullOrEmpty(form["ApplianceId"]) ? new SelectList(dbA.GetAll(), "Id", "NameAppliance", form["ApplianceId"]) : new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = !String.IsNullOrEmpty(form["SignalId"]) ? new SelectList(dbS.GetAll(), "Id", "Name", form["SignalId"]) : new SelectList(dbS.GetAll(), "Id", "Name");

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


        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = dbSAppV.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var questions = dbSAppV.GetAll().OrderBy(o => o.SignalAppliance.Id).Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 join s in dbS.GetAll() on dbS.GetById(dbSA.GetById(question.SignalAppliance.Id).Signal.Id).Id equals s.Id
                                 join a in dbA.GetAll() on dbA.GetById(dbSA.GetById(question.SignalAppliance.Id).Appliance.Id).Id equals a.Id
                                 join sa in dbSA.GetAll() on question.SignalAppliance.Id equals sa.Id
                                 join at in dbAT.GetAll() on question.AlarmType.Id equals at.Id
                                 select new { s.Id, a.NameAppliance, s.Name, at.NameAlarmType, question.Value }).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from question in questionDatas
                        select new
                        {
                            id = question.Id,
                            cell = new[] { question.Id.ToString(), question.NameAppliance.ToString(), question.Name.ToString(), question.NameAlarmType.ToString(), question.Value.ToString() }
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
                                foreach (SignalAppliance item in signalApplianceList)
                                {
                                    properties = new Dictionary<string, object>();
                                    properties.Add("SignalAppliance.Id", item.Id);
                                    properties.Add("AlarmType.Id", dbAT.GetById(int.Parse(form["AlarmTypeId"])).Id);

                                    if (dbSAppV.GetByProperties(properties).Any())
                                    {
                                        ViewData["ValidationErrorMessage"] = "Registro (Señal-Equipo-Alarma): " + ConfigurationManager.AppSettings["EmptyOrDuplicatedFieldErrorMessage"].ToString();
                                        return false; //Error on record already exists.
                                    }
                                }
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

    }
}
