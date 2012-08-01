using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;
using System.Reflection;

namespace SMCLCore.Domain.Repositories
{
    public class UserRepository : IRepository<User>
    {
        void IRepository<User>.Save(User entity)
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

        void IRepository<User>.Update(User entity)
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

        void IRepository<User>.Delete(int id)
        {
            IRepository<User> repo = new UserRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<User>.Delete(Guid id)
        {
            IRepository<User> repo = new UserRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        User IRepository<User>.GetByUserLogin(string loginEmail)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<User>().Add(Restrictions.Eq("LoginEmail", loginEmail)).UniqueResult<User>();
            }
        }

        User IRepository<User>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<User>().Add(Restrictions.Eq("Id", id)).UniqueResult<User>();
            }
        }

        User IRepository<User>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<User>().Add(Restrictions.Eq("Id", id)).UniqueResult<User>();
            }
        }

        IList<User> IRepository<User>.GetByUserId(int id)
        {
            return new List<User>();
        }

        IList<User> IRepository<User>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<User>().Add(Restrictions.Eq(name, value)).List<User>();
                    }
                    else
                    {
                        return session.CreateCriteria<User>().Add(Restrictions.Disjunction()
                                                             .Add(Restrictions.IsNull(name)))
                                                             .List<User>();
                    }
                }
                else
                {
                    return new List<User>();
                }
            }
        }

        IList<User> IRepository<User>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<User> list = new List<User>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<User>().Add(conjunction).List<User>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<User> IRepository<User>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(User));

                return criteria.List<User>();
            }
        }
    }
}
