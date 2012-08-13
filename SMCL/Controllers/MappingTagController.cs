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
using SMCL.Services.LoggingInterface;
using SMCL.Services.LoggingImplementation;
using SMCL.Enums;
using NHibernate.Exceptions;

namespace SMCL.Controllers
{ 
    public class MappingTagController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<Signal> dbS = new SignalRepository();
        private IRepository<Appliance> dbA = new ApplianceRepository();
        private IRepository<AlarmType> dbAT = new AlarmTypeRepository();
        private IRepository<MappingTag> db = new MappingTagRepository();

        //
        // GET: /MappingTag/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /MappingTag/Details/5

        public ViewResult Details(int id)
        {
            MappingTag mappingtag = db.GetById(id);
            mappingtag.Signal = dbS.GetById(mappingtag.Signal.Id);
            mappingtag.Appliance = dbA.GetById(mappingtag.Appliance.Id);
            mappingtag.AlarmType = dbAT.GetById(mappingtag.AlarmType.Id);
            return View(mappingtag);
        }

        //
        // GET: /MappingTag/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.AlarmTypeId = new SelectList((from at in dbAT.GetAll() where at.Id != 4 select at).ToList(), "Id", "NameAlarmType");
            
            return View();
        } 

        //
        // POST: /MappingTag/Create

        [HttpPost]
        public ActionResult Create(MappingTag mappingtag, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                mappingtag.Appliance = dbA.GetById(Convert.ToInt32(form["ApplianceId"]));
                mappingtag.Signal = dbS.GetById(Convert.ToInt32(form["SignalId"]));
                mappingtag.AlarmType = dbAT.GetById(Convert.ToInt32(form["AlarmTypeId"]));
                db.Save(this.RemoveExtraSpaces(mappingtag));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + mappingtag.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            ViewBag.ApplianceId = !String.IsNullOrEmpty(form["ApplianceId"]) ? new SelectList(dbA.GetAll(), "Id", "NameAppliance", form["ApplianceId"]) : new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = !String.IsNullOrEmpty(form["SignalId"]) ? new SelectList(dbS.GetAll(), "Id", "Name", form["SignalId"]) : new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.AlarmTypeId = !String.IsNullOrEmpty(form["AlarmTypeId"]) ? new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", form["AlarmTypeId"]) : new SelectList(dbAT.GetAll(), "Id", "NameAlarmType");

            return View(mappingtag);
        }
                       
        //
        // GET: /MappingTag/Edit/5
 
        public ActionResult Edit(int id)
        {
            MappingTag mappingtag = db.GetById(id);
            mappingtag.Appliance = dbA.GetById(mappingtag.Appliance.Id);
            mappingtag.Signal = dbS.GetById(mappingtag.Signal.Id);
            mappingtag.AlarmType = dbAT.GetById(mappingtag.AlarmType.Id);

            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance", mappingtag.Appliance.Id);
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name", mappingtag.Signal.Id);
            ViewBag.AlarmTypeId = new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", mappingtag.AlarmType.Id);

            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(mappingtag);
        }

        //
        // POST: /MappingTag/Edit/5

        [HttpPost]
        public ActionResult Edit(MappingTag mappingtag, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                mappingtag.Appliance = dbA.GetById(Convert.ToInt32(form["ApplianceId"]));
                mappingtag.Signal = dbS.GetById(Convert.ToInt32(form["SignalId"]));
                mappingtag.AlarmType = dbAT.GetById(Convert.ToInt32(form["AlarmTypeId"]));
                db.Save(this.RemoveExtraSpaces(mappingtag));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + mappingtag.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            ViewBag.ApplianceId = !String.IsNullOrEmpty(form["ApplianceId"]) ? new SelectList(dbA.GetAll(), "Id", "NameAppliance", form["ApplianceId"]) : new SelectList(dbA.GetAll(), "Id", "NameAppliance");
            ViewBag.SignalId = !String.IsNullOrEmpty(form["SignalId"]) ? new SelectList(dbS.GetAll(), "Id", "Name", form["SignalId"]) : new SelectList(dbS.GetAll(), "Id", "Name");
            ViewBag.AlarmTypeId = !String.IsNullOrEmpty(form["AlarmTypeId"]) ? new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", form["AlarmTypeId"]) : new SelectList(dbAT.GetAll(), "Id", "NameAlarmType");

            return View(mappingtag);
        }

        //
        // GET: /MappingTag/Delete/5
 
        public ActionResult Delete(int id)
        {
            MappingTag mappingtag = db.GetById(id);
            mappingtag.Appliance = dbA.GetById(mappingtag.Appliance.Id);
            mappingtag.Signal = dbS.GetById(mappingtag.Signal.Id);
            mappingtag.AlarmType = dbAT.GetById(mappingtag.AlarmType.Id);

            ViewBag.ApplianceId = new SelectList(dbA.GetAll(), "Id", "NameAppliance", mappingtag.Appliance.Id);
            ViewBag.SignalId = new SelectList(dbS.GetAll(), "Id", "Name", mappingtag.Signal.Id);
            ViewBag.AlarmTypeId = new SelectList(dbAT.GetAll(), "Id", "NameAlarmType", mappingtag.AlarmType.Id);
            
            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(mappingtag);
        }

        //
        // POST: /MappingTag/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            try
            {
                db.Delete(id);
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + id + ")", (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);
            }
            catch (GenericADOException ex)
            {
                ViewData["ValidationErrorMessage"] = "Imposible eliminar, registros dependientes asociados.";

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ex.InnerException.Message, (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);

                MappingTag entity = db.GetById(id);
                entity.AlarmType = dbAT.GetById(entity.AlarmType.Id);
                entity.Appliance = dbA.GetById(entity.Appliance.Id);
                entity.Signal = dbS.GetById(entity.Signal.Id);

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

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = db.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var questions = db.GetAll().OrderBy(o => o.Id).Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 join s in dbS.GetAll() on question.Signal.Id equals s.Id
                                 join a in dbA.GetAll() on question.Appliance.Id equals a.Id
                                 join at in dbAT.GetAll() on question.AlarmType.Id equals at.Id
                                 select new { question.Id, question.Tag, question.Description, s.Name, a.NameAppliance, at.NameAlarmType }).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from question in questionDatas
                        select new
                        {
                            id = question.Id,
                            cell = new[] { question.Id.ToString(), question.Tag.ToString(), question.Description, question.Name, question.NameAppliance, question.NameAlarmType }
                        }).ToList()
            };
            return Json(jsonData);
        }

        private MappingTag RemoveExtraSpaces(MappingTag mappingtag)
        {
            mappingtag.Tag = !String.IsNullOrWhiteSpace(mappingtag.Tag) ? mappingtag.Tag.Trim() : null;
            mappingtag.Description = !String.IsNullOrWhiteSpace(mappingtag.Description) ? mappingtag.Description.Trim() : null;

            return mappingtag;
        }

        private bool FormCollectionIsValid(FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(form["Tag"]))
            {
                if (!String.IsNullOrEmpty(form["ApplianceId"]))
                {
                    if (!String.IsNullOrEmpty(form["SignalId"]))
                    {
                        if (!String.IsNullOrEmpty(form["AlarmTypeId"]))
                        {
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
                ViewData["ValidationErrorMessage"] = "Nombre etiqueta: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                return false;
            }
        }
    }
}