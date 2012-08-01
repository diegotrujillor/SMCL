using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMCLCore.Domain.Model;
using SMCL.Models;
using SMCLCore;
using SMCLCore.Domain.Repositories;
using System.Configuration;
using SMCL.Services.LoggingImplementation;
using SMCL.Services.LoggingInterface;
using SMCL.Enums;
using NHibernate.Exceptions;

namespace SMCL.Controllers
{ 
    public class ApplianceController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<Area> dbA = new AreaRepository();
        private IRepository<Appliance> db = new ApplianceRepository();

        //
        // GET: /Appliance/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /Appliance/Details/5

        public ViewResult Details(int id)
        {
            Appliance appliance = db.GetById(id);
            appliance.Area = dbA.GetById(appliance.Area.Id);
            return View(appliance);
        }

        //
        // GET: /Appliance/Create

        public ActionResult Create()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            ViewBag.AreaId = new SelectList(dbA.GetAll(), "Id", "Name");
            
            return View();
        } 

        //
        // POST: /Appliance/Create

        [HttpPost]
        public ActionResult Create(Appliance appliance, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                appliance.Area = dbA.GetById(Convert.ToInt32(form["AreaId"]));
                db.Save(this.RemoveExtraSpaces(appliance));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + appliance.Id + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            ViewBag.AreaId = !String.IsNullOrEmpty(form["AreaId"]) ? new SelectList(dbA.GetAll(), "Id", "Name", form["AreaId"]) : new SelectList(dbA.GetAll(), "Id", "Name");

            return View(appliance);
        }
        
        //
        // GET: /Appliance/Edit/5
 
        public ActionResult Edit(int id)
        {
            Appliance appliance = db.GetById(id);
            appliance.Area = dbA.GetById(appliance.Area.Id);
            ViewBag.AreaId = new SelectList(dbA.GetAll(), "Id", "Name", appliance.Area.Id);
            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(appliance);
        }

        //
        // POST: /Appliance/Edit/5

        [HttpPost]
        public ActionResult Edit(Appliance appliance, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            if (this.FormCollectionIsValid(form))
            {
                appliance.Area = dbA.GetById(Convert.ToInt32(form["AreaId"]));
                db.Update(this.RemoveExtraSpaces(appliance));

                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + appliance.Id + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            ViewBag.AreaId = !String.IsNullOrEmpty(form["AreaId"]) ? new SelectList(dbA.GetAll(), "Id", "Name", form["AreaId"]) : new SelectList(dbA.GetAll(), "Id", "Name");

            return View(appliance);
        }

        //
        // GET: /Appliance/Delete/5
 
        public ActionResult Delete(int id)
        {
            Appliance appliance = db.GetById(id);
            appliance.Area = dbA.GetById(appliance.Area.Id);
            ViewBag.AreaId = new SelectList(dbA.GetAll(), "Id", "Name", appliance.Area.Id);
            ViewData["ValidationErrorMessage"] = String.Empty;
            
            return View(appliance);
        }

        //
        // POST: /Appliance/Delete/5

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

                Appliance entity = db.GetById(id);
                entity.Area = dbA.GetById(entity.Area.Id);

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

            var questions = db.GetAll().OrderBy(o => o.NameAppliance == sidx).Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 join a in dbA.GetAll() on question.Area.Id equals a.Id
                                 select new { question.Id, question.NameAppliance, question.Description, a.Name }).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from question in questionDatas
                        select new
                        {
                            id = question.Id,
                            cell = new[] { question.Id.ToString(), question.NameAppliance.ToString(), question.Description, question.Name }
                        }).ToList()
            };
            return Json(jsonData);
        }

        private Appliance RemoveExtraSpaces(Appliance appliance)
        {
            appliance.NameAppliance = !String.IsNullOrWhiteSpace(appliance.NameAppliance) ? appliance.NameAppliance.Trim() : null;
            appliance.Description = !String.IsNullOrWhiteSpace(appliance.Description) ? appliance.Description.Trim() : null;

            return appliance;
        }

        private bool FormCollectionIsValid(FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(form["NameAppliance"]))
            {
                if (!String.IsNullOrEmpty(form["AreaId"]))
                {
                    return true;
                }
                else
                {
                    ViewData["ValidationErrorMessage"] = "Nombre área: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                    return false;
                }
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Nombre equipo: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                return false;
            }
        }
    }
}