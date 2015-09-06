using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;
using TrafficAppAPI.Repository.Contracts;
using TrafficAppAPI.Service.Contracts;

namespace TrafficAppAPI.Service.Implementations
{
    public class ShoutService : IShoutService
    {
        private IShoutRepository _shoutRepository;
        public ShoutService(IShoutRepository shoutRepository)
        {
            _shoutRepository = shoutRepository;
        }
        public async Task<bool> AddShout(Shout shout)
        {
            await _shoutRepository.AddShout(shout);
            return true;
        }

        public List<Shout> GetShouts()
        {
            throw new NotImplementedException();
        }
    }
}
