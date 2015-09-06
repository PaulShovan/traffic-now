using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.DAL.Implementations;
using TrafficAppAPI.Model;
using TrafficAppAPI.Repository.Contracts;
using MongoDB.Driver;

namespace TrafficAppAPI.Repository.Implementations
{
    public class ShoutRepository : Repository<Shout>, IShoutRepository
    {
        public async Task<bool> AddShout(Shout shout)
        {
            await Collection.InsertOneAsync(shout);
            return true;
        }
    }
}
