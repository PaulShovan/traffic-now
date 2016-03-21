using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;

namespace TrafficNow.Service.Interface
{
    public interface IPointService
    {
        Task<bool> InserPoint(string userId);
        //Task<bool> AddPoint(string userId, int point, PointDescription details);
        //Task<int> GetUserPoint(string userId);
        //Task<Point> GetPointDetails(string userId);
    }
}
