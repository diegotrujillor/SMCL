using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class SignalApplianceValueRepository : IRepository<SignalApplianceValue>
    {
        void IRepository<SignalApplianceValue>.Save(SignalApplianceValue entity)
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

        void IRepository<SignalApplianceValue>.Update(SignalApplianceValue entity)
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

        void IRepository<SignalApplianceValue>.Delete(int id)
        {
            IRepository<SignalApplianceValue> repo = new SignalApplianceValueRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<SignalApplianceValue>.Delete(Guid id)
        {
            IRepository<SignalApplianceValue> repo = new SignalApplianceValueRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        SignalApplianceValue IRepository<SignalApplianceValue>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        SignalApplianceValue IRepository<SignalApplianceValue>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<SignalApplianceValue>().Add(Restrictions.Eq("Id", id)).UniqueResult<SignalApplianceValue>();
            }
        }

        SignalApplianceValue IRepository<SignalApplianceValue>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<SignalApplianceValue>().Add(Restrictions.Eq("Id", id)).UniqueResult<SignalApplianceValue>();
            }
        }

        IList<SignalApplianceValue> IRepository<SignalApplianceValue>.GetByUserId(int id)
        {
            return new List<SignalApplianceValue>();
        }

        IList<SignalApplianceValue> IRepository<SignalApplianceValue>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<SignalApplianceValue>().Add(Restrictions.Eq(name, value)).List<SignalApplianceValue>();
                    }
                    else
                    {
                        return session.CreateCriteria<SignalApplianceValue>().Add(Restrictions.Disjunction()
                                                                             .Add(Restrictions.IsNull(name)))
                                                                             .List<SignalApplianceValue>();
                    }
                }
                else
                {
                    return new List<SignalApplianceValue>();
                }
            }
        }

        IList<SignalApplianceValue> IRepository<SignalApplianceValue>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<SignalApplianceValue> list = new List<SignalApplianceValue>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<SignalApplianceValue>().Add(conjunction).List<SignalApplianceValue>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<SignalApplianceValue> IRepository<SignalApplianceValue>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(SignalApplianceValue));

                return criteria.List<SignalApplianceValue>();
            }
        }
    }
}
