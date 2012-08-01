using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class UserRoleRepository : IRepository<UserRole>
    {
        void IRepository<UserRole>.Save(UserRole entity)
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

        void IRepository<UserRole>.Update(UserRole entity)
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

        void IRepository<UserRole>.Delete(int id)
        {
            IRepository<UserRole> repo = new UserRoleRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<UserRole>.Delete(Guid id)
        {
            IRepository<UserRole> repo = new UserRoleRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        UserRole IRepository<UserRole>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        UserRole IRepository<UserRole>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<UserRole>().Add(Restrictions.Eq("Id", id)).UniqueResult<UserRole>();
            }
        }

        IList<UserRole> IRepository<UserRole>.GetByUserId(int id)
        {
            IRepository<User> repoU = new UserRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<UserRole>().Add(Restrictions.Eq("User", repoU.GetById(id))).List<UserRole>();
            }
        }

        UserRole IRepository<UserRole>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<UserRole>().Add(Restrictions.Eq("Id", id)).UniqueResult<UserRole>();
            }
        }

        IList<UserRole> IRepository<UserRole>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<UserRole>().Add(Restrictions.Eq(name, value)).List<UserRole>();
                    }
                    else
                    {
                        return session.CreateCriteria<UserRole>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<UserRole>();
                    }
                }
                else
                {
                    return new List<UserRole>();
                }
            }
        }

        IList<UserRole> IRepository<UserRole>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<UserRole> list = new List<UserRole>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<UserRole>().Add(conjunction).List<UserRole>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<UserRole> IRepository<UserRole>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(UserRole));

                return criteria.List<UserRole>();
            }
        }
    }
}
