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

            var questions = db.GetAll().OrderBy(o => o.DateTime).Skip(pageIndex * pageSize);//.Take(pageSize);

            var questionDatas = (from question in questions
                                 select new { question.Id, question.DateTime, question.Text }).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from question in questionDatas
                        select new
                        {
                            id = question.Id,
                            cell = new[] { question.Id.ToString(), question.DateTime.ToString(), question.Text }
                        }).ToList()
            };
            return Json(jsonData);
        }
    }
}