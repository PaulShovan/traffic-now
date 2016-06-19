using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.Shout.DbModels;
using TrafficNow.Model.Shout.ViewModels;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.Shout
{
    public interface IShoutRepository : IRepository<Model.Shout.DbModels.Shout>
    {
        Task<ShoutViewModel> AddShout(Model.Shout.DbModels.Shout shout);
        Task<List<ShoutViewModel>> GetShouts(int? offset, int? count, string userId);
        Task<List<ShoutViewModel>> GetNearbyShouts(double lat, double lon, double rad, string userId);
        Task<List<ShoutViewModel>> GetFollowersShouts(int? offset, int? count, List<string> followers);
        Task<List<ShoutViewModel>> GetShoutsOfUser(int? offset, int? count, string userId);
        Task<ShoutViewModel> GetShoutById(string shoutId, string userId);
        Task<ShoutViewModel> GetSharedShout(string sharedLink, bool isAuthorized);
        Task<List<Comment>> GetShoutComments(string shoutId, int skip, int limit);
        Task<Model.Shout.DbModels.Shout> AddShoutComment(string shoutId, Comment comment);
        Task<Model.Shout.DbModels.Shout> AddLike(string shoutId, UserBasicInformation like);
        Task<bool> RemoveLike(string shoutId, UserBasicInformation like);
        Task<bool> IsAlreadyLiked(string shoutId, UserBasicInformation like);
        Task<List<UserBasicInformation>> GetLikes(string shoutId);
    }

}
