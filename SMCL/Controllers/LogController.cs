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

namespace SMCL.Controllers
{ 
    public class LogController : Controller
    {
        private IRepository<Log> db = new LogRepository();
        private IRepository<User> dbU = new UserRepository();
        private IRepository<Event> dbE = new EventRepository();

        //
        // GET: /Log/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /Log/Details/5

        public ViewResult Details(int id)
        {
            Log log = db.GetById(id);
            log.Event = dbE.GetById(log.Event.Id);
            log.User = dbU.GetById(log.User.Id);

            return View(log);
        }

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = db.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var logs = db.GetAll().OrderBy(o => o.DateTime).Skip(pageIndex * pageSize);//.Take(pageSize);

            var result = (from log in logs
                          join ev in dbE.GetAll() on log.Event.Id equals ev.Id
                          join u in dbU.GetAll() on log.User.Id equals u.Id
                          select new { log.Id, log.DateTime, EventName = ev.Name, log.Text, User = u.FirstName + " " + u.LastName1 + "(" + u.LoginEmail + ")"}).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from log in result
                        select new
                        {
                            id = log.Id,
                            cell = new[] { log.Id.ToString(), log.DateTime.ToString(), log.EventName, log.Text, log.User }
                        }).ToList()
            };
            return Json(jsonData);
        }
    }
}