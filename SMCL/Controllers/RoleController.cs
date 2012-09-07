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
    public class RoleController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<Role> db = new RoleRepository();

        //
        // GET: /Role/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /Role/Details/5

        public ViewResult Details(int id)
        {
            Role role = db.GetById(id);
            return View(role);
        }

        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        public ActionResult Create(Role role, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Save(this.RemoveExtraSpaces(role));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + role.Id.ToString().Replace("-", "").ToUpper() + " - Description=" + role.Description + " - Name=" + role.Name + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(role);
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(int id)
        {
            Role role = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(role);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        public ActionResult Edit(Role role, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                db.Update(this.RemoveExtraSpaces(role));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + role.Id.ToString().Replace("-", "").ToUpper() + " - Description=" + role.Description + " - Name=" + role.Name + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(role);
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(int id)
        {
            Role role = db.GetById(id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View(role);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            Role role = db.GetById(id);

            try
            {
                if (role != null)
                {
                    db.Delete(id);
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + 
                                              ControllerContext.RouteData.Values["controller"] + 
                                              "(Id=" + role.Id.ToString().Replace("-", "").ToUpper() + 
                                              " - Description=" + role.Description + 
                                              " - Name=" + role.Name + ")", 
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
                                          "(Id=" + role.Id.ToString().Replace("-", "").ToUpper() +
                                          " - Description=" + role.Description +
                                          " - Name=" + role.Name + ")", 
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

        private Role RemoveExtraSpaces(Role role)
        {
            role.Name = !String.IsNullOrWhiteSpace(role.Name) ? role.Name.Trim() : null;
            role.Description = !String.IsNullOrWhiteSpace(role.Description) ? role.Description.Trim() : null;

            return role;
        }

        private bool FormCollectionIsValid(FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(form["Name"]))
            {
                return true;
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Nombre rol: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
            }

            return false;
        }
    }
}