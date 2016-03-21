using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Model.NotificationModel;
using TrafficNow.Repository.Implementation.Base;
using TrafficNow.Repository.Interface.Notification;

namespace TrafficNow.Repository.Implementation.Notification
{
    public class NotificationRepository : Repository<Model.NotificationModel.Notification>, INotificationRepository
    {
        public async Task<bool> AddNotification(Model.NotificationModel.Notification notification)
        {
            try
            {
                await Collection.InsertOneAsync(notification);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Model.NotificationModel.Notification>> GetNotification(string userId)
        {
            try
            {
                var projection = Builders<Model.NotificationModel.Notification>.Projection.Exclude("_id");
                var result = await Collection.Find(n => n.receipentUserId == userId).Project<Model.NotificationModel.Notification>(projection).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
