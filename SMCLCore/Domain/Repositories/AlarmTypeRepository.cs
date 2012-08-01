using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class AlarmTypeRepository : IRepository<AlarmType>
    {
        void IRepository<AlarmType>.Save(AlarmType entity)
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

        void IRepository<AlarmType>.Update(AlarmType entity)
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

        void IRepository<AlarmType>.Delete(int id)
        {
            IRepository<AlarmType> repo = new AlarmTypeRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<AlarmType>.Delete(Guid id)
        {
            IRepository<AlarmType> repo = new AlarmTypeRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        AlarmType IRepository<AlarmType>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        AlarmType IRepository<AlarmType>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<AlarmType>().Add(Restrictions.Eq("Id", id)).UniqueResult<AlarmType>();
            }
        }

        AlarmType IRepository<AlarmType>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<AlarmType>().Add(Restrictions.Eq("Id", id)).UniqueResult<AlarmType>();
            }
        }

        IList<AlarmType> IRepository<AlarmType>.GetByUserId(int id)
        {
            return new List<AlarmType>();
        }

        IList<AlarmType> IRepository<AlarmType>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<AlarmType>().Add(Restrictions.Eq(name, value)).List<AlarmType>();
                    }
                    else
                    {
                        return session.CreateCriteria<AlarmType>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<AlarmType>();
                    }
                }
                else
                {
                    return new List<AlarmType>();
                }
            }
        }

        IList<AlarmType> IRepository<AlarmType>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<AlarmType> list = new List<AlarmType>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<AlarmType>().Add(conjunction).List<AlarmType>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<AlarmType> IRepository<AlarmType>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(AlarmType));

                return criteria.List<AlarmType>();
            }
        }
    }
}
