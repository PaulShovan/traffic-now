using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Implementation.Base
{
    public class Repository<T> : TrafficNowContext, IRepository<T> where T : class
    {
        public IMongoCollection<T> Collection { get; private set; }

        public Repository()
        {
            Collection = Database.GetCollection<T>(typeof(T).Name.ToLower());
        }
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
