using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Design.PluralizationServices;
using TrafficNow.Repository.Interface.Base;
using System.Globalization;

namespace TrafficNow.Repository.Implementation.Base
{
    public class Repository<T> : TrafficNowContext, IRepository<T> where T : class
    {
        public IMongoCollection<T> Collection { get; private set; }
        public IMongoCollection<T> PointCollection { get; private set; }

        public Repository()
        {
            var service = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            Collection = Database.GetCollection<T>(service.Pluralize(typeof(T).Name.ToLower()));
            PointCollection = Database.GetCollection<T>(service.Pluralize("point"));
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
