using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.Point
{
    public interface IPointRepository : IRepository<Model.User.DbModels.Point>
    {
        Task<bool> InserPoint(Model.User.DbModels.Point point);
        Task<bool> AddPoint(string userId, int point, PointDescription details);
        Task<Model.User.DbModels.Point> GetUserPoint(string userId);
        Task<List<Model.User.DbModels.Point>> GetPoints(string userId, int offset = 0, int count = 10);
        Task<Model.User.DbModels.Point> GetPointDetails(string userId);
        Task<List<Model.User.DbModels.Point>> GetFollowingsPoint(List<string> followingsId);
    }
}
