using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;
using TrafficAppAPI.Model.Dto;
using TrafficAppAPI.Repository.Contracts;
using TrafficAppAPI.Service.Contracts;

namespace TrafficAppAPI.Service.Implementations
{
    public class ShoutService : IShoutService
    {
        private IShoutRepository _shoutRepository;
        private bool IsValidTrafficCondition(string condition)
        {
            string[] conditions = new string[] { "High", "Medium", "Low" };
            if (String.IsNullOrWhiteSpace(condition))
            {
                return false;
            }
            return (Array.IndexOf(conditions, condition) > -1);
        }
        public bool ValidateShout(Shout shout)
        {
            if(String.IsNullOrWhiteSpace(shout.ShoutedByName) 
                || String.IsNullOrWhiteSpace(shout.ShoutedById) 
                || String.IsNullOrWhiteSpace(shout.Latitude) 
                || String.IsNullOrWhiteSpace(shout.Longitude)
                || String.IsNullOrWhiteSpace(shout.Location)
                || !IsValidTrafficCondition(shout.TrafficCondition))
            {
                return false;
            }
            return true;
        }
        public ShoutService(IShoutRepository shoutRepository)
        {
            _shoutRepository = shoutRepository;
        }
        public async Task<Shout> AddShout(Shout shout)
        {
            try
            {
                if (ValidateShout(shout))
                {
                    shout.ShoutId = Guid.NewGuid().ToString();
                    return await _shoutRepository.AddShout(shout);
                }
                return new Shout();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        public async Task<List<Shout>> GetShouts(int? skip, int? limit, string sort)
        {
            try
            {
                var shouts = await _shoutRepository.GetShouts(skip, limit, sort);
                return shouts;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Shout> GetShoutById(string shoutId)
        {
            try
            {
                var shout = await _shoutRepository.GetShoutById(shoutId);
                return shout;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Comment>> GetShoutComments(string shoutId, int skip, int limit)
        {
            try
            {
                var comments = await _shoutRepository.GetShoutComments(shoutId, skip, limit);
                return comments;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Comment> AddShoutComment(string shoutId, Comment comment)
        {
            try
            {
                comment.CommentId = Guid.NewGuid().ToString();
                var commentRes = await _shoutRepository.AddShoutComment(shoutId, comment);
                return commentRes;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
