using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class LogRepository : IRepository<Log>
    {
        void IRepository<Log>.Save(Log entity)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Save(entity);
                    tx.Commit();
                }
            }
        }

        void IRepository<Log>.Update(Log entity)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Update(entity);
                    tx.Commit();
                }
            }
        }

        void IRepository<Log>.Delete(int id)
        {
            IRepository<Log> repo = new LogRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Log>.Delete(Guid id)
        {
            IRepository<Log> repo = new LogRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Log IRepository<Log>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Log IRepository<Log>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Log>().Add(Restrictions.Eq("Id", id)).UniqueResult<Log>();
            }
        }

        Log IRepository<Log>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Log>().Add(Restrictions.Eq("Id", id)).UniqueResult<Log>();
            }
        }

        IList<Log> IRepository<Log>.GetByUserId(int id)
        {
            return new List<Log>();
        }

        IList<Log> IRepository<Log>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Log>().Add(Restrictions.Eq(name, value)).List<Log>();
                    }
                    else
                    {
                        return session.CreateCriteria<Log>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Log>();
                    }
                }
                else
                {
                    return new List<Log>();
                }
            }
        }

        IList<Log> IRepository<Log>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Log> list = new List<Log>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Log>().Add(conjunction).List<Log>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Log> IRepository<Log>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Log));

                return criteria.List<Log>();
            }
        }
    }
}
