using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficNow.Repository.Interface.Base;

namespace TrafficNow.Repository.Interface.Notification
{
    public interface INotificationRepository : IRepository<Model.NotificationModel.Notification>
    {
        Task<bool> AddNotification(Model.NotificationModel.Notification notification);
        Task<List<Model.NotificationModel.Notification>> GetNotification(string userId, int skip, int limit);
    }
}
