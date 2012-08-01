using System;
using System.Collections.Generic;

namespace SMCLCore
{
    public interface IRepository<T>
    {
        void Save(T entity);
        void Update(T entity);
        void Delete(int id);
        void Delete(Guid id);
        T GetById(int id);
        T GetById(Guid id);
        T GetByUserLogin(string login);
        IList<T> GetByUserId(int id);
        IList<T> GetByProperty(string name, object value);
        IList<T> GetByProperties(Dictionary<string, object> properties);
        IList<T> GetAll();
    }
}
