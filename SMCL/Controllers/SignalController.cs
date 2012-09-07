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
using SMCL.Services.LoggingInterface;
using SMCL.Services.LoggingImplementation;
using SMCL.Enums;
using NHibernate.Exceptions;

namespace SMCL.Controllers
{
    public class SignalController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<Signal> db = new SignalRepository();

        //
        // GET: /Signal/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /Signal/Details/5

        public ViewResult Details(int id)
        {
            Signal signal = db.GetById(id);
            return View(signal);
        }

        //
        // GET: /Signal/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View();
        }

        //
        // POST: /Signal/Create

        [HttpPost]
        public ActionResult Create(Signal signal, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Save(this.RemoveExtraSpaces(signal));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signal.Id.ToString().Replace("-", "").ToUpper() + " - Description=" + signal.Description + " - Name=" + signal.Name + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(signal);
        }

        //
        // GET: /Signal/Edit/5

        public ActionResult Edit(int id)
        {
            Signal signal = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(signal);
        }

        //
        // POST: /Signal/Edit/5

        [HttpPost]
        public ActionResult Edit(Signal signal, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Update(this.RemoveExtraSpaces(signal));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + signal.Id.ToString().Replace("-", "").ToUpper() + " - Description=" + signal.Description + " - Name=" + signal.Name + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(signal);
        }

        //
        // GET: /Signal/Delete/5

        public ActionResult Delete(int id)
        {
            Signal signal = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(signal);
        }

        //
        // POST: /Signal/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            Signal signal = db.GetById(id);

            try
            {
                if (signal != null)
                {
                    db.Delete(id);
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + 
                                              ControllerContext.RouteData.Values["controller"] + 
                                              "(Id=" + signal.Id.ToString().Replace("-", "").ToUpper() + 
                                              " - Description=" + signal.Description + 
                                              " - Name=" + signal.Name + ")", 
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
                                          "(Id=" + signal.Id.ToString().Replace("-", "").ToUpper() +
                                          " - Description=" + signal.Description +
                                          " - Name=" + signal.Name + ")", 
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

            var questions = db.GetAll().Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 select new { question.Id, question.Name, question.Description }).OrderBy(o => o.Id).ToList();

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

        private Signal RemoveExtraSpaces(Signal signal)
        {
            signal.Name = !String.IsNullOrWhiteSpace(signal.Name) ? signal.Name.Trim() : null;
            signal.Description = !String.IsNullOrWhiteSpace(signal.Description) ? signal.Description.Trim() : null;

            return signal;
        }

        private bool FormCollectionIsValid(FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(form["Name"]))
            {
                return true;
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Nombre señal: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
            }

            return false;
        }
    }
}