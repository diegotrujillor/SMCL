using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class EventRepository : IRepository<Event>
    {
        void IRepository<Event>.Save(Event entity)
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

        void IRepository<Event>.Update(Event entity)
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

        void IRepository<Event>.Delete(int id)
        {
            IRepository<Event> repo = new EventRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Event>.Delete(Guid id)
        {
            IRepository<Event> repo = new EventRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Event IRepository<Event>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Event IRepository<Event>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Event>().Add(Restrictions.Eq("Id", id)).UniqueResult<Event>();
            }
        }

        Event IRepository<Event>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Event>().Add(Restrictions.Eq("Id", id)).UniqueResult<Event>();
            }
        }

        IList<Event> IRepository<Event>.GetByUserId(int id)
        {
            return new List<Event>();
        }

        IList<Event> IRepository<Event>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Event>().Add(Restrictions.Eq(name, value)).List<Event>();
                    }
                    else
                    {
                        return session.CreateCriteria<Event>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Event>();
                    }
                }
                else
                {
                    return new List<Event>();
                }
            }
        }

        IList<Event> IRepository<Event>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Event> list = new List<Event>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Event>().Add(conjunction).List<Event>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Event> IRepository<Event>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Event));

                return criteria.List<Event>();
            }
        }
    }
}
