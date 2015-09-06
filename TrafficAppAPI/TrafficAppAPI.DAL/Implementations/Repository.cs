using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.DAL.Contracts;

namespace TrafficAppAPI.DAL.Implementations
{
    public class Repository<T> : TrafficNowContext, IRepository<T> where T : class
    {
        public bool Add(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public void UpdateEntity(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
