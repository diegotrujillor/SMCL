using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class SignalApplianceRepository : IRepository<SignalAppliance>
    {
        void IRepository<SignalAppliance>.Save(SignalAppliance entity)
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

        void IRepository<SignalAppliance>.Update(SignalAppliance entity)
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

        void IRepository<SignalAppliance>.Delete(int id)
        {
            IRepository<SignalAppliance> repo = new SignalApplianceRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<SignalAppliance>.Delete(Guid id)
        {
            IRepository<SignalAppliance> repo = new SignalApplianceRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        SignalAppliance IRepository<SignalAppliance>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        SignalAppliance IRepository<SignalAppliance>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<SignalAppliance>().Add(Restrictions.Eq("Id", id)).UniqueResult<SignalAppliance>();
            }
        }

        SignalAppliance IRepository<SignalAppliance>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<SignalAppliance>().Add(Restrictions.Eq("Id", id)).UniqueResult<SignalAppliance>();
            }
        }

        IList<SignalAppliance> IRepository<SignalAppliance>.GetByUserId(int id)
        {
            return new List<SignalAppliance>();
        }

        IList<SignalAppliance> IRepository<SignalAppliance>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<SignalAppliance>().Add(Restrictions.Eq(name, value)).List<SignalAppliance>();
                    }
                    else
                    {
                        return session.CreateCriteria<SignalAppliance>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<SignalAppliance>();
                    }
                }
                else
                {
                    return new List<SignalAppliance>();
                }
            }
        }

        IList<SignalAppliance> IRepository<SignalAppliance>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<SignalAppliance> list = new List<SignalAppliance>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<SignalAppliance>().Add(conjunction).List<SignalAppliance>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<SignalAppliance> IRepository<SignalAppliance>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(SignalAppliance));

                return criteria.List<SignalAppliance>();
            }
        }
    }
}
