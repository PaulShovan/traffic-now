using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.User.DbModels;
using TrafficNow.Repository.Interface.Point;
using TrafficNow.Service.Interface;

namespace TrafficNow.Service.Implementation
{
    public class PointService : IPointService
    {
        private IPointRepository _pointRepository;
        public async Task<bool> InserPoint(string userId)
        {
            try
            {
                var userPoint = new Point();
                userPoint.userId = userId;
                userPoint.totalPoint = 2;
                return await _pointRepository.InserPoint(userPoint);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PointService(IPointRepository pointRepository)
        {
            _pointRepository = pointRepository;
        }
    }
}
