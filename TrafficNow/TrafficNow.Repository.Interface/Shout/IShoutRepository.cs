using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Core.Shout.ViewModel;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.Shout
{
    public interface IShoutRepository : IRepository<ShoutModel>
    {
        Task<ShoutViewModel> AddShout(ShoutModel shout);
        Task<List<ShoutViewModel>> GetShouts(int? offset, int? count);
        Task<List<ShoutViewModel>> GetFollowersShouts(int? offset, int? count, List<string> followers);
        Task<List<ShoutViewModel>> GetShouts(int? offset, int? count, string userId);
        Task<ShoutViewModel> GetShoutById(string shoutId);
        Task<List<CommentViewModel>> GetShoutComments(string shoutId, int skip, int limit);
        Task<CommentViewModel> AddShoutComment(string shoutId, CommentModel comment);
        Task<bool> AddLike(string shoutId, LikerModel like);
        Task<bool> RemoveLike(string shoutId, LikerModel like);
        Task<bool> IsAlreadyLiked(string shoutId, LikerModel like);
        Task<List<LikerViewModel>> GetLikes(string shoutId);
    }

}
