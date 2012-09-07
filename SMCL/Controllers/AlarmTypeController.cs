using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMCLCore.Domain.Model;
using SMCL.Models;
using SMCLCore.Domain.Repositories;
using SMCLCore;
using System.Configuration;
using SMCL.Services.LoggingImplementation;
using SMCL.Services.LoggingInterface;
using SMCL.Enums;
using NHibernate.Exceptions;

namespace SMCL.Controllers
{
    public class AlarmTypeController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<AlarmType> db = new AlarmTypeRepository();

        //
        // GET: /AlarmType/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /AlarmType/Details/5

        public ViewResult Details(int id)
        {
            AlarmType alarmtype = db.GetById(id);
            return View(alarmtype);
        }

        //
        // GET: /AlarmType/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View();
        }

        //
        // POST: /AlarmType/Create

        [HttpPost]
        public ActionResult Create(AlarmType alarmtype, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Save(this.RemoveExtraSpaces(alarmtype));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + alarmtype.Id.ToString().Replace("-", "").ToUpper() + " - Description=" + alarmtype.Description + " - NameAlarmType=" + alarmtype.NameAlarmType + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(alarmtype);
        }

        //
        // GET: /AlarmType/Edit/5

        public ActionResult Edit(int id)
        {
            AlarmType alarmtype = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(alarmtype);
        }

        //
        // POST: /AlarmType/Edit/5

        [HttpPost]
        public ActionResult Edit(AlarmType alarmtype, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Update(this.RemoveExtraSpaces(alarmtype));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + alarmtype.Id.ToString().Replace("-", "").ToUpper() + " - Description=" + alarmtype.Description + " - NameAlarmType=" + alarmtype.NameAlarmType + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(alarmtype);
        }

        //
        // GET: /AlarmType/Delete/5

        public ActionResult Delete(int id)
        {
            AlarmType alarmtype = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(alarmtype);
        }

        //
        // POST: /AlarmType/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            AlarmType alarmtype = db.GetById(id);

            try
            {
                if (alarmtype != null)
                {
                    db.Delete(id);
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + 
                                              ControllerContext.RouteData.Values["controller"] + 
                                              "(Id=" + alarmtype.Id.ToString().Replace("-", "").ToUpper() + 
                                              " - Description=" + alarmtype.Description + 
                                              " - NameAlarmType=" + alarmtype.NameAlarmType + ")", 
                                              (int)EventTypes.Delete, 
                                              (int)Session["UserId"]));
                    log.Write(logList);
                }
            }
            catch (GenericADOException)
            {
                ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["CannotDeleteHasAssociatedRecords"];

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] +
                                          ConfigurationManager.AppSettings["CannotDeleteHasAssociatedRecords"] + " " + 
                                          ControllerContext.RouteData.Values["controller"] + 
                                          "(Id=" + alarmtype.Id.ToString().Replace("-", "").ToUpper() + 
                                          " - Description=" + alarmtype.Description + 
                                          " - NameAlarmType=" + alarmtype.NameAlarmType + ")", 
                                          (int)EventTypes.Delete, 
                                          (int)Session["UserId"]));
                log.Write(logList);

                return View(db.GetById(id));
            }
            catch (Exception ex)
            {
                ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["UnknownError"];

                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ex.InnerException.Message, (int)EventTypes.Delete, (int)Session["UserId"]));
                log.Write(logList);

                return View(db.GetById(id));
            }

            return RedirectToAction("Index");
        }

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = db.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var questions = db.GetAll().OrderBy(o => o.NameAlarmType == sidx).Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 select new { question.Id, Name = question.NameAlarmType, question.Description }).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from question in questionDatas
                        select new
                        {
                            id = question.Id,
                            cell = new[] { question.Id.ToString(), question.Name.ToString(), question.Description }
                        }).ToList()
            };
            return Json(jsonData);
        }

        private AlarmType RemoveExtraSpaces(AlarmType alarmtype)
        {
            alarmtype.NameAlarmType = !String.IsNullOrWhiteSpace(alarmtype.NameAlarmType) ? alarmtype.NameAlarmType.Trim() : null;
            alarmtype.Description = !String.IsNullOrWhiteSpace(alarmtype.Description) ? alarmtype.Description.Trim() : null;

            return alarmtype;
        }

        private bool FormCollectionIsValid(FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(form["NameAlarmType"]))
            {
                return true;
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Nombre tipo alarma: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
            }

            return false;
        }
    }
}