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
using SMCL.Utilities;
using System.Text.RegularExpressions;
using SMCL.Services.LoggingImplementation;
using SMCL.Services.LoggingInterface;
using SMCL.Enums;
using NHibernate.Exceptions;

namespace SMCL.Controllers
{
    public class UserController : Controller
    {
        ILoggable log = new LogSMCL();
        private IRepository<User> db = new UserRepository();
        private IRepository<Role> dbR = new RoleRepository();
        private IRepository<UserRole> dbUR = new UserRoleRepository();

        private const int ActionCreate = 1;
        private const int ActionUpdate = 2;

        //
        // GET: /User/

        public ViewResult Index()
        {
            return View(db.GetAll());
        }

        //
        // GET: /User/Details/5

        public ViewResult Details(int id)
        {
            User user = db.GetById(id);
            List<int> userRoles = new List<int>();

            ViewData["urlImage"] = this.GetUrlImage(user);

            foreach (UserRole userRole in dbUR.GetByUserId(id))
            {
                userRoles.Add(userRole.Role.Id);
            }
            IList<Role> roles = dbR.GetAll();
            MultiSelectList msl = new MultiSelectList(roles, "Id", "Name", userRoles);
            ViewData["Roles"] = msl;

            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            ViewBag.IsActive = true;
            IList<Role> roles = dbR.GetAll();
            MultiSelectList mslRoles = new MultiSelectList(roles, "Id", "Name", this.GetActiveRolesByDefault());
            ViewData["Roles"] = mslRoles;

            Passphrase passphrase;
            var passphrases = this.GetPassphrases();
            IList<Passphrase> listPassphrases = new List<Passphrase>();
            foreach (var item in passphrases)
            {
                passphrase = new Passphrase();
                passphrase.id = int.Parse(item.Substring(0, 1));
                passphrase.value = item.Substring(1).Trim();
                listPassphrases.Add(passphrase);
            }
            MultiSelectList mslPassphrases = new MultiSelectList(listPassphrases, "id", "value");
            ViewData["Passphrases"] = mslPassphrases;

            ViewData["ValidationErrorMessage"] = String.Empty;

            return View();
        }

        public string[] GetPassphrases()
        {
            char[] delimiter = ConfigurationManager.AppSettings["PassphrasesKeysSeparator"].ToCharArray();
            var passphrases = ConfigurationManager.AppSettings["PassphrasesKeys"].Split(delimiter, StringSplitOptions.None);

            return passphrases;
        }

        public string GetPassphraseById(int id)
        {
            char[] delimiter = ConfigurationManager.AppSettings["PassphrasesKeysSeparator"].ToCharArray();
            var passphrases = ConfigurationManager.AppSettings["PassphrasesKeys"].Split(delimiter, StringSplitOptions.None);

            foreach (var passphrase in passphrases)
            {
                if (int.Parse(passphrase.Substring(0, 1)) == id)
                {
                    return passphrase.Substring(1);
                }
            }

            return String.Empty;
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user, FormCollection form)
        {

            ViewData["ValidationErrorMessage"] = String.Empty;

            try
            {
                if (this.FormCollectionToCreateIsValid(user, form))
                {
                    user.Password = new Cryptography().EncryptSHA1(form["passwordPwd"]);
                    user.PassphraseId = int.Parse(form["PassphraseId"]);
                    user.PassphraseValue = new Cryptography().EncryptSHA1(form["passphraseValue"]);

                    db.Save(this.RemoveExtraSpaces(user));

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["CreateText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + user.Id.ToString().Replace("-", "").ToUpper() + " - DocumentId=" + user.DocumentId + " - LoginEmail=" + user.LoginEmail + " - FirstName=" + user.FirstName + " - MiddleName=" + user.MiddleName + " - LastName1=" + user.LastName1 + " - LastName2=" + user.LastName2 + " - PhoneNumber=" + user.PhoneNumber + ")", (int)EventTypes.Create, (int)Session["UserId"]));
                    log.Write(logList);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Passphrase passphrase;
            var passphrases = this.GetPassphrases();
            IList<Passphrase> listPassphrases = new List<Passphrase>();
            foreach (var item in passphrases)
            {
                passphrase = new Passphrase();
                passphrase.id = int.Parse(item.Substring(0, 1));
                passphrase.value = item.Substring(1).Trim();
                listPassphrases.Add(passphrase);
            }
            MultiSelectList mslPassphrases = new MultiSelectList(listPassphrases, "id", "value");
            ViewData["Passphrases"] = mslPassphrases;

            ViewData["Roles"] = this.GetRolesInList(form, ActionCreate);

            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            User user = db.GetById(id);
            List<int> userRoles = new List<int>();
            foreach (UserRole userRole in dbUR.GetByUserId(id))
            {
                userRoles.Add(userRole.Role.Id);
            }
            IList<Role> roles = dbR.GetAll();
            MultiSelectList msl = new MultiSelectList(roles, "Id", "Name", userRoles);
            ViewData["Roles"] = msl;

            Passphrase passphrase;
            char[] delimiter = ConfigurationManager.AppSettings["PassphrasesKeysSeparator"].ToCharArray();
            var passphrases = ConfigurationManager.AppSettings["PassphrasesKeys"].Split(delimiter, StringSplitOptions.None);
            IList<Passphrase> listPassphrases = new List<Passphrase>();
            foreach (var item in passphrases)
            {
                passphrase = new Passphrase();
                passphrase.id = int.Parse(item.Substring(0, 1));
                passphrase.value = item.Substring(1).Trim();
                listPassphrases.Add(passphrase);
            }

            passphrase = new Passphrase();
            passphrase.id = user.PassphraseId;
            passphrase.value = user.PassphraseValue;
            IList<Passphrase> listDefault = new List<Passphrase>();
            listDefault.Add(passphrase);

            MultiSelectList mslPassphrases = new MultiSelectList(listPassphrases, "id", "value", listDefault);
            ViewData["Passphrases"] = mslPassphrases;

            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User user, FormCollection form)
        {
            ViewData["ValidationErrorMessage"] = String.Empty;

            try
            {
                if (this.FormCollectionToUpdateIsValid(user, form))
                {
                    user.Password = new Cryptography().EncryptSHA1(form["passwordPwd"]);
                    user.PassphraseId = int.Parse(form["PassphraseId"]);
                    user.PassphraseValue = new Cryptography().EncryptSHA1(form["passphraseValue"]);

                    db.Update(this.RemoveExtraSpaces(user));

                    List<Object> logList = new List<Object>();
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["EditText"] + ControllerContext.RouteData.Values["controller"] + "(Id=" + user.Id.ToString().Replace("-", "").ToUpper() + " - DocumentId=" + user.DocumentId + " - LoginEmail=" + user.LoginEmail + " - FirstName=" + user.FirstName + " - MiddleName=" + user.MiddleName + " - LastName1=" + user.LastName1 + " - LastName2=" + user.LastName2 + " - PhoneNumber=" + user.PhoneNumber + ")", (int)EventTypes.Edit, (int)Session["UserId"]));
                    log.Write(logList);

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Passphrase passphrase;
            var passphrases = this.GetPassphrases();
            IList<Passphrase> listPassphrases = new List<Passphrase>();
            foreach (var item in passphrases)
            {
                passphrase = new Passphrase();
                passphrase.id = int.Parse(item.Substring(0, 1));
                passphrase.value = item.Substring(1).Trim();
                listPassphrases.Add(passphrase);
            }
            MultiSelectList mslPassphrases = new MultiSelectList(listPassphrases, "id", "value");
            ViewData["Passphrases"] = mslPassphrases;

            ViewData["Roles"] = this.GetRolesInList(form, ActionUpdate);

            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id)
        {
            User user = db.GetById(id);
            List<int> userRoles = new List<int>();

            ViewData["urlImage"] = this.GetUrlImage(user);

            foreach (UserRole userRole in dbUR.GetByUserId(id))
            {
                userRoles.Add(userRole.Role.Id);
            }
            IList<Role> roles = dbR.GetAll();
            MultiSelectList msl = new MultiSelectList(roles, "Id", "Name", userRoles);
            ViewData["Roles"] = msl;

            ViewData["ValidationErrorMessage"] = String.Empty;

            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Object> logList = new List<Object>();
            ViewData["ValidationErrorMessage"] = String.Empty;

            User user = db.GetById(id);

            try
            {
                if (user != null)
                {
                    db.Delete(id);
                    logList.Add(log.GetNewLog(ConfigurationManager.AppSettings["DeleteText"] + 
                                              ControllerContext.RouteData.Values["controller"] + 
                                              "(Id=" + user.Id.ToString().Replace("-", "").ToUpper() + 
                                              " - DocumentId=" + user.DocumentId + 
                                              " - LoginEmail=" + user.LoginEmail + 
                                              " - FirstName=" + user.FirstName + 
                                              " - MiddleName=" + user.MiddleName + 
                                              " - LastName1=" + user.LastName1 + 
                                              " - LastName2=" + user.LastName2 + 
                                              " - PhoneNumber=" + user.PhoneNumber + ")", 
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
                                          "(Id=" + user.Id.ToString().Replace("-", "").ToUpper() +
                                          " - DocumentId=" + user.DocumentId +
                                          " - LoginEmail=" + user.LoginEmail +
                                          " - FirstName=" + user.FirstName +
                                          " - MiddleName=" + user.MiddleName +
                                          " - LastName1=" + user.LastName1 +
                                          " - LastName2=" + user.LastName2 +
                                          " - PhoneNumber=" + user.PhoneNumber + ")", 
                                          (int)EventTypes.Delete, 
                                          (int)Session["UserId"]));
                log.Write(logList);

                ViewData["urlImage"] = this.GetUrlImage(db.GetById(id));

                User entity = db.GetById(id);
                List<int> userRoles = new List<int>();
                foreach (UserRole userRole in dbUR.GetByUserId(id))
                {
                    userRoles.Add(userRole.Role.Id);
                }
                IList<Role> roles = dbR.GetAll();
                MultiSelectList msl = new MultiSelectList(roles, "Id", "Name", userRoles);
                ViewData["Roles"] = msl;

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

        private System.Collections.IEnumerable GetActiveRolesByDefault()
        {
            List<int> rolesArray;
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["ActiveRolesByDefault"].ToString()))
            {
                double number;
                rolesArray = new List<int>();
                var items = ConfigurationManager.AppSettings["ActiveRolesByDefault"];
                foreach (Object item in items)
                {
                    rolesArray.Add(double.TryParse(item.ToString(), out number) ? Convert.ToInt32(item.ToString()) : 0);
                }
                return rolesArray;
            }

            return new int[] { 0 };
        }

        public JsonResult DynamicGridData(string sidx, string sord, int page, int rows)
        {
            var pageIndex = page - 1;
            var pageSize = rows;
            var totalRecords = db.GetAll().Count;
            var totalPages = (totalRecords + pageSize - 1) / pageSize;

            var questions = db.GetAll().Skip(pageIndex * pageSize);//.Take(pageSize);
            //var userRoles = dbUR.GetAll();

            var questionDatas = (from question in questions
                                 //from userrole in userRoles
                                 //join r in dbR.GetAll() on userrole.Role.Id equals r.Id
                                 //join u in db.GetAll() on userrole.User.Id equals u.Id
                                 select new
                                 {
                                     question.Id,
                                     question.DocumentId,
                                     question.LoginEmail,
                                     question.FirstName,
                                     question.MiddleName,
                                     question.LastName1,
                                     question.LastName2,
                                     question.IsActive,
                                     question.PhoneNumber//,
                                     //r.Name
                                 }).OrderBy(o => o.Id).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from question in questionDatas
                        select new
                        {
                            id = question.Id,
                            cell = new[] { question.Id.ToString(), 
                                           question.DocumentId.ToString(), 
                                           question.LoginEmail, 
                                           question.FirstName, 
                                           question.MiddleName, 
                                           question.LastName1, 
                                           question.LastName2, 
                                           question.IsActive.ToString(),
                                           question.PhoneNumber//, 
                                           //question.Name 
                                         }
                        }).ToList()
            };
            return Json(jsonData);
        }

        private User RemoveExtraSpaces(User user)
        {
            user.DocumentId = !String.IsNullOrWhiteSpace(user.DocumentId.ToString()) ? Convert.ToDouble(user.DocumentId.ToString().Trim()) : 0;
            user.LoginEmail = !String.IsNullOrWhiteSpace(user.LoginEmail) ? user.LoginEmail.Trim() : null;
            user.FirstName = !String.IsNullOrWhiteSpace(user.FirstName) ? user.FirstName.Trim() : null;
            user.MiddleName = !String.IsNullOrWhiteSpace(user.MiddleName) ? user.MiddleName.Trim() : null;
            user.LastName1 = !String.IsNullOrWhiteSpace(user.LastName1) ? user.LastName1.Trim() : null;
            user.LastName2 = !String.IsNullOrWhiteSpace(user.LastName2) ? user.LastName2.Trim() : null;
            user.PhoneNumber = !String.IsNullOrWhiteSpace(user.PhoneNumber) ? user.PhoneNumber.Trim() : null;

            return user;
        }

        private string GetUrlImage(SMCLCore.Domain.Model.User user)
        {
            if (user.IsActive)
            {
                return "~/Content/Images/1accept.png";
            }
            else
            {
                return "~/Content/Images/4inactivar.png";
            }
        }

        private object GetRolesInList(FormCollection form, int action)
        {
            List<int> userRolesInForm = new List<int>();
            if (!String.IsNullOrEmpty(form["RoleIds"]))
            {
                foreach (object item in form["RoleIds"].Replace(",", "").ToString())
                {
                    userRolesInForm.Add(Convert.ToInt32(item.ToString()));
                }
                return new MultiSelectList(dbR.GetAll(), "Id", "Name", userRolesInForm);
            }
            else
            {
                switch (action)
                {
                    case 1:
                    default:
                        return new MultiSelectList(dbR.GetAll(), "Id", "Name", this.GetActiveRolesByDefault());
                    case 2:
                        return new MultiSelectList(dbR.GetAll(), "Id", "Name", null);
                }
            }
        }

        private bool FormCollectionToCreateIsValid(SMCLCore.Domain.Model.User user, FormCollection form)
        {
            double numberOut;
            if (!db.GetAll().Any(o => o.LoginEmail == user.LoginEmail) && !String.IsNullOrWhiteSpace(user.LoginEmail))
            {
                if (this.IsLoginEmailValid(user.LoginEmail))
                {
                    if (user.DocumentId > 0 && double.TryParse(user.DocumentId.ToString(), out numberOut))
                    {
                        if (!String.IsNullOrWhiteSpace(user.FirstName) && !String.IsNullOrWhiteSpace(user.LastName1))
                        {
                            if (!String.IsNullOrWhiteSpace(form["passwordPwd"]))
                            {
                                if (this.IsPasswordValid(form["passwordPwd"], form["UC"], form["LC"], form["NU"], form["SY"]))
                                {
                                    if (!String.IsNullOrEmpty(form["PassphraseID"]) && !String.IsNullOrWhiteSpace(form["passphraseValue"]))
                                    {
                                        if (!String.IsNullOrEmpty(form["RoleIds"]))
                                        {
                                            this.AddRolesInstances(user, form);
                                            return true;
                                        }
                                        else
                                        {
                                            ViewData["ValidationErrorMessage"] = "Roles: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        ViewData["ValidationErrorMessage"] = "Pregunta/respuesta secreta: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                                        return false;
                                    }
                                }
                                else
                                {
                                    ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["PasswordErrorMessage"].ToString();
                                    return false;
                                }
                            }
                            else
                            {
                                ViewData["ValidationErrorMessage"] = "Contraseña: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                                return false;
                            }
                        }
                        else
                        {
                            ViewData["ValidationErrorMessage"] = "Nombre y apellido: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                            return false;
                        }
                    }
                    else
                    {
                        ViewData["ValidationErrorMessage"] = "Nro. Documento: " + ConfigurationManager.AppSettings["NonZeroFieldErrorMessage"].ToString();
                        return false;
                    }
                }
                else
                {
                    ViewData["ValidationErrorMessage"] = "Login (E-mail): " + ConfigurationManager.AppSettings["InvalidFormatFieldErrorMessage"].ToString();
                    return false;
                }
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Login (E-mail): " + ConfigurationManager.AppSettings["EmptyOrDuplicatedFieldErrorMessage"].ToString();
                return false;
            }
        }



        private bool FormCollectionToUpdateIsValid(SMCLCore.Domain.Model.User user, FormCollection form)
        {
            if (!String.IsNullOrWhiteSpace(user.LoginEmail))
            {
                if (this.IsLoginEmailValid(user.LoginEmail))
                {
                    if (!String.IsNullOrWhiteSpace(user.FirstName) && !String.IsNullOrWhiteSpace(user.LastName1))
                    {
                        if (!String.IsNullOrWhiteSpace(form["passwordPwd"]))
                        {
                            if (this.IsPasswordValid(form["passwordPwd"], form["UC"], form["LC"], form["NU"], form["SY"]))
                            {
                                if (!String.IsNullOrEmpty(form["PassphraseID"]) && !String.IsNullOrWhiteSpace(form["passphraseValue"]))
                                {
                                    if (!String.IsNullOrEmpty(form["RoleIds"]))
                                    {
                                        foreach (UserRole userRole in dbUR.GetByUserId(user.Id))
                                        {
                                            dbUR.Delete(userRole.Id);
                                        }

                                        this.AddRolesInstances(user, form);
                                        return true;
                                    }
                                    else
                                    {
                                        ViewData["ValidationErrorMessage"] = "Roles: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                                        return false;
                                    }
                                }
                                else
                                {
                                    ViewData["ValidationErrorMessage"] = "Pregunta/respuesta secreta: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                                    return false;
                                }
                            }
                            else
                            {
                                ViewData["ValidationErrorMessage"] = ConfigurationManager.AppSettings["PasswordErrorMessage"].ToString();
                                return false;
                            }
                        }
                        else
                        {
                            ViewData["ValidationErrorMessage"] = "Contraseña: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                            return false;
                        }
                    }
                    else
                    {
                        ViewData["ValidationErrorMessage"] = "Nombre y apellido: " + ConfigurationManager.AppSettings["EmptyFieldErrorMessage"].ToString();
                        return false;
                    }
                }
                else
                {
                    ViewData["ValidationErrorMessage"] = "Login (E-mail): " + ConfigurationManager.AppSettings["InvalidFormatFieldErrorMessage"].ToString();
                    return false;
                }
            }
            else
            {
                ViewData["ValidationErrorMessage"] = "Login (E-mail): " + ConfigurationManager.AppSettings["EmptyOrDuplicatedFieldErrorMessage"].ToString();
                return false;
            }
        }



        private bool IsLoginEmailValid(string loginEmail)
        {
            String pattern = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(loginEmail, pattern))
            {
                if (Regex.Replace(loginEmail, pattern, String.Empty).Length > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool IsPasswordValid(string pwd, string uc, string lc, string nu, string sy)
        {
            try
            {
                if (pwd.Length < Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordLength"]))
                {
                    return false;
                }

                if (int.Parse(uc) < Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordUpperCaseLetters"]))
                {
                    return false;
                }

                if (int.Parse(lc) < Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordLowerCaseLetters"]))
                {
                    return false;
                }

                if (int.Parse(nu) < Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordNumerics"]))
                {
                    return false;
                }

                if (int.Parse(sy) < Convert.ToInt32(ConfigurationManager.AppSettings["MinPasswordSymbols"]))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        private void AddRolesInstances(User user, FormCollection form)
        {
            var roles = form["RoleIds"].Replace(",", "");
            foreach (Object role in roles)
            {
                UserRole userRole = new UserRole();
                userRole.Role = dbR.GetById(Convert.ToInt32(role.ToString()));
                userRole.User = user;
                user.Roles.Add(userRole);
            }
        }
    }

    class Passphrase
    {
        public int id { get; set; }
        public string value { get; set; }
    }
}