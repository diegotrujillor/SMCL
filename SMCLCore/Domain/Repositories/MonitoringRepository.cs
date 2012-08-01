using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using NHibernate.SqlCommand;

namespace SMCLCore.Domain.Repositories
{
    public class MonitoringRepository : IRepository<Monitoring>
    {
        void IRepository<Monitoring>.Save(Monitoring entity)
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

        void IRepository<Monitoring>.Update(Monitoring entity)
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

        void IRepository<Monitoring>.Delete(int id)
        {
            IRepository<Monitoring> repo = new MonitoringRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Monitoring>.Delete(Guid id)
        {
            IRepository<Monitoring> repo = new MonitoringRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Monitoring IRepository<Monitoring>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Monitoring IRepository<Monitoring>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Monitoring>().Add(Restrictions.Eq("Id", id)).UniqueResult<Monitoring>();
            }
        }

        Monitoring IRepository<Monitoring>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Monitoring>().Add(Restrictions.Eq("Id", id)).UniqueResult<Monitoring>();
            }
        }

        IList<Monitoring> IRepository<Monitoring>.GetByUserId(int id)
        {
            return new List<Monitoring>();
        }

        IList<Monitoring> IRepository<Monitoring>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Monitoring>().Add(Restrictions.Eq(name, value)).List<Monitoring>();
                    }
                    else
                    {
                        return session.CreateCriteria<Monitoring>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Monitoring>();
                    }
                }
                else
                {
                    return new List<Monitoring>();
                }
            }
        }

        IList<Monitoring> IRepository<Monitoring>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Monitoring> list = new List<Monitoring>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Monitoring>().Add(conjunction).List<Monitoring>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Monitoring> IRepository<Monitoring>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Monitoring));

                return criteria.List<Monitoring>();
            }
        }
    }
}
