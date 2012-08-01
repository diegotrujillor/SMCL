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

namespace SMCL.Controllers
{ 
    public class EventController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<Event> db = new EventRepository();

        //
        // GET: /Event/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /Event/Details/5

        public ViewResult Details(int id)
        {
            Event eventt = db.GetById(id);
            return View(eventt);
        }

        //
        // GET: /Event/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View();
        } 

        //
        // POST: /Event/Create

        [HttpPost]
        public ActionResult Create(Event eventt, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Save(this.RemoveExtraSpaces(eventt));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + eventt.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(eventt);
        }

        //
        // GET: /Event/Edit/5
 
        public ActionResult Edit(int id)
        {
            Event eventt = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(eventt);
        }

        //
        // POST: /Event/Edit/5

        [HttpPost]
        public ActionResult Edit(Event eventt, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Update(this.RemoveExtraSpaces(eventt));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + eventt.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }
            
            return View(eventt);
        }

        //
        // GET: /Event/Delete/5
 
        public ActionResult Delete(int id)
        {
            Event eventt = db.GetById(id);
            return View(eventt);
        }

        //
        // POST: /Event/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            db.Delete(id);

            List<Object> logList = new List<Object>();
            logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + id + ")", (int)EventTypes.Delete, (int)Session["UserId"]));
            log.Write(logList);

            return RedirectToAction("Index");
        }

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = db.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var questions = db.GetAll().OrderBy(o => o.Name == sidx).Skip(pageIndex * pageSize);//.Take(pageSize);

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

        private Event RemoveExtraSpaces(Event eventt)
        {
            eventt.Name = !String.IsNullOrWhiteSpace(eventt.Name) ? eventt.Name.Trim() : null;
            eventt.Description = !String.IsNullOrWhiteSpace(eventt.Description) ? eventt.Description.Trim() : null;

            return eventt;
        }

        private bool FormCollectionIsValid(FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(form["Name"]))
            {
                return true;
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Nombre evento: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
            }

            return false;
        }
    }
}