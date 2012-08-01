using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class RoleRepository : IRepository<Role>
    {
        void IRepository<Role>.Save(Role entity)
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

        void IRepository<Role>.Update(Role entity)
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

        void IRepository<Role>.Delete(int id)
        {
            IRepository<Role> repo = new RoleRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<Role>.Delete(Guid id)
        {
            IRepository<Role> repo = new RoleRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        Role IRepository<Role>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        Role IRepository<Role>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Role>().Add(Restrictions.Eq("Id", id)).UniqueResult<Role>();
            }
        }

        Role IRepository<Role>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<Role>().Add(Restrictions.Eq("Id", id)).UniqueResult<Role>();
            }
        }

        IList<Role> IRepository<Role>.GetByUserId(int id)
        {
            return new List<Role>();
        }

        IList<Role> IRepository<Role>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<Role>().Add(Restrictions.Eq(name, value)).List<Role>();
                    }
                    else
                    {
                        return session.CreateCriteria<Role>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<Role>();
                    }
                }
                else
                {
                    return new List<Role>();
                }
            }
        }

        IList<Role> IRepository<Role>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<Role> list = new List<Role>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<Role>().Add(conjunction).List<Role>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<Role> IRepository<Role>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Role));

                return criteria.List<Role>();
            }
        }
    }
}
