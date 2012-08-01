using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMCL.Services.LoggingInterface;
using SMCL.Models;
using System.Configuration;
using SMCLCore;
using SMCLCore.Domain.Model;
using SMCLCore.Domain.Repositories;

namespace SMCL.Services.LoggingImplementation
{
    public class LogSMCL : ILoggable
    {
        //Instanciando contextos de datos.
        private IRepository<Log> smcl = new LogRepository();
        //cnnSMCL smcl = new cnnSMCL();

        /// <summary>
        /// Get Log table size in number of rows.
        /// </summary>
        /// <returns>The number of records.</returns>
        int ILoggable.LogSize()
        {
            return smcl.GetAll().Count;
        }

        /// <summary>
        /// Make a new instance for a Log object.
        /// </summary>
        /// <param name="LogMessage"></param>
        /// <param name="EventID"></param>
        /// <param name="UserID"></param>
        /// <returns>A generic object.</returns>
        Object ILoggable.GetNewLog(string LogMessage, int EventID, int UserID)
        {
            var NewLog = new Log();
            IRepository<Event> eventRepo = new EventRepository();
            IRepository<User> eventUser = new UserRepository();

            try
            {
                NewLog.DateTime = DateTime.Now;
                NewLog.Text = LogMessage;
                NewLog.Event = eventRepo.GetById(EventID);
                NewLog.User = eventUser.GetById(UserID);

                return NewLog;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save each record of list on context database.
        /// </summary>
        /// <param name="LogItems"></param>
        void ILoggable.Write(List<Object> LogItems)
        {
            var cast = new Log();
            var model = new Log();
            try
            {
                foreach (var row in LogItems)
                {                    
                    cast = (Log)row;

                    model.DateTime = cast.DateTime;
                    model.Text = cast.Text;
                    model.Event = cast.Event;
                    model.User = cast.User;

                    smcl.Save(model);
                }
            }
            catch (Exception ex)
            {                
                throw new Exception(ConfigurationManager.AppSettings["UnknownError"].ToString(), ex);
            }
        }
    }
}