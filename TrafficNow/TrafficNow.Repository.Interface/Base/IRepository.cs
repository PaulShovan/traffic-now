using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficNow.Repository.Interface.Base
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        bool Add(T entity);
        void UpdateEntity(T entity);
        void Delete(T entity);
        void Delete(string id);
    }
}
