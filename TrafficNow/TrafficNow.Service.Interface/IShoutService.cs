using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Core.Shout.DataModel;
using TrafficNow.Core.Shout.ViewModel;
using TrafficNow.Core.User.Dto;

namespace TrafficNow.Service.Interface
{
    public interface IShoutService
    {
        Task<ShoutViewModel> AddShout(ShoutModel shout);
        Task<List<ShoutViewModel>> GetFollowersShouts(int? offset, int? count, string userId);
        Task<CommentViewModel> AddShoutComment(string shoutId, CommentModel comment);
        Task<bool> AddOrRemoveLike(string shoutId, LikerModel like);
        bool ValidateShout(ShoutModel shout);
    }
}
