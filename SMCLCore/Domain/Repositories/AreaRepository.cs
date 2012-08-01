using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class AreaRepository : IRepository<Area>
    {
        void IRepository<Area>.Save(Area entity)
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

        void IRepository<Area>.Update(Area entity)
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

        void IRepository<Area>.Delete(int id)
        {
            IRepository<Area> repo = new AreaRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Area>.Delete(Guid id)
        {
            IRepository<Area> repo = new AreaRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Area IRepository<Area>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Area IRepository<Area>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Area>().Add(Restrictions.Eq("Id", id)).UniqueResult<Area>();
            }
        }

        Area IRepository<Area>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Area>().Add(Restrictions.Eq("Id", id)).UniqueResult<Area>();
            }
        }

        IList<Area> IRepository<Area>.GetByUserId(int id)
        {
            return new List<Area>();
        }

        IList<Area> IRepository<Area>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Area>().Add(Restrictions.Eq(name, value)).List<Area>();
                    }
                    else
                    {
                        return session.CreateCriteria<Area>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Area>();
                    }
                }
                else
                {
                    return new List<Area>();
                }
            }
        }

        IList<Area> IRepository<Area>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Area> list = new List<Area>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Area>().Add(conjunction).List<Area>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Area> IRepository<Area>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Area));

                return criteria.List<Area>();
            }
        }
    }
}
