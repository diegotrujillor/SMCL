using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using SMCL.Enums;
using SMCL.Models;
using SMCL.Utilities;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;
using SMCL.Services.LoggingInterface;
using SMCL.Services.LoggingImplementation;
using NHibernate;
using Oracle.DataAccess.Client;
using System.Web.Configuration;
using System.Data;
using NHibernate.Mapping;

namespace SMCL.Controllers
{
    public class HomeController : Controller
    {
        ILoggable log = new LogSMCL();
        IRepository<User> db = new UserRepository();
        IRepository<UserRole> dbUR = new UserRoleRepository();
        IRepository<Monitoring> dbM = new MonitoringRepository();

        [AllowAnonymous]
        public ActionResult Intro()
        {
            ViewData["IntroTitle"] = ConfigurationManager.AppSettings["IntroTitle"];
            ViewData["IntroButtonText"] = ConfigurationManager.AppSettings["IntroButtonText"];
            return View();
        }

        public ActionResult Inbox()
        {
            this.InitializeInbox();
            return PartialView("Inbox");
        }

        public ActionResult InboxClock()
        {
            ViewBag.DateFormatted = this.GetDateFormatted(DateTime.Now, "F");
            return PartialView("InboxClock");
        }

        public ActionResult Index()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;

            if (Session["Id"] != null)
            {
                if (!Security.IsOptionInUserRoles(dbUR.GetByUserId(Convert.ToInt32(Session["Id"].ToString())), this.ControllerContext.RouteData.Values["Action"].ToString()))
                {
                    return RedirectToAction("Error");
                }
                else
                {
                    User user = db.GetById(Convert.ToInt32(Session["UserId"]));
                    if (user.IsLoggedFirstTime)
                    {
                        ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["FirstTimeUserMessage"].ToString();
                        user.IsLoggedFirstTime = false;
                        db.Update(user);
                    }

                    this.InitializeIndex();
                    if (!String.IsNullOrEmpty(Request.Params["Callback"]))
                    {
                        this.HandleCallbacks();

                        string requestJsonParam = "";
                        byte[] b = new byte[Request.ContentLength];
                        Request.InputStream.Read(b, 0, Request.ContentLength);
                        requestJsonParam = System.Text.UTF8Encoding.UTF8.GetString(b);
                    }
                }
            }

            return View();
        }

        public ActionResult About()
        {
            if (Session["Id"] != null)
            {
                if (!Security.IsOptionInUserRoles(dbUR.GetByUserId(Convert.ToInt32(Session["Id"].ToString())), this.ControllerContext.RouteData.Values["Action"].ToString()))
                {
                    return RedirectToAction("Error");
                }
            }

            return View();
        }

        public ActionResult Reporting()
        {
            if (Session["Id"] != null)
            {
                if (!Security.IsOptionInUserRoles(dbUR.GetByUserId(Convert.ToInt32(Session["Id"].ToString())), this.ControllerContext.RouteData.Values["Action"].ToString()))
                {
                    return RedirectToAction("Error");
                }
            }

            return View();
        }

        public ActionResult Management()
        {
            if (Session["Id"] != null)
            {
                if (!Security.IsOptionInUserRoles(dbUR.GetByUserId(Convert.ToInt32(Session["Id"].ToString())), this.ControllerContext.RouteData.Values["Action"].ToString()))
                {
                    return RedirectToAction("Error");
                }
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOn()
        {
            ViewData["ValidationErrorMessage"] = String.Empty;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOn(User user, FormCollection form)
        {
            if (this.FormCollectionIsValid(user, form))
            {
                FormsAuthentication.SetAuthCookie(user.LoginEmail, true);

                User userVar = db.GetByUserLogin(user.LoginEmail);

                Session["UserId"] = userVar.Id;
                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["LoginText"], (int)EventTypes.Login, userVar.Id));
                log.Write(logList);

                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            if (Session["UserId"] != null)
            {
                List<Object> logList = new List<Object>();
                logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["LogoutText"], (int)EventTypes.Logout, (int)Session["UserId"]));
                log.Write(logList);
            }

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("LogOn");
        }

        public ActionResult Error()
        {
            ViewBag.ErrorMessage = ConfigurationManager.AppSettings["OptionErrorMessage"].ToString();
            ViewBag.ErrorDescription = ConfigurationManager.AppSettings["OptionErrorDescription"].ToString();

            return View();
        }

        private void InitializeInbox()
        {
            ViewBag.MonApplianceName = ConfigurationManager.AppSettings["MonApplianceName"].ToString();
            ViewBag.MonAlarmTypeName = ConfigurationManager.AppSettings["MonAlarmTypeName"].ToString();
            ViewBag.DateTitle = ConfigurationManager.AppSettings["DateTitle"].ToString();
            ViewBag.MonValue = ConfigurationManager.AppSettings["MonValue"].ToString();
            ViewBag.MonSignalName = ConfigurationManager.AppSettings["MonSignalName"].ToString();
            ViewBag.SenderTitle = ConfigurationManager.AppSettings["SenderTitle"].ToString();
            ViewBag.SubjectTitle = ConfigurationManager.AppSettings["SubjectTitle"].ToString();
        }

        private void InitializeIndex()
        {
            ViewBag.InboxTitle = ConfigurationManager.AppSettings["InboxTitle"].ToString();
            ViewBag.AlarmCheckOut = ConfigurationManager.AppSettings["AlarmCheckOutTitle"].ToString();
            ViewBag.AlarmPlaceHolder = ConfigurationManager.AppSettings["AlarmPlaceHolder"].ToString();
            ViewBag.AlarmSaveButton = ConfigurationManager.AppSettings["AlarmSaveButton"].ToString();
            ViewBag.AlarmCancelButton = ConfigurationManager.AppSettings["AlarmCancelButton"].ToString();
            ViewBag.Message = ConfigurationManager.AppSettings["WelcomeMessage"].ToString();
            ViewBag.FullName = this.GetFullName(Session["FirstName"].ToString(), Session["MiddleName"].ToString(), Session["LastName"].ToString());
        }

        private void HandleCallbacks()
        {
            switch (Request.Params["Callback"])
            {
                case "saveCommentsOnAlarm":
                    this.SaveCommentsOnAlarm(Request.Params["monitorId"].ToString(), Request.Params["commentsOnAlarm"].ToString());
                    break;

                default:
                    Response.StatusCode = 500;
                    Response.Write("Invalid Callback Method");
                    break;
            }

            Response.End();
        }

        public JsonResult FillGridReturnList(JQueryDataTableParamModel param)
        {
            IRepository<AlarmType> dbAT = new AlarmTypeRepository();
            IRepository<MappingTag> dbMT = new MappingTagRepository();
            IRepository<Signal> dbSgn = new SignalRepository();
            IRepository<Appliance> dbAppl = new ApplianceRepository();

            var monRecords = from r in
                                 (  from mon in dbM.GetAll()
                                    join mapp in dbMT.GetAll() on mon.MappingTag.Id equals mapp.Id
                                  select new { monDatetime = mon.DateTime, appId = mapp.Appliance.Id, sigId = mapp.Signal.Id, pv = (mapp.AlarmType.Id.Equals(Convert.ToInt32("1")) ? mon.Value : 0), alarm = (mon.Value == 1 && !mapp.AlarmType.Id.Equals(Convert.ToInt32("1")) ? mapp.AlarmType.Id : 0), userId = mon.User.Id })
                            group r by new { r.monDatetime, r.appId, r.sigId, r.userId } into g
                           select new { monRecord = g.Key, monValue = g.Sum(d => d.pv), alarm = g.Sum(d => d.alarm) };

            var monitoring = from m in monRecords
                         join mon in dbM.GetAll() on m.monRecord.monDatetime equals mon.DateTime
                         join mapp in dbMT.GetAll() on new { mapId = mon.MappingTag.Id, appId = m.monRecord.appId, sigId = m.monRecord.sigId } equals new { mapId = mapp.Id, appId = mapp.Appliance.Id, sigId = mapp.Signal.Id }
                         where (m.alarm == 0 ? 1 : m.alarm) == mapp.AlarmType.Id
                               && mon.CommentsOnAlarm == null
                               && (m.alarm == Convert.ToInt32(ConfigurationManager.AppSettings["HighAlarmId"])
                                   || m.alarm == Convert.ToInt32(ConfigurationManager.AppSettings["LowAlarmId"]))
                         select new { monId = mon.Id, dateTime = m.monRecord.monDatetime, appId = m.monRecord.appId, sigId = m.monRecord.sigId, alarm = m.alarm, monValue = m.monValue, userId = m.monRecord.userId };

            var items = monitoring.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in
                             items.OrderByDescending(d => d.dateTime)
                         select new[] {
                             c.dateTime.ToString(),
                             dbAppl.GetById(c.appId).NameAppliance,
                             dbAT.GetById((c.alarm == 0 ? Convert.ToInt32(ConfigurationManager.AppSettings["NormalAlarmId"]) : c.alarm)).NameAlarmType,
                             c.monValue.ToString(),
                             dbSgn.GetById(c.sigId).Name,
                             c.monId.ToString()
                         };

            return Json(new
                            {
                                sEcho = param.sEcho,
                                iTotalRecords = monitoring.Count(),
                                iTotalDisplayRecords = monitoring.Count(),
                                aaData = result
                            },
                        JsonRequestBehavior.AllowGet);

        }

        public JsonResult GraphDataList()
        {
            var jsonResult = new Object();
            List<KeyValuePair<object, object>> rawList = new List<KeyValuePair<object, object>>();

            switch (Request.Params["Callback"])
            {
                case "paintGraph":
                    var iArea = Request.Params["iArea"].ToString();
                    var iAppliance = Request.Params["iAppliance"].ToString();
                    var iAlarm = Request.Params["iAlarm"].ToString();
                    var iSignal = Request.Params["iSignal"].ToString();
                    DateTime iStartDate = Convert.ToDateTime(Request.Params["iStartDate"].ToString());
                    TimeSpan tsStartDate = new TimeSpan(0, 0, 0);
                    DateTime iFinalDate = Convert.ToDateTime(Request.Params["iFinalDate"].ToString());
                    TimeSpan tsFinalDate = new TimeSpan(23, 59, 59);

                    iStartDate = iStartDate.Date + tsStartDate;
                    iFinalDate = iFinalDate.Date + tsFinalDate;

                    OracleDataReader odr;

                    try
                    {
                        using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ReportsMonitoringData"].ConnectionString))
                        {
                            conn.Open();

                            OracleCommand cmd = new OracleCommand("SANPRO.SMCL_GET_SIGNALS_DATA", conn);
                            cmd.BindByName = true;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.Add("AREA_PARAM", OracleDbType.Int32, iArea, ParameterDirection.Input);
                            cmd.Parameters.Add("APPLIANCE_PARAM", OracleDbType.Int32, iAppliance, ParameterDirection.Input);
                            cmd.Parameters.Add("ALARM_PARAM", OracleDbType.Int32, iAlarm, ParameterDirection.Input);
                            cmd.Parameters.Add("SIGNAL_PARAM", OracleDbType.Int32, iSignal, ParameterDirection.Input);
                            cmd.Parameters.Add("INIT_DATE_PARAM", OracleDbType.Varchar2, iStartDate.ToString("yyyy/MM/dd"), ParameterDirection.Input);
                            cmd.Parameters.Add("END_DATE_PARAM", OracleDbType.Varchar2, iFinalDate.ToString("yyyy/MM/dd"), ParameterDirection.Input);
                            cmd.Parameters.Add("RESULTADO_C", OracleDbType.RefCursor, ParameterDirection.Output);

                            odr = cmd.ExecuteReader();

                            int number;
                            if (odr.Read())
                            {
                                if (int.TryParse(EventTypes.Report.ToString(), out number) && int.TryParse(Session["UserId"].ToString(), out number))
                                {
                                    List<Object> logList = new List<Object>();
                                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["ReportText"] + "Gráficas de tendencia", Convert.ToInt32(EventTypes.Report), Convert.ToInt32(Session["UserId"])));
                                    log.Write(logList);
                                }
                            }

                            while (odr.Read())
                            {
                                rawList.Add(new KeyValuePair<object, object>(odr.GetValue(0).ToString(), odr.GetValue(6).ToString()));
                            }
                            var tempList = rawList.ToList();

                            jsonResult = from c in tempList
                                     select new[]{
                                         Convert.ToDateTime(c.Key.ToString()).ToString("MM/dd/yyyy HH:mm:ss"),
                                         c.Value.ToString()
                                     };
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;

                default:
                    break;
            }

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        private string GetAlarmSubject(int alarmTypeId)
        {
            if (alarmTypeId == Convert.ToInt32(ConfigurationManager.AppSettings["NormalAlarmId"]))
            {
                return ConfigurationManager.AppSettings["NormalMonitoringInboxSubject"].ToString();
            }

            if (alarmTypeId == Convert.ToInt32(ConfigurationManager.AppSettings["HighAlarmId"]))
            {
                return ConfigurationManager.AppSettings["HighMonitoringInboxSubject"].ToString();
            }

            if (alarmTypeId == Convert.ToInt32(ConfigurationManager.AppSettings["LowAlarmId"]))
            {
                return ConfigurationManager.AppSettings["LowMonitoringInboxSubject"].ToString();
            }

            return ConfigurationManager.AppSettings["UnknownMonitoringInboxSubject"].ToString();
        }

        private dynamic GetFullName(string firstName, string middleName, string lastName)
        {
            if (!String.IsNullOrEmpty(middleName.ToString()))
            {
                return UpperCaseFirst(firstName) + " " + UpperCaseFirst(middleName.Substring(0, 1)) + ". " + UpperCaseFirst(lastName);
            }
            else
            {
                return UpperCaseFirst(firstName) + " " + UpperCaseFirst(lastName);
            }
        }

        private static string UpperCaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        private void SaveCommentsOnAlarm(string monitorId, string commentsOnAlarm)
        {
            double numberOut;
            Monitoring monitor = new Monitoring();
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            if (!String.IsNullOrEmpty(monitorId) && !String.IsNullOrWhiteSpace(commentsOnAlarm))
            {
                if (double.TryParse(monitorId.ToString(), out numberOut))
                {
                    monitor = dbM.GetById(Convert.ToInt32(monitorId));
                    monitor.CommentsOnAlarm = commentsOnAlarm;

                    dbM.Update(monitor);

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["NotifyAlarmText"], (int)EventTypes.Notify, (int)Session["UserId"]));
                    log.Write(logList);
                }
            }

            Response.ContentType = "application/json; charset=utf-8";
        }

        private string GetDateFormatted(DateTime dateTime, string format)
        {
            return this.CapitalizeWords(dateTime.ToString(format, new CultureInfo(ConfigurationManager.AppSettings["CultureInfo"].ToString()))).Replace("De", "de");
        }

        private string CapitalizeWords(string phrase)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(phrase);
        }

        public JsonResult GetAlarmValue()
        {
            var result = new Object();
            IRepository<SignalAppliance> dbSA = new SignalApplianceRepository();
            IRepository<SignalApplianceValue> dbSAV = new SignalApplianceValueRepository();

            switch (Request.Params["Callback"])
            {
                case "alarmValues":
                    var iAppliance = Request.Params["iAppliance"].ToString();
                    var iAlarm = Request.Params["iAlarm"].ToString();
                    var iSignal = Request.Params["iSignal"].ToString();

                    Dictionary<string, object> properties = new Dictionary<string, object>();
                    properties.Add("Signal.Id", int.Parse(iSignal));
                    properties.Add("Appliance.Id", int.Parse(iAppliance));

                    SignalAppliance signalAppliance = dbSA.GetByProperties(properties).FirstOrDefault();

                    if (signalAppliance == null)
                    {
                        return Json("undefined", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        properties = new Dictionary<string, object>();
                        properties.Add("SignalAppliance.Id", signalAppliance.Id);
                        properties.Add("AlarmType.Id", int.Parse(iAlarm));

                        SignalApplianceValue signalApplianceValue = dbSAV.GetByProperties(properties).FirstOrDefault();
                        if (signalApplianceValue == null)
                        {
                            return Json("undefined", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(signalApplianceValue.Value.ToString(), JsonRequestBehavior.AllowGet);
                        }
                    }

                default:
                    break;

            }

            return Json("undefined", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GetPassphrase()
        {
            IRepository<User> dbU = new UserRepository();

            switch (Request.Params["Callback"])
            {
                case "values":
                    var users = dbU.GetByProperty("DocumentId", double.Parse(Request.Params["iDocumentId"]));

                    if (!users.Any())
                    {
                        return Json("documentIdNotFound", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        UserController user = new UserController();
                        return Json(user.GetPassphraseById(users.First().PassphraseId), JsonRequestBehavior.AllowGet);
                    }

                default:
                    break;
            }

            return Json("undefined", JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult RememberPassword()
        {
            IRepository<User> dbU = new UserRepository();

            try
            {
                switch (Request.Params["Callback"])
                {
                    case "values":
                        var users = dbU.GetByProperty("DocumentId", Convert.ToDouble(Request.Params["iDocumentId"]));

                        if (!users.Any())
                        {
                            return Json("documentIdNotFound", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var userPassphraseValue = users.First().PassphraseValue;
                            if (!String.Equals(userPassphraseValue, new Cryptography().EncryptSHA1(Request.Params["iPassphraseValue"])))
                            {
                                return Json("invalidPassphraseValue", JsonRequestBehavior.AllowGet);
                            }

                            var newPassword = this.ResetPassword(Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordLength"]));
                            users.First().Password = new Cryptography().EncryptSHA1(newPassword);
                            dbU.Update(users.FirstOrDefault());

                            List<Object> logList = new List<Object>();
                            logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["ChangePasswordText"], (int)EventTypes.Password, users.First().Id));
                            log.Write(logList);

                            return Json(new
                            {
                                users.First().LoginEmail,
                                newPassword
                            },
                            JsonRequestBehavior.AllowGet);
                        }

                    default:
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Json("undefined", JsonRequestBehavior.AllowGet);
        }

        private string ResetPassword(int length)
        {
            /*Thanks to http://www.code-tips.com/2010/08/c-generate-random-string-of-specific.html*/

            //Initiate objects & vars
            Random random = new Random();
            String randomString = "";

            int randNumber;

            //Loop ‘length’ times to generate a random number or character
            for (int i = 0; i < length; i++)
            {
                if (random.Next(1, 3) == 1)
                {
                    randNumber = random.Next(97, 123); //char {a-z}
                }
                else
                {
                    randNumber = random.Next(48, 58); //int {0-9}
                }

                //append random char or digit to random string
                randomString = randomString + (char)randNumber;
            }

            //return the random string
            return randomString;
        }

        private bool FormCollectionIsValid(SMCLCore.Domain.Model.User user, FormCollection form)
        {
            Cryptography access = new Cryptography();
            if (!String.IsNullOrWhiteSpace(form["LoginEmail"]) && !String.IsNullOrWhiteSpace(form["Password"]))
            {
                var entity = db.GetByUserLogin(form["LoginEmail"]);
                if (entity != null)
                {
                    user = db.GetById(entity.Id);
                    if (String.Equals(user.LoginEmail, form["LoginEmail"]) && String.Equals(user.Password, access.EncryptSHA1(form["Password"].ToString())))
                    {
                        if (user.IsActive)
                        {
                            Session["Id"] = user.Id;
                            Session["FirstName"] = user.FirstName;
                            Session["MiddleName"] = !String.IsNullOrEmpty(user.MiddleName) ? user.MiddleName : String.Empty;
                            Session["LastName"] = user.LastName1;

                            return true;
                        }
                        else
                        {
                            ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["InactiveUserErrorMessage"].ToString();
                            return false;
                        }
                    }
                    else
                    {
                        Session["WrongPasswordAttempts"] = Convert.ToInt32(Session["WrongPasswordAttempts"]) + 1;
                        if (Convert.ToInt32(Session["WrongPasswordAttempts"]) > Convert.ToInt32(ConfigurationManager.AppSettings["AmountWrongPasswordAttempts"]))
                        {
                            user.IsActive = false;
                            db.Update(user);
                            ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["AmountWrongPasswordAttemptsErrorMessage"].ToString();
                            Session["WrongPasswordAttempts"] = 0;

                            List<Object> logList = new List<Object>();
                            logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["BlockAccountText"], (int)EventTypes.Account, (int)Session["UserId"]));
                            log.Write(logList);

                            return false;
                        }
                        else
                        {
                            ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["WrongPasswordErrorMessage"].ToString();
                            return false;
                        }
                    }
                }
                else
                {
                    ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["NonExistentLoginEmailErrorMessage"].ToString();
                    return false;
                }
            }
            else
            {
                ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["EmptyUserOrPasswordErrorMessage"].ToString();
                return false;
            }
        }
    }
}