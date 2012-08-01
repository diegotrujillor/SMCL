using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using SMCLCore.Domain.Model;
using System;

namespace SMCLCore.Domain.Repositories
{
    public class MappingTagRepository : IRepository<MappingTag>
    {
        void IRepository<MappingTag>.Save(MappingTag entity)
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

        void IRepository<MappingTag>.Update(MappingTag entity)
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

        void IRepository<MappingTag>.Delete(int id)
        {
            IRepository<MappingTag> repo = new MappingTagRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        void IRepository<MappingTag>.Delete(Guid id)
        {
            IRepository<MappingTag> repo = new MappingTagRepository();
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete(repo.GetById(id));
                    tx.Commit();
                }
            }
        }

        MappingTag IRepository<MappingTag>.GetByUserLogin(string loginEmail)
        {
            return null;
        }

        MappingTag IRepository<MappingTag>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<MappingTag>().Add(Restrictions.Eq("Id", id)).UniqueResult<MappingTag>();
            }
        }

        MappingTag IRepository<MappingTag>.GetById(Guid id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria<MappingTag>().Add(Restrictions.Eq("Id", id)).UniqueResult<MappingTag>();
            }
        }

        IList<MappingTag> IRepository<MappingTag>.GetByUserId(int id)
        {
            return new List<MappingTag>();
        }

        IList<MappingTag> IRepository<MappingTag>.GetByProperty(string name, object value)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                if (!String.IsNullOrEmpty(name))
                {
                    if (value != null)
                    {
                        return session.CreateCriteria<MappingTag>().Add(Restrictions.Eq(name, value)).List<MappingTag>();
                    }
                    else
                    {
                        return session.CreateCriteria<MappingTag>().Add(Restrictions.Disjunction()
                                                                   .Add(Restrictions.IsNull(name)))
                                                                   .List<MappingTag>();
                    }
                }
                else
                {
                    return new List<MappingTag>();
                }
            }
        }

        IList<MappingTag> IRepository<MappingTag>.GetByProperties(Dictionary<string, object> properties)
        {
            Conjunction conjunction = new Conjunction();
            IList<MappingTag> list = new List<MappingTag>();

            if (properties.Count > 0)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    foreach (var item in properties)
                    {
                        conjunction.Add(Restrictions.Eq(item.Key, item.Value));
                    }

                    return session.CreateCriteria<MappingTag>().Add(conjunction).List<MappingTag>();
                }
            }
            else
            {
                return list;
            }
        }

        IList<MappingTag> IRepository<MappingTag>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(MappingTag));

                return criteria.List<MappingTag>();
            }
        }
    }
}
