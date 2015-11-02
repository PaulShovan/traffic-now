using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficAppAPI.Model;
using TrafficAppAPI.Model.Dto;

namespace TrafficAppAPI.Service.Contracts
{
    public interface IShoutService
    {
        Task<Shout> AddShout(Shout shout);
        Task<List<Shout>> GetShouts(int? skip, int? limit, string sort);
        Task<Shout> GetShoutById(string shoutId);
        Task<List<Comment>> GetShoutComments(string shoutId, int skip, int limit);
        Task<Comment> AddShoutComment(string shoutId, Comment comment);
        Task<bool> AddOrRemoveLike(string shoutId, Liker like);
        Task<List<Liker>> GetLikes(string shoutId);
        bool ValidateShout(Shout shout);
    }
}
