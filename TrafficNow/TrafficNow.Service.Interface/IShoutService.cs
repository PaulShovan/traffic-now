using System.Collections.Generic;
using System.Threading.Tasks;
using TrafficNow.Model.Shout.DbModels;
using TrafficNow.Model.Shout.ViewModels;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Service.Interface
{
    public interface IShoutService
    {
        Task<ShoutViewModel> AddShout(Shout shout);
        Task<List<ShoutViewModel>> GetFollowersShouts(int? offset, int? count, string userId);
        Task<Comment> AddShoutComment(string shoutId, Comment comment);
        Task<bool> AddOrRemoveLike(string shoutId, UserBasicInformation like);
        bool ValidateShout(Shout shout);
    }
}
