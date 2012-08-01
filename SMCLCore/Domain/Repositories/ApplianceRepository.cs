using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class ApplianceRepository : IRepository<Appliance>
    {
        void IRepository<Appliance>.Save(Appliance entity)
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

        void IRepository<Appliance>.Update(Appliance entity)
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

        void IRepository<Appliance>.Delete(int id)
        {
            IRepository<Appliance> repo = new ApplianceRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Appliance>.Delete(Guid id)
        {
            IRepository<Appliance> repo = new ApplianceRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Appliance IRepository<Appliance>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Appliance IRepository<Appliance>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Appliance>().Add(Restrictions.Eq("Id", id)).UniqueResult<Appliance>();
            }
        }

        Appliance IRepository<Appliance>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Appliance>().Add(Restrictions.Eq("Id", id)).UniqueResult<Appliance>();
            }
        }

        IList<Appliance> IRepository<Appliance>.GetByUserId(int id)
        {
            return new List<Appliance>();
        }

        IList<Appliance> IRepository<Appliance>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Appliance>().Add(Restrictions.Eq(name, value)).List<Appliance>();
                    }
                    else
                    {
                        return session.CreateCriteria<Appliance>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Appliance>();
                    }
                }
                else
                {
                    return new List<Appliance>();
                }
            }
        }

        IList<Appliance> IRepository<Appliance>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Appliance> list = new List<Appliance>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Appliance>().Add(conjunction).List<Appliance>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Appliance> IRepository<Appliance>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Appliance));

                return criteria.List<Appliance>();
            }
        }
    }
}
