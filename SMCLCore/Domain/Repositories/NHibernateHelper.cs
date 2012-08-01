using NHibernate;
using NHibernate.Cfg;

namespace SMCLCore.Domain.Repositories
{
    public class NHibernateHelper
    {
        private static ISessionFactory sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    var configuration = new Configuration();
                    configuration.Configure();
                    sessionFactory = configuration.BuildSessionFactory();
                }

                return sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
