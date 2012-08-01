using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class SignalRepository : IRepository<Signal>
    {
        void IRepository<Signal>.Save(Signal entity)
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

        void IRepository<Signal>.Update(Signal entity)
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

        void IRepository<Signal>.Delete(int id)
        {
            IRepository<Signal> repo = new SignalRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Signal>.Delete(Guid id)
        {
            IRepository<Signal> repo = new SignalRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Signal IRepository<Signal>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Signal IRepository<Signal>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Signal>().Add(Restrictions.Eq("Id", id)).UniqueResult<Signal>();
            }
        }

        Signal IRepository<Signal>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Signal>().Add(Restrictions.Eq("Id", id)).UniqueResult<Signal>();
            }
        }

        IList<Signal> IRepository<Signal>.GetByUserId(int id)
        {
            return new List<Signal>();
        }

        IList<Signal> IRepository<Signal>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Signal>().Add(Restrictions.Eq(name, value)).List<Signal>();
                    }
                    else
                    {
                        return session.CreateCriteria<Signal>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Signal>();
                    }
                }
                else
                {
                    return new List<Signal>();
                }
            }
        }

        IList<Signal> IRepository<Signal>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Signal> list = new List<Signal>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Signal>().Add(conjunction).List<Signal>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Signal> IRepository<Signal>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Signal));

                return criteria.List<Signal>();
            }
        }
    }
}
