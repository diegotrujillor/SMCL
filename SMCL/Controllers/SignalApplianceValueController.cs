using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public class SignalApplianceValueController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<Signal> dbS = new SignalRepository();
        private IRepository<Appliance> dbA = new ApplianceRepository();
        private IRepository<AlarmType> dbAT = new AlarmTypeRepository();
        private IRepository<SignalAppliance> dbSA = new SignalApplianceRepository();
        private IRepository<SignalApplianceValue> db = new SignalApplianceValueRepository();

        //
        // GET: /SignalApplianceValue/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /SignalApplianceValue/Details/5

        public ViewResult Details(Guid id)
        {
            SignalApplianceValue signalApplianceValue = db.GetById(id);

            ViewBag.Value = signalApplianceValue.Value;
            ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Appliance.Id).NameAppliance;
            ViewBag.SignalName = dbS.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Signal.Id).Name;
            ViewBag.AlarmTypeName = dbAT.GetById(signalApplianceValue.AlarmType.Id).NameAlarmType;

            return View(signalApplianceValue);
        }

        //
        // GET: /SignalApplianceValue/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.AlarmTypeId = new SelectList(dbAT.GetAll(), "Id", "NameAlarmType");

            return View();
        }

        //
        // POST: /SignalApplianceValue/Create

        [HttpPost]
        public ActionResult Create(SignalApplianceValue signalApplianceValue, FormCollection form)
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
                    SignalAppliance signalAppliance = new SignalAppliance();
                    signalAppliance.Signal = dbS.GetById(int.Parse(form["SignalId"]));
                    signalAppliance.Appliance = dbA.GetById(int.Parse(form["ApplianceId"]));

                    dbSA.Save(signalAppliance);
                    signalApplianceList = dbSA.GetByProperties(properties);
                }

                foreach (SignalAppliance item in signalApplianceList)
                {
                    signalApplianceValue.SignalAppliance = item;
                    signalApplianceValue.AlarmType = dbAT.GetById(int.Parse(form["AlarmTypeId"]));
                    signalApplianceValue.Value = float.Parse(form["Value"]);

                    properties = new Dictionary<string, object>();
                    properties.Add("SignalAppliance.Id", item.Id);
                    properties.Add("AlarmType.Id", dbAT.GetById(signalApplianceValue.AlarmType.Id).Id);

                    if (!db.GetByProperties(properties).Any())
                    {
                        db.Save(signalApplianceValue);

                        List<Object> logList = new List<Object>();
                        logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalApplianceValue.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
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
            ViewBag.AlarmTypeId = !String.IsNullOrEmpty(form["AlarmTypeId"]) ? new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", form["AlarmTypeId"]) : new SelectList(dbAT.GetAll(), "Id", "NameAlarmType");

            return View();
        }

        //
        // GET: /SignalApplianceValue/Edit/5

        public ActionResult Edit(Guid id)
        {
            SignalApplianceValue signalApplianceValue = db.GetById(id);
            signalApplianceValue.SignalAppliance = dbSA.GetById(signalApplianceValue.SignalAppliance.Id);
            signalApplianceValue.SignalAppliance.Signal = dbS.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Signal.Id);
            signalApplianceValue.SignalAppliance.Appliance = dbA.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Appliance.Id);
            signalApplianceValue.AlarmType = dbAT.GetById(signalApplianceValue.AlarmType.Id);

            ViewBag.Value = signalApplianceValue.Value;
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name", dbS.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Signal.Id).Id);
            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance", dbA.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Appliance.Id).Id);
            ViewBag.AlarmTypeId = new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", dbAT.GetById(signalApplianceValue.AlarmType.Id).Id);
            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(signalApplianceValue);
        }

        //
        // POST: /SignalApplianceValue/Edit/5

        [HttpPost]
        public ActionResult Edit(SignalApplianceValue signalApplianceValue, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form, false))
            {
                SignalAppliance signalAppliance = dbSA.GetById(db.GetById(signalApplianceValue.Id).SignalAppliance.Id);

                if (signalAppliance != null)
                {
                    signalAppliance.Signal = dbS.GetById(int.Parse(form["SignalId"]));
                    signalAppliance.Appliance = dbA.GetById(int.Parse(form["ApplianceId"]));

                    dbSA.Update(signalAppliance);

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalApplianceValue.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                    log.Write(logList);
                }
                else
                {
                    signalAppliance = new SignalAppliance();
                    signalAppliance.Signal = dbS.GetById(int.Parse(form["SignalId"]));
                    signalAppliance.Appliance = dbA.GetById(int.Parse(form["ApplianceId"]));

                    dbSA.Save(signalAppliance);

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalAppliance.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                    log.Write(logList);
                }

                signalApplianceValue.SignalAppliance = signalAppliance;
                signalApplianceValue.AlarmType = dbAT.GetById(int.Parse(form["AlarmTypeId"]));
                signalApplianceValue.Value = float.Parse(form["Value"]);

                db.Update(signalApplianceValue);

                List<Object> logL = new List<Object>();
                logL.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signalApplianceValue.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logL);

                return RedirectToAction("Index");
            }

            ViewBag.ApplianceId = !String.IsNullOrEmpty(form["ApplianceId"]) ? new SelectList(dbA.GetAll(), "Id", "NameAppliance", form["ApplianceId"]) : new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = !String.IsNullOrEmpty(form["SignalId"]) ? new SelectList(dbS.GetAll(), "Id", "Name", form["SignalId"]) : new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.AlarmTypeId = !String.IsNullOrEmpty(form["AlarmTypeId"]) ? new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", form["AlarmTypeId"]) : new SelectList(dbAT.GetAll(), "Id", "NameAlarmType");

            return View();
        }

        //
        // GET: /SignalApplianceValue/Delete/5

        public ActionResult Delete(Guid id)
        {
            SignalApplianceValue signalApplianceValue = db.GetById(id);

            ViewBag.Value = signalApplianceValue.Value;
            ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Appliance.Id).NameAppliance;
            ViewBag.SignalName = dbS.GetById(dbSA.GetById(signalApplianceValue.SignalAppliance.Id).Signal.Id).Name;
            ViewBag.AlarmTypeName = dbAT.GetById(signalApplianceValue.AlarmType.Id).NameAlarmType;
            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(signalApplianceValue);
        }

        //
        // POST: /SignalApplianceValue/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            try
            {
                dbSA.Delete(db.GetById(id).SignalAppliance.Id);

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + id + ")", (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);
            }
            catch (GenericADOException ex)
            {
                ViewData["ValidationErrorMessage"] = "Imposible eliminar, registros dependientes asociados.";

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ex.InnerException.Message, (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);

                SignalApplianceValue entity = db.GetById(id);

                ViewBag.ApplianceName = dbA.GetById(dbSA.GetById(entity.SignalAppliance.Id).Appliance.Id).NameAppliance;
                ViewBag.SignalName = dbS.GetById(dbSA.GetById(entity.SignalAppliance.Id).Signal.Id).Name;
                ViewBag.AlarmTypeName = dbAT.GetById(entity.AlarmType.Id).NameAlarmType;

                return View(entity);
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

        private SignalApplianceValue RemoveExtraSpaces(SignalApplianceValue signalappliancevalue)
        {
            //signalappliancevalue.Value = !String.IsNullOrWhiteSpace(signalappliancevalue.Value) ? signalappliancevalue.Value.Trim() : null;

            return signalappliancevalue;
        }



        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = db.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var questions = db.GetAll().OrderBy(o => o.SignalAppliance.Id).Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 join s in dbS.GetAll() on dbS.GetById(dbSA.GetById(question.SignalAppliance.Id).Signal.Id).Id equals s.Id
                                 join a in dbA.GetAll() on dbA.GetById(dbSA.GetById(question.SignalAppliance.Id).Appliance.Id).Id equals a.Id
                                 join sa in dbSA.GetAll() on question.SignalAppliance.Id equals sa.Id
                                 join at in dbAT.GetAll() on question.AlarmType.Id equals at.Id
                                 select new { question.Id, a.NameAppliance, s.Name, at.NameAlarmType, question.Value }).OrderBy(o => o.Id).ToList();

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
            if (!String.IsNullOrWhiteSpace(form["Value"]))
            {
                if (!String.IsNullOrEmpty(form["ApplianceId"]))
                {
                    if (!String.IsNullOrEmpty(form["SignalId"]))
                    {
                        if (!String.IsNullOrEmpty(form["AlarmTypeId"]))
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

                                    if (db.GetByProperties(properties).Any())
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
                            ViewData["ValidationErrorMessage"] = "Nombre tipo alarma: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                            return false;
                        }
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
                ViewData["ValidationErrorMessage"] = "Nombre Valor: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                return false;
            }
        }
    }
}